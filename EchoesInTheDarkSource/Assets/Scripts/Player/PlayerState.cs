using UnityEngine;

namespace Player
{
    public class PlayerState : MonoBehaviour
    {
        private Vector2 facingDirection = Vector2.down;

        public Vector2 GetFacingDirection()
        {
            return facingDirection;
        }
        
        public void SetFacingDirection(Vector2 newDirection)
        {
            facingDirection = newDirection.normalized;
        }
    }
}
