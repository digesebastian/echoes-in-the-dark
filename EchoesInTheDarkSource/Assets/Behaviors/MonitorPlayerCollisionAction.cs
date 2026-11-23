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

    private Collider2D _guardCollider;
    private Collider2D _playerCollider;
    private GameObject _chaseTargetMarker;
    private float _catchRadius;

    protected override Status OnStart()
    {
        if (GameObject == null)
        {
            Debug.LogError("MonitorPlayerCollisionAction: Guard GameObject is null.");
            return Status.Failure;
        }

        _guardCollider = GameObject.GetComponentInChildren<Collider2D>();

        if (Player != null && Player.Value != null)
        {
            _playerCollider = Player.Value.GetComponentInChildren<CircleCollider2D>();
            if (_playerCollider == null)
                _playerCollider = Player.Value.GetComponentInChildren<Collider2D>();
        }

        if (_guardCollider == null || _playerCollider == null)
        {
            Debug.LogWarning($"MonitorPlayerCollisionAction: Missing Collider2D on guard ({_guardCollider != null}) or player ({_playerCollider != null}).");
            return Status.Failure;
        }

        _chaseTargetMarker = new GameObject("ChaseTargetMarker");
        _catchRadius = 1.2f;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (GameObject == null || Player == null || Player.Value == null)
            return Status.Failure;

        if (_guardCollider == null || _playerCollider == null)
            return Status.Failure;

        float distance = Vector2.Distance(GameObject.transform.position, Player.Value.transform.position);
        if (distance <= _catchRadius)
        {
            Debug.Log("Player Caught! Game Over.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return Status.Success;
        }

        bool isOverlapping = _guardCollider.IsTouching(_playerCollider);

        if (IsPlayerInRadius != null)
        {
            IsPlayerInRadius.Value = isOverlapping;
        }

        if (isOverlapping && _chaseTargetMarker != null)
        {
            _chaseTargetMarker.transform.position = Player.Value.transform.position;

            if (HighPoints != null)
            {
                if (HighPoints.Value == null)
                    HighPoints.Value = new List<GameObject>();

                if (!HighPoints.Value.Contains(_chaseTargetMarker))
                    HighPoints.Value.Add(_chaseTargetMarker);
            }
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (HighPoints != null && HighPoints.Value != null && _chaseTargetMarker != null)
        {
            HighPoints.Value.Remove(_chaseTargetMarker);
        }

        if (_chaseTargetMarker != null)
        {
            UnityEngine.Object.Destroy(_chaseTargetMarker);
            _chaseTargetMarker = null;
        }
    }
}