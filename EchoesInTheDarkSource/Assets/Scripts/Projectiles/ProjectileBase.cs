using UnityEngine;

namespace Projectiles
{
    public abstract class ProjectileBase : MonoBehaviour
    {
        public abstract void Throw(Transform target, float velocity);

        public abstract void Hit();
    }
}
