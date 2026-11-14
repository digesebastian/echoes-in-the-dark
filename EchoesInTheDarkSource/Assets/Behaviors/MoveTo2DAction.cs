using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, Unity.Properties.GeneratePropertyBag]
[NodeDescription(
    name: "Move To 2D",
    story: "Move [Agent] to [TargetPosition]",
    category: "Action/Navigation",
    id: "f2b3c4d5e6a17890123456789abcdef1")]
public partial class MoveTo2DAction : Action
{
    // Explicit declarations ensure the compiler always finds them
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Vector3> TargetPosition;

    // Direct block inputs
    public float speed = 2.0f;
    public float stoppingDistance = 0.2f;

    [Header("Sprite Views (Optional)")]
    public Sprite frontSprite;
    public Sprite backSprite;
    public Sprite sideSprite;

    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rb2d;

    protected override Status OnStart()
    {
        if (Agent == null || Agent.Value == null)
            return Status.Failure;

        _spriteRenderer = Agent.Value.GetComponentInChildren<SpriteRenderer>();
        _rb2d = Agent.Value.GetComponent<Rigidbody2D>();

        // Lock transform rotation flat to camera
        Agent.Value.transform.rotation = Quaternion.identity;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Agent == null || Agent.Value == null)
            return Status.Failure;

        Vector2 currentPos = Agent.Value.transform.position;
        Vector2 target = TargetPosition.Value;
        float distance = Vector2.Distance(currentPos, target);

        // Check arrival
        if (distance <= stoppingDistance)
        {
            if (_rb2d != null)
                _rb2d.linearVelocity = Vector2.zero;

            return Status.Success;
        }

        Vector2 dir = (target - currentPos).normalized;

        if (_rb2d != null && !_rb2d.isKinematic)
        {
            _rb2d.linearVelocity = dir * speed;
        }
        else
        {
            Agent.Value.transform.position = Vector2.MoveTowards(currentPos, target, speed * Time.deltaTime);
        }

        UpdateFacing(dir);

        // Prevent Y-90 degree flips
        Vector3 rot = Agent.Value.transform.eulerAngles;
        if (rot.x != 0f || rot.y != 0f)
        {
            Agent.Value.transform.rotation = Quaternion.Euler(0f, 0f, rot.z);
        }

        return Status.Running;
    }

    private void UpdateFacing(Vector2 direction)
    {
        if (_spriteRenderer == null) return;

        if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
        {
            if (direction.y > 0)
            {
                if (backSprite != null) _spriteRenderer.sprite = backSprite;
            }
            else
            {
                if (frontSprite != null) _spriteRenderer.sprite = frontSprite;
            }
            _spriteRenderer.flipX = false;
        }
        else
        {
            if (sideSprite != null) _spriteRenderer.sprite = sideSprite;
            _spriteRenderer.flipX = (direction.x < 0);
        }
    }

    protected override void OnEnd()
    {
        if (_rb2d != null)
            _rb2d.linearVelocity = Vector2.zero;
    }
}