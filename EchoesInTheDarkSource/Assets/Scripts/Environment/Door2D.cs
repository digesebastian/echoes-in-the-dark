using System.Collections;
using Unity.Behavior;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(BoxCollider2D))]
public class Door2D : MonoBehaviour
{
    [Header("Door State")]
    public bool isOpen = false;
    public bool isLocked = false;

    [Header("Layer Settings")]
    [Tooltip("Layer when unlocked (NOT in GridPathfinder2D Unwalkable Mask so enemies path through it)")]
    public string unlockedLayerName = "Doors";
    [Tooltip("Layer when locked (MUST be in GridPathfinder2D Unwalkable Mask so enemies path around it)")]
    public string lockedLayerName = "Obstacles";

    [Header("Player Interaction")]
    [Tooltip("Key to open/close the door when standing near it")]
    public KeyCode interactKey = KeyCode.E;
    [Tooltip("If true, doors opened by the player will also auto-close after autoCloseDelay")]
    public bool autoCloseForPlayer = false;

    [Header("Enemy Behavior")]
    [Tooltip("Allow enemies to automatically open unlocked doors when approaching")]
    public bool enemiesCanOpen = true;
    [Tooltip("Seconds after an enemy passes through before the door closes behind them (0 = stay open)")]
    public float autoCloseDelay = 2.0f;

    [Header("Visuals (Optional)")]
    public Sprite closedSprite;
    public Sprite openSprite;
    [Tooltip("Rotate door when open (e.g., 90 degrees if using a hinge pivot)")]
    public float openRotationAngle = 0f;

    private BoxCollider2D _solidCollider;
    private SpriteRenderer _spriteRenderer;
    private Quaternion _closedRotation;
    private Bounds _doorBounds;
    private bool _playerInRange;
    private int _entitiesInDoorway;
    private Coroutine _autoCloseRoutine;

    void Awake()
    {
        BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
        foreach (var col in colliders)
        {
            if (!col.isTrigger)
            {
                _solidCollider = col;
                break;
            }
        }

        if (_solidCollider == null)
        {
            _solidCollider = gameObject.AddComponent<BoxCollider2D>();
            _solidCollider.isTrigger = false;
        }

        _doorBounds = _solidCollider.bounds;
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _closedRotation = transform.localRotation;
    }

    void Start()
    {
        SyncLayerWithLockState(updateGrid: false);
        ApplyDoorState(isOpen, updateGrid: true);
    }

    void Update()
    {
        if (_playerInRange && WasInteractPressed())
        {
            if (isLocked)
            {
                Debug.Log($"{name} is locked!");
                return;
            }

            bool nextState = !isOpen;
            SetOpen(nextState, triggerAutoClose: nextState && autoCloseForPlayer);
        }
    }

    private bool WasInteractPressed()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            return true;
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(interactKey))
            return true;
#endif
        return false;
    }

    public void ToggleDoor()
    {
        if (isLocked) return;
        SetOpen(!isOpen, triggerAutoClose: !isOpen && autoCloseForPlayer);
    }

    public void SetOpen(bool open, bool triggerAutoClose = true)
    {
        if (isOpen == open) return;
        isOpen = open;
        ApplyDoorState(isOpen, updateGrid: true);

        if (_autoCloseRoutine != null)
        {
            StopCoroutine(_autoCloseRoutine);
            _autoCloseRoutine = null;
        }

        if (isOpen && triggerAutoClose && autoCloseDelay > 0f)
        {
            _autoCloseRoutine = StartCoroutine(AutoCloseCoroutine());
        }
    }

    public void SetLocked(bool locked)
    {
        if (isLocked == locked) return;
        isLocked = locked;

        if (isLocked && isOpen)
        {
            isOpen = false;
            ApplyDoorState(false, updateGrid: false);
        }

        SyncLayerWithLockState(updateGrid: true);
    }

    private void SyncLayerWithLockState(bool updateGrid)
    {
        string targetLayerName = isLocked ? lockedLayerName : unlockedLayerName;
        int targetLayer = LayerMask.NameToLayer(targetLayerName);

        if (targetLayer != -1)
        {
            gameObject.layer = targetLayer;
        }
        else
        {
            Debug.LogWarning($"Door2D on {name}: Layer '{targetLayerName}' does not exist! Create it in Edit > Project Settings > Tags and Layers.");
        }

        if (updateGrid && GridPathfinder2D.Instance != null)
        {
            GridPathfinder2D.Instance.UpdateBounds(_doorBounds);
        }
    }

    private void ApplyDoorState(bool open, bool updateGrid)
    {
        _solidCollider.enabled = !open;

        if (_spriteRenderer != null)
        {
            if (open && openSprite != null)
                _spriteRenderer.sprite = openSprite;
            else if (!open && closedSprite != null)
                _spriteRenderer.sprite = closedSprite;
            else if (openSprite == null && closedSprite == null && openRotationAngle == 0f)
            {
                Color c = _spriteRenderer.color;
                c.a = open ? 0.35f : 1f;
                _spriteRenderer.color = c;
            }
        }

        if (openRotationAngle != 0f)
        {
            transform.localRotation = open
                ? _closedRotation * Quaternion.Euler(0f, 0f, openRotationAngle)
                : _closedRotation;
        }

        if (updateGrid && GridPathfinder2D.Instance != null)
        {
            GridPathfinder2D.Instance.UpdateBounds(_doorBounds);
        }
    }

    private IEnumerator AutoCloseCoroutine()
    {
        yield return new WaitForSeconds(autoCloseDelay);

        while (_entitiesInDoorway > 0)
        {
            yield return new WaitForSeconds(0.2f);
        }

        SetOpen(false, triggerAutoClose: false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.isTrigger) return;

        if (IsPlayer(other))
        {
            _playerInRange = true;
            _entitiesInDoorway++;
        }
        else if (IsEnemy(other))
        {
            _entitiesInDoorway++;
            if (enemiesCanOpen && !isLocked && !isOpen)
            {
                SetOpen(true, triggerAutoClose: true);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.isTrigger) return;

        if (enemiesCanOpen && !isLocked && !isOpen && IsEnemy(other))
        {
            SetOpen(true, triggerAutoClose: true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.isTrigger) return;

        if (IsPlayer(other))
        {
            _playerInRange = false;
            _entitiesInDoorway = Mathf.Max(0, _entitiesInDoorway - 1);
        }
        else if (IsEnemy(other))
        {
            _entitiesInDoorway = Mathf.Max(0, _entitiesInDoorway - 1);
        }
    }

    private bool IsPlayer(Collider2D col)
    {
        return col.CompareTag("Player") || (col.transform.root != null && col.transform.root.CompareTag("Player"));
    }

    private bool IsEnemy(Collider2D col)
    {
        return col.GetComponentInParent<BehaviorGraphAgent>() != null;
    }
}