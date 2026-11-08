using UnityEngine;

namespace Projectiles
{
    public class CoinProjectile : ProjectileBase
    {
        private Vector2 _target;
        private float _velocity;

        // Update is called once per frame
        void Update()
        {
            Vector3 moveDirection = (_target - (Vector2)transform.position).normalized;
            transform.position += _velocity * Time.deltaTime * moveDirection;

            if (Vector3.Distance(transform.position, _target) < 0.1f)
            {
                Hit();
            }
        }

        public override void Throw(Vector2 target, float velocity)
        {
            _target = target;
            _velocity = velocity;
        }

        public override void Hit()
        {
            //Destroy(gameObject);
        }
    }
}
