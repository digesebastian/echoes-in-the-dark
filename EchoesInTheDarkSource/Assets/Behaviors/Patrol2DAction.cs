using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using System.Collections.Generic;
using UnityEngine.Serialization;

[Serializable, NodeDescription(name: "Patrol2D", story: "Guard patrols along [Waypoints]", category: "Action", id: "")]
public class Patrol2DAction : Action
{
    [SerializeReference] public BlackboardVariable<List<GameObject>> Waypoints;
    [SerializeReference] public BlackboardVariable<float> Speed;
    [SerializeReference] public BlackboardVariable<float> WaitTime;

    private int currentIndex = 0;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    
    private enum SpeedState
    {
        Idle,
        Walking,
        Sprinting
    }
    
    private Animator _animator;

    private static readonly int HorizontalMovement = Animator.StringToHash("HorizontalMovement");
    private static readonly int VerticalMovement = Animator.StringToHash("VerticalMovement");
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int IsSprinting = Animator.StringToHash("IsSprinting");
    private static readonly int PreviousHorizontalMovement = Animator.StringToHash("PreviousHorizontalMovement");
    private static readonly int PreviousVerticalMovement = Animator.StringToHash("PreviousVerticalMovement");
        
    private float _horizontalMovement;
    private float _verticalMovement;
    private Vector2 _previousMovement;

    protected override Status OnStart()
    {
        if (Waypoints.Value == null || Waypoints.Value.Count == 0) return Status.Failure;
        _animator = GameObject.GetComponent<Animator>();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        var waypointsList = Waypoints.Value;
        if (waypointsList == null || waypointsList.Count == 0) return Status.Failure;

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                // go to the next point in a circular fashion
                currentIndex = (currentIndex + 1) % waypointsList.Count;
            }

            return Status.Running;
        }

        Transform target = waypointsList[currentIndex].transform;
        Vector2 direction = (Vector2)target.position - (Vector2)GameObject.transform.position;
        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            GameObject.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }

        Vector2 movementDir = isWaiting ? Vector2.zero : direction;
        SpeedState currentSpeedState = isWaiting ? SpeedState.Idle : SpeedState.Walking;
        UpdateAnimator(movementDir, currentSpeedState);
        
        GameObject.transform.position = Vector2.MoveTowards(GameObject.transform.position, target.position,
            Speed.Value * Time.deltaTime);

        if (Vector2.Distance(GameObject.transform.position, target.position) <= 0.1f)
        {
            isWaiting = true;
            waitTimer = WaitTime.Value;
        }

        return Status.Running;
    }
    
    private void UpdateAnimator(Vector2 direction, SpeedState speedState)
    {
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
}