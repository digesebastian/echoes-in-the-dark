using System.Collections.Generic;
using Unity.Behavior;
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

        private CircleCollider2D _noiseCollider;
        private readonly HashSet<BehaviorGraphAgent> _alertedGuards = new();

        private Vector2 _target;
        private float _velocity;

        private bool _hasHit;
        private bool _isThrown;
        private float _decayTimeLeft;
        
        private void Awake()
        {
            _noiseCollider = GetComponentInChildren<CircleCollider2D>(true);

            if (_noiseCollider != null)
            {
                _noiseCollider.isTrigger = true;
                _noiseCollider.enabled = false;
            }
        }

        public override void Throw(Vector2 target, float velocity)
        {
            base.Throw(target, velocity);

            _target = target;
            _velocity = velocity;
            _isThrown = true;
            _hasHit = false;
            _alertedGuards.Clear();
            _decayTimeLeft = decayTimeTotal;

            spotlight.pointLightOuterRadius = spotlightMinRadius;
            if (_noiseCollider != null)
            {
                _noiseCollider.radius = spotlightMinRadius;
                _noiseCollider.enabled = false;
            }

            Vector2 moveDirection = (_target - rb.position).normalized;
            rb.linearVelocity = moveDirection * _velocity;
        }

        private void FixedUpdate()
        {
            if (!_isThrown) return;

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
            if (_decayTimeLeft > (decayTimeTotal / 2f))
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

            if (_noiseCollider != null)
            {
                _noiseCollider.radius = Mathf.Max(0f, spotlight.pointLightOuterRadius);
            }

            _decayTimeLeft -= Time.fixedDeltaTime;

            if (_decayTimeLeft <= 0f)
            {
                Destroy(gameObject);
            }
        }

        public override void Hit()
        {
            if (_hasHit) return;

            _hasHit = true;
            if (_noiseCollider != null)
            {
                _noiseCollider.enabled = true;
            }

            Decay();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_hasHit && other.CompareTag("Wall"))
            {
                rb.linearVelocity = Vector2.zero;
                Hit();
                return;
            }

            TryAttractGuard(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            TryAttractGuard(other);
        }

        private void TryAttractGuard(Collider2D other)
        {
            if (!_hasHit) return;
            
            if (other is not CircleCollider2D) return;

            BehaviorGraphAgent guardAgent = other.GetComponentInParent<BehaviorGraphAgent>();
            if (guardAgent == null || !_alertedGuards.Add(guardAgent)) return;
            
            // Ignore coins if the guard is already chasing the Player
            if (guardAgent.BlackboardReference.GetVariableValue("IsPlayerInRadius", out bool isPlayerInRadius) && isPlayerInRadius)
                return;

            if (guardAgent.BlackboardReference.GetVariableValue("coin_points", out List<GameObject> coinPoints))
            {
                GameObject coinPoint = new GameObject("CoinPoint");
                coinPoint.transform.position = transform.position;

                coinPoints.Add(coinPoint);
                guardAgent.BlackboardReference.SetVariableValue("coin_points", coinPoints);
            }
        }
    }
}