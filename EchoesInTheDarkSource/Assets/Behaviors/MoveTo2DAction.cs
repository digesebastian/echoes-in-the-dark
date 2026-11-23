using System;
using System.Collections.Generic;
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
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Vector3> TargetPosition;

    public float speed = 2.5f;
    public float stoppingDistance = 0.25f;

    [Tooltip("How often (in seconds) to recalculate the A* path while moving (useful when chasing a moving target)")]
    public float repathInterval = 0.35f;

    [Header("Sprite Views (Optional)")]
    public Sprite frontSprite;
    public Sprite backSprite;
    public Sprite sideSprite;

    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rb2d;
    private List<Vector2> _path;
    private int _currentWaypointIndex;
    private float _repathTimer;
    private Vector2 _lastGoalPos;

    protected override Status OnStart()
    {
        if (Agent == null || Agent.Value == null)
            return Status.Failure;

        _spriteRenderer = Agent.Value.GetComponentInChildren<SpriteRenderer>();
        _rb2d = Agent.Value.GetComponent<Rigidbody2D>();

        Agent.Value.transform.rotation = Quaternion.identity;

        _repathTimer = 0f;
        if (!RecalculatePath())
        {
            Vector2 startPos = Agent.Value.transform.position;
            Vector2 goalPos = TargetPosition.Value;
            if (Vector2.Distance(startPos, goalPos) <= stoppingDistance)
                return Status.Success;

            return Status.Failure;
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Agent == null || Agent.Value == null)
            return Status.Failure;

        Vector2 currentPos = Agent.Value.transform.position;
        Vector2 currentGoal = TargetPosition.Value;

        // Check overall arrival at final destination
        if (Vector2.Distance(currentPos, currentGoal) <= stoppingDistance)
        {
            StopMovement();
            return Status.Success;
        }

        // Periodically recalculate A* path if the target moved (e.g., chasing the player)
        _repathTimer += Time.deltaTime;
        if (_repathTimer >= repathInterval && Vector2.Distance(currentGoal, _lastGoalPos) > 0.25f)
        {
            _repathTimer = 0f;
            RecalculatePath();
        }

        if (_path == null || _currentWaypointIndex >= _path.Count)
        {
            StopMovement();
            return Status.Success;
        }

        Vector2 waypoint = _path[_currentWaypointIndex];
        float distance = Vector2.Distance(currentPos, waypoint);

        // Advance to next waypoint along the A* path
        if (distance <= stoppingDistance)
        {
            _currentWaypointIndex++;
            if (_currentWaypointIndex >= _path.Count)
            {
                StopMovement();
                return Status.Success;
            }
            waypoint = _path[_currentWaypointIndex];
        }

        Vector2 dir = (waypoint - currentPos).normalized;

        if (_rb2d != null && !_rb2d.isKinematic)
        {
            _rb2d.linearVelocity = dir * speed;
        }
        else
        {
            Agent.Value.transform.position = Vector2.MoveTowards(currentPos, waypoint, speed * Time.deltaTime);
        }

        UpdateFacing(dir);

        // Enforce flat 2D rotation every frame
        Vector3 rot = Agent.Value.transform.eulerAngles;
        if (rot.x != 0f || rot.y != 0f)
        {
            Agent.Value.transform.rotation = Quaternion.Euler(0f, 0f, rot.z);
        }

        return Status.Running;
    }

    private bool RecalculatePath()
    {
        Vector2 startPos = Agent.Value.transform.position;
        Vector2 goalPos = TargetPosition.Value;
        _lastGoalPos = goalPos;

        if (GridPathfinder2D.Instance != null)
        {
            List<Vector2> newPath = GridPathfinder2D.Instance.FindPath(startPos, goalPos);
            if (newPath != null && newPath.Count > 0)
            {
                _path = newPath;
                // Skip index 0 if the guard is already standing right on the first node
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
        if (_rb2d != null)
            _rb2d.linearVelocity = Vector2.zero;
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
        StopMovement();
    }
}