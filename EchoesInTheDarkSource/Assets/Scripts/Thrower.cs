using UnityEngine;
using UnityEngine.InputSystem;
using Projectiles;

public class Thrower : MonoBehaviour
{
    [SerializeField] private ProjectileBase projectilePrefab;
    [SerializeField] private Transform target;

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            ThrowObject();
        }
    }

    private void ThrowObject()
    {
        ProjectileBase projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.Throw(target, 10f);
    }
}
