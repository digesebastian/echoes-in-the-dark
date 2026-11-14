using UnityEngine;

namespace Projectiles
{
    public abstract class ProjectileBase : MonoBehaviour
    {
        protected Rigidbody2D rb;

        public virtual void Throw(Vector2 target, float velocity)
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0;
        }

        public abstract void Hit();
    }
}
