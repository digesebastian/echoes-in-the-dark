using UnityEngine;

namespace Projectiles
{
    public class CoinProjectile : ProjectileBase
    {
        private Transform _target;
        private float _velocity;

        // Update is called once per frame
        void Update()
        {
            Vector3 moveDirection = (_target.position - transform.position).normalized;
            transform.position += _velocity * Time.deltaTime * moveDirection;

            if (Vector3.Distance(transform.position, _target.position) < 0.1f)
            {
                Hit();
            }
        }

        public override void Throw(Transform target, float velocity)
        {
            _target = target;
            _velocity = velocity;
        }

        public override void Hit()
        {
            Destroy(gameObject);
        }
    }
}
