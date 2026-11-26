using UnityEngine;

namespace Player
{
    public class PlayerState : MonoBehaviour
    {
        private Vector2 _facingDirection = Vector2.down;

        public Vector2 FacingDirection  {
            get => _facingDirection;
            set => _facingDirection = value.normalized; 
        }

        public bool HasClaimedTreasure { get; set; } = false;
    } 
}
