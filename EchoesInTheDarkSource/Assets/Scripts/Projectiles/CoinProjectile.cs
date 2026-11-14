using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Projectiles
{
    public class CoinProjectile : ProjectileBase
    {
        [SerializeField] private float decayTimeTotal = 2.0f;
        [SerializeField] private Light2D spotlight;
        [SerializeField] private float spotlightMinRadius = 0.1f;
        [SerializeField] private float spotlightMaxRadius = 5f;
        [SerializeField] private float spotLightRadiusDelta = 0.02f;
        
        private Vector2 _target;
        private float _velocity;
        
        private bool _hasHit = false; 
        private bool _isThrown = false;

        private float _decayTimeLeft;
        
        // ReSharper disable Unity.PerformanceAnalysis
        public override void Throw(Vector2 target, float velocity)
        {
            base.Throw(target, velocity);
            
            _target = target;
            _velocity = velocity;
            _isThrown = true;
            _decayTimeLeft = decayTimeTotal;
            spotlight.pointLightOuterRadius = spotlightMinRadius;
            
            Vector3 moveDirection = (_target - rb.position).normalized;
            rb.linearVelocity = moveDirection * _velocity;
        }
        
        // Update is called once per frame
        private void FixedUpdate()
        {
            if (!_isThrown) return;

            if (_hasHit)
            {
                Decay();
                return;
            }

            if (Vector3.Distance(rb.position, _target) < 0.1f)
            {
                rb.position = _target;
                rb.linearVelocity = Vector2.zero;
                Hit();
            }
        }

        private void Decay()
        {
            if (_decayTimeLeft > (decayTimeTotal / 2))
            {
                if (spotlight.pointLightOuterRadius <= spotlightMaxRadius)
                { 
                    spotlight.pointLightOuterRadius += spotLightRadiusDelta;
                }
            }
            else
            {
                if (spotlight.pointLightOuterRadius >= 0f)
                { 
                    spotlight.pointLightOuterRadius -= spotLightRadiusDelta;
                }
            }
            _decayTimeLeft -= Time.fixedDeltaTime;

            if (_decayTimeLeft <= 0)
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
            if (other.CompareTag("Wall"))
            {
                rb.linearVelocity = Vector2.zero;
                Hit();
            }
        }
    }
}
