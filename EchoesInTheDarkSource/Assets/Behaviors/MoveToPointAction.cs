using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move to Point", story: "Move to first point in [list]", category: "Action", id: "1007a5536b208a6d9bf80bafe802b46d")]
public partial class MoveToPointAction : Action
{
    [SerializeReference] public BlackboardVariable<List<GameObject>> List;
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<float> Speed;

    [Tooltip("How often (in seconds) to recalculate the A* path when the target moves")]
    public float repathInterval = 0.3f;
    public float stoppingDistance = 0.25f;

    private enum SpeedState
    {
        Idle,
        Walking,
        Sprinting
    }

    private Animator _animator;
    private Rigidbody2D _rb2d;

    private static readonly int HorizontalMovement = Animator.StringToHash("HorizontalMovement");
    private static readonly int VerticalMovement = Animator.StringToHash("VerticalMovement");
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int IsSprinting = Animator.StringToHash("IsSprinting");
    private static readonly int PreviousHorizontalMovement = Animator.StringToHash("PreviousHorizontalMovement");
    private static readonly int PreviousVerticalMovement = Animator.StringToHash("PreviousVerticalMovement");

    private Vector2 _previousMovement;
    private List<Vector2> _path;
    private int _currentWaypointIndex;
    private float _repathTimer;
    private Vector2 _lastGoalPos;
    private GameObject _lastTargetObj;

    // Anti-stuck tracking
    private Vector2 _lastProgressPos;
    private float _stuckTimer;

    protected override Status OnStart()
    {
        if (GameObject == null || List == null || List.Value == null || List.Value.Count == 0 || List.Value[0] == null)
            return Status.Failure;

        _animator = GameObject.GetComponentInChildren<Animator>();
        _rb2d = GameObject.GetComponent<Rigidbody2D>();

        _repathTimer = 0f;
        _stuckTimer = 0f;
        _lastProgressPos = GameObject.transform.position;

        if (!RecalculatePath(List.Value[0]))
        {
            Vector2 currentPos = GameObject.transform.position;
            Vector2 targetPos = List.Value[0].transform.position;
            if (Vector2.Distance(currentPos, targetPos) <= stoppingDistance)
            {
                UpdateAnimator(Vector2.zero, SpeedState.Idle);
                return Status.Success;
            }
            return Status.Failure;
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (GameObject == null || List == null)
            return Status.Failure;

        var listItems = List.Value;
        if (listItems == null || listItems.Count == 0 || listItems[0] == null)
        {
            StopMovement();
            return Status.Failure;
        }

        GameObject targetObj = listItems[0];
        Vector2 currentPos = GameObject.transform.position;
        Vector2 finalGoalPos = targetObj.transform.position;
        float moveSpeed = (Speed != null && Speed.Value > 0f) ? Speed.Value : 3.0f;

        // Check arrival at final target
        if ((finalGoalPos - currentPos).sqrMagnitude <= stoppingDistance * stoppingDistance)
        {
            StopMovement();
            GameObject.transform.position = new Vector3(finalGoalPos.x, finalGoalPos.y, GameObject.transform.position.z);
            UpdateAnimator(Vector2.zero, SpeedState.Idle);
            return Status.Success;
        }

        // Recalculate A* path if target moved
        _repathTimer += Time.deltaTime;
        if (targetObj != _lastTargetObj || (_repathTimer >= repathInterval && Vector2.Distance(finalGoalPos, _lastGoalPos) > 0.25f))
        {
            _repathTimer = 0f;
            RecalculatePath(targetObj);
        }

        if (_path == null || _currentWaypointIndex >= _path.Count)
        {
            // Arrived at the closest walkable A* node near the target
            StopMovement();
            UpdateAnimator(Vector2.zero, SpeedState.Idle);
            return Status.Success;
        }

        Vector2 currentWaypoint = _path[_currentWaypointIndex];
        if ((currentWaypoint - currentPos).sqrMagnitude <= stoppingDistance * stoppingDistance)
        {
            _currentWaypointIndex++;
            if (_currentWaypointIndex >= _path.Count)
            {
                StopMovement();
                UpdateAnimator(Vector2.zero, SpeedState.Idle);
                return Status.Success;
            }
            currentWaypoint = _path[_currentWaypointIndex];
        }

        Vector2 direction = (currentWaypoint - currentPos).normalized;

        // Stuck detection: if barely moved over 0.25s, slide around obstacle and repath
        if (Vector2.Distance(currentPos, _lastProgressPos) < 0.05f)
        {
            _stuckTimer += Time.deltaTime;
            if (_stuckTimer >= 0.25f)
            {
                _stuckTimer = 0f;
                // Nudge perpendicular to current direction to unhook from wall corner
                Vector2 perp = new Vector2(-direction.y, direction.x);
                direction = (direction + perp * 1.25f).normalized;
                RecalculatePath(targetObj);
            }
        }
        else
        {
            _lastProgressPos = currentPos;
            _stuckTimer = 0f;
        }

        if (direction.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            GameObject.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }

        UpdateAnimator(direction, SpeedState.Walking);

        if (_rb2d != null && _rb2d.bodyType == RigidbodyType2D.Dynamic)
        {
            _rb2d.linearVelocity = direction * moveSpeed;
        }
        else
        {
            Vector2 nextPos = Vector2.MoveTowards(currentPos, currentWaypoint, moveSpeed * Time.deltaTime);
            GameObject.transform.position = new Vector3(nextPos.x, nextPos.y, GameObject.transform.position.z);
        }

        return Status.Running;
    }

    private bool RecalculatePath(GameObject targetObj)
    {
        if (targetObj == null) return false;

        Vector2 startPos = GameObject.transform.position;
        Vector2 goalPos = targetObj.transform.position;
        _lastGoalPos = goalPos;
        _lastTargetObj = targetObj;

        if (GridPathfinder2D.Instance != null)
        {
            List<Vector2> newPath = GridPathfinder2D.Instance.FindPath(startPos, goalPos);
            if (newPath != null && newPath.Count > 0)
            {
                _path = newPath;
                _currentWaypointIndex = (_path.Count > 1 && Vector2.Distance(startPos, _path[0]) < stoppingDistance) ? 1 : 0;
                return true;
            }
            return false;
        }
        else
        {
            _path = new List<Vector2> { goalPos };
            _currentWaypointIndex = 0;
            return true;
        }
    }

    private void StopMovement()
    {
        if (_rb2d != null && _rb2d.bodyType == RigidbodyType2D.Dynamic)
            _rb2d.linearVelocity = Vector2.zero;
    }

    private void UpdateAnimator(Vector2 direction, SpeedState speedState)
    {
        if (_animator == null) return;

        bool isMoving = speedState != SpeedState.Idle;
        _animator.SetBool(IsMoving, isMoving);
        _animator.SetBool(IsSprinting, speedState == SpeedState.Sprinting);

        if (isMoving)
        {
            _animator.SetFloat(HorizontalMovement, direction.x);
            _animator.SetFloat(VerticalMovement, direction.y);
            _previousMovement = direction;
        }
        else
        {
            _animator.SetFloat(PreviousHorizontalMovement, _previousMovement.x);
            _animator.SetFloat(PreviousVerticalMovement, _previousMovement.y);
        }
    }

    protected override void OnEnd()
    {
        StopMovement();
        UpdateAnimator(Vector2.zero, SpeedState.Idle);
    }
}