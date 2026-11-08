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
        
    private Vector2 _previousMovement;

    protected override Status OnStart()
    {
        if (List.Value == null || List.Value.Count == 0) return Status.Failure;
        _animator = GameObject.GetComponent<Animator>();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        var ListList = List.Value;
        if (ListList == null || ListList.Count == 0) return Status.Failure;
        
        Transform target = ListList[0].transform; // target always the first point in the list
        
        Vector2 direction = (Vector2)target.position - (Vector2)GameObject.transform.position;
        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            GameObject.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }

        UpdateAnimator(direction, SpeedState.Walking);
        
        GameObject.transform.position = Vector2.MoveTowards(
            GameObject.transform.position,
            target.position,
            Speed.Value * Time.deltaTime
        );
        
        float distanceSq = ((Vector2)target.position - (Vector2)GameObject.transform.position).sqrMagnitude;
        if (distanceSq <= 0.2f * 0.2f)
        {
            GameObject.transform.position = target.position;
            return Status.Success; // moves on to the next node
        }

        return Status.Running;
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
}