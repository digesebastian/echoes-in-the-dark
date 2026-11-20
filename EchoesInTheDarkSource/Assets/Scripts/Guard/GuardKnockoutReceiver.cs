using Projectiles;
using Unity.Behavior;
using UnityEngine;

public class GuardKnockoutReceiver : MonoBehaviour
{
    [SerializeField] private BehaviorGraphAgent behaviorAgent;
    private int _projectileLayer;
    
    private void Awake()
    {
        if (behaviorAgent == null)
            behaviorAgent = GetComponent<BehaviorGraphAgent>();

        _projectileLayer = LayerMask.NameToLayer("Projectile");
    }
    
    private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer != _projectileLayer)
                return;
            
            CoinProjectile coin = other.GetComponentInParent<CoinProjectile>();
            if (coin == null || coin.HasLanded)
                return;
    
            behaviorAgent.SetVariableValue("IsKnockedOut", true);
            Destroy(other.attachedRigidbody.gameObject);
        }
}
