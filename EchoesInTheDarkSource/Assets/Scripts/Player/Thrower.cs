using GridBasedMovement;
using Projectiles;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class Thrower : MonoBehaviour
    {
        [SerializeField] private ProjectileBase projectilePrefab;
        [SerializeField] private float throwForce = 10f;
    
        private PlayerState _playerState;

        private void Awake()
        {
            _playerState = GetComponent<PlayerState>();
        }

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
            Vector2 target = (Vector2)transform.position + _playerState.GetFacingDirection() * throwForce; 
        
            ProjectileBase projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            projectile.Throw(target, 10f);
        }
    }
}
