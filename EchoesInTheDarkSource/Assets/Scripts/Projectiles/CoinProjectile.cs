using UnityEngine;

namespace Projectiles
{
    public class CoinProjectile : ProjectileBase
    {
        [SerializeField] private float decayTime = 2.0f;
        
        private Vector2 _target;
        private float _velocity;
        
        private bool _hasHit = false; 
        private bool _isThrown = false;

        // Update is called once per frame
        void Update()
        {
            if (!_isThrown && _hasHit) { return; }
            
            Vector3 moveDirection = (_target - (Vector2)transform.position).normalized;
            
            transform.position += _velocity * Time.deltaTime * moveDirection;

            if (Vector3.Distance(transform.position, _target) < 0.1f)
            {
                transform.position =  _target;
                Hit();
                _hasHit = true;
            }
        }

        public override void Throw(Vector2 target, float velocity)
        {
            _target = target;
            _velocity = velocity;
            _isThrown = true;
        }

        public override void Hit()
        {
            Destroy(gameObject, decayTime);
        }
    }
}
