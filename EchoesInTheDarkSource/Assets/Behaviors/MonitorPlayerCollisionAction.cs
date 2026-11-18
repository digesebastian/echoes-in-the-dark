using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.SceneManagement;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Monitor Player Collision", story: "Monitor [Player] Collisions", category: "Action",
    id: "a4f61fd866640df82f50f97823df5f9c")]
public partial class MonitorPlayerCollisionAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<List<GameObject>> HighPoints;
    [SerializeReference] public BlackboardVariable<bool> IsPlayerInRadius;

    private Collider2D _guardCircleCollider;
    private Collider2D _guardCollider;
    private Collider2D _playerCircleCollider;
    private Collider2D _playerCollider;
    private GameObject _chaseTargetMarker;

    protected override Status OnStart()
    {
        _guardCircleCollider = GameObject.GetComponentInChildren<CircleCollider2D>();
        _guardCollider = GameObject.GetComponent<BoxCollider2D>();

        if (Player.Value != null) _playerCircleCollider = Player.Value.GetComponentInChildren<CircleCollider2D>();
        if (Player.Value != null) _playerCollider = Player.Value.GetComponent<BoxCollider2D>();
        
        _chaseTargetMarker = new GameObject("ChaseTargetMarker");

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_guardCircleCollider == null || _playerCircleCollider == null || _playerCollider == null) return Status.Failure;
        if (_guardCollider.IsTouching(_playerCollider))
        {
            Debug.Log("Player Caught! Game Over.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return Status.Success;
        }

        bool isOverlapping = _guardCircleCollider.IsTouching(_playerCircleCollider);
        IsPlayerInRadius.Value = isOverlapping;

        if (isOverlapping)
        {
            _chaseTargetMarker.transform.position = Player.Value.transform.position;

            if (!HighPoints.Value.Contains(_chaseTargetMarker)) HighPoints.Value.Add(_chaseTargetMarker);
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {

        if (_chaseTargetMarker != null) UnityEngine.Object.Destroy(_chaseTargetMarker);
    }
}