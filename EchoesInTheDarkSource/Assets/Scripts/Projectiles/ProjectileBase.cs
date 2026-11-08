using UnityEngine;

namespace Projectiles
{
    public abstract class ProjectileBase : MonoBehaviour
    {
        public abstract void Throw(Vector2 target, float velocity);

        public abstract void Hit();
    }
}
