using System;
using UnityEngine;

namespace GridBasedMovement
{
    public class MovementController : MonoBehaviour
    {
        [SerializeField] private bool moveDiagonally = false;
        [SerializeField] private float speed = 5f;
        [SerializeField] private LayerMask obstacle;

        private Animator _animator;

        private static readonly int HorizontalMovement = Animator.StringToHash("HorizontalMovement");
        private static readonly int VerticalMovement = Animator.StringToHash("VerticalMovement");
        private static readonly int IsMoving = Animator.StringToHash("IsMoving");
        private static readonly int PreviousHorizontalMovement = Animator.StringToHash("PreviousHorizontalMovement");
        private static readonly int PreviousVerticalMovement = Animator.StringToHash("PreviousVerticalMovement");
        
        private float _horizontalMovement;
        private float _verticalMovement;
        private Vector2 _previousMovement;

        private void Awake()
        {
            _animator = GetComponent<Animator>();

            print(obstacle.value);
        }

        private void Update()
        {
                _horizontalMovement = Input.GetAxisRaw("Horizontal");
                
                if (!moveDiagonally && (_horizontalMovement > 0))
                {
                    _verticalMovement = 0; 
                }
                else
                {
                    _verticalMovement = Input.GetAxisRaw("Vertical");
                }

                Vector3 movementInput = new Vector3(_horizontalMovement, _verticalMovement);
                UpdateAnimator(movementInput);
                
                if (movementInput != Vector3.zero)
                {
                    Vector3 nextPosition = transform.position + movementInput.normalized * speed * Time.deltaTime;
                    if (!Physics2D.OverlapCircle(nextPosition, .6f, obstacle))
                    {
                        transform.position = nextPosition;
                    }
                }              
        }
        private void UpdateAnimator(Vector2 direction)
        {
            bool isMoving = direction != Vector2.zero;
            _animator.SetBool(IsMoving, isMoving);

            if (isMoving)
            {
                // When actively walking, update the walk animation parameters
                _animator.SetFloat(HorizontalMovement, direction.x);
                _animator.SetFloat(VerticalMovement, direction.y);
                
                // Continuously overwrite our memory with this active direction
                _previousMovement = direction;
            }
            else
            {
                // When stopped, feed the saved memory into the idle animation parameters
                _animator.SetFloat(PreviousHorizontalMovement, _previousMovement.x);
                _animator.SetFloat(PreviousVerticalMovement, _previousMovement.y);
            }
        } 
    }
}
