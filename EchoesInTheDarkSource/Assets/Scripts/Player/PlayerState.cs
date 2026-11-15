using UnityEngine;

namespace Player
{
    public class PlayerState : MonoBehaviour
    {
        private Vector2 _facingDirection = Vector2.down;

        public Vector2 GetFacingDirection()
        {
            return _facingDirection;
        }
        
        public void SetFacingDirection(Vector2 newDirection)
        {
            _facingDirection = newDirection.normalized;
        }
    }
}
