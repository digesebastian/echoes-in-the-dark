using System;
using System.Collections.Generic;
using Projectiles;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Monitor Coin Collision", story: "Monitor Coin Collisions", category: "Action",
    id: "3079c3db302f240c4ea433b0662c19b0")]
public partial class MonitorCoinCollisionAction : Action
{
    [SerializeReference] public BlackboardVariable<List<GameObject>> CoinPoints;
    [SerializeReference] public BlackboardVariable<bool> IsPlayerInRadius;

    private CircleCollider2D _guardCollider;
    private readonly HashSet<CoinProjectile> _heardCoins = new();
    private int _coinLayerMask;

    protected override Status OnStart()
    {
        _guardCollider = GameObject.GetComponent<CircleCollider2D>();
        _coinLayerMask = LayerMask.GetMask("Projectile");
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_guardCollider == null) return Status.Failure;

        _heardCoins.RemoveWhere(coin => coin == null);
        Collider2D[] hits = Physics2D.OverlapCircleAll(
                    _guardCollider.bounds.center,
                    _guardCollider.radius,
                    _coinLayerMask
                );

        foreach (Collider2D hit in hits)
        {
            CoinProjectile coin = hit.GetComponentInParent<CoinProjectile>();
            if (coin == null || !coin.HasLanded || !_heardCoins.Add(coin)) continue;

            if (IsPlayerInRadius) continue;
            
            GameObject coinPoint = new GameObject("CoinPoint");
            coinPoint.transform.position = coin.transform.position;
            CoinPoints.Value.Add(coinPoint);
        }

        return Status.Running;
    }
}
