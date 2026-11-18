using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Spotlights
{
    public abstract class SpotlightBase : MonoBehaviour
    {
        [SerializeField] private float shadowFade = 1.0f;
        
        private Light2D _spotlight;
        private Rigidbody2D _rb;
        protected CircleCollider2D SpotlightCollider;
        
        protected virtual void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _spotlight = GetComponentInChildren<Light2D>();
            SpotlightCollider = GetComponent<CircleCollider2D>();
            _rb.bodyType = RigidbodyType2D.Kinematic; // ensure the body is kinematic
        }

        // Implementing members must handle collisions with the spotlight
        public abstract void OnTriggerEnter2D(Collider2D other);
        
        public void SetRadius(float radius)
        {
            _spotlight.pointLightOuterRadius = radius;
            SpotlightCollider.radius = _spotlight.pointLightOuterRadius - shadowFade;
        }
    }
}
