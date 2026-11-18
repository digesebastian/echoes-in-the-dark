using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Spotlights
{
    public abstract class SpotlightBase : MonoBehaviour
    {
        [SerializeField] private float initialRadius = 5.0f;
        [SerializeField] private float shadowFade = 1.5f;
        
        private Rigidbody2D _rb;
        protected Light2D Spotlight;
        protected CircleCollider2D SpotlightCollider;
        
        protected virtual void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.bodyType = RigidbodyType2D.Kinematic; // ensure the body is kinematic
            
            Spotlight = GetComponentInChildren<Light2D>();
            SpotlightCollider = GetComponent<CircleCollider2D>();
            SetRadius(initialRadius);
        }

        // Implementing members must handle collisions with the spotlight
        // At the least this method must be an empty method, since this means the spotlight handles it's own collisions
        public abstract void OnTriggerEnter2D(Collider2D other);

        public float GetRadius()
        {
            return Spotlight.pointLightOuterRadius;
        }
        
        public void SetRadius(float radius)
        {
            Spotlight.pointLightOuterRadius = radius;
            SpotlightCollider.radius = Spotlight.pointLightOuterRadius - shadowFade;
        }
    }
}
