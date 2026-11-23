using UnityEngine;
using Spotlights;

namespace Projectiles
{
    public class CoinProjectile : ProjectileBase
    {
        [SerializeField] private ProjectileSpotlight spotlight;
        
        [SerializeField] private float decayTimeTotal = 2.0f;
        [SerializeField] private float spotlightMinRadius = 0.1f;
        [SerializeField] private float spotlightMaxRadius = 5f;
        [SerializeField] private float spotLightRadiusDelta = 0.02f;

        private Vector2 _target;
        private float _velocity;
        private bool _hasHit;
        public bool HasHit => _hasHit;
        
        private float _decayTimeLeft;

        // Initializes the coin and throws it
        public override void Throw(Vector2 target, float velocity)
        {
            base.Throw(target, velocity);

            _target = target;
            _velocity = velocity;
            _hasHit = false;
            _decayTimeLeft = decayTimeTotal;

            spotlight.SetRadius(spotlightMinRadius);

            Vector2 moveDirection = (_target - rb.position).normalized;
            rb.linearVelocity = moveDirection * _velocity;
        }

        private void FixedUpdate()
        {
            if (_hasHit)
            {
                Decay();
                return;
            }

            if (Vector2.Distance(rb.position, _target) < 0.1f)
            {
                rb.position = _target;
                rb.linearVelocity = Vector2.zero;
                Hit();
            }
        }

        private void Decay()
        {
            float currentRadius = spotlight.GetRadius();
            if (_decayTimeLeft > (decayTimeTotal / 2f))
            {
                if (currentRadius <= spotlightMaxRadius)
                {
                    spotlight.SetRadius(currentRadius + spotLightRadiusDelta);
                }
            }
            else
            {
                if (currentRadius >= 0f)
                {
                    spotlight.SetRadius(currentRadius - spotLightRadiusDelta);
                }
            }
            
            _decayTimeLeft -= Time.fixedDeltaTime;

            if (_decayTimeLeft <= 0f)
            {
                Destroy(gameObject);
            }
        }

        public override void Hit()
        {
            _hasHit = true;

            Decay();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_hasHit && other.CompareTag("Wall"))
            {
                rb.linearVelocity = Vector2.zero;
                Hit();
            }
        }
    }
}