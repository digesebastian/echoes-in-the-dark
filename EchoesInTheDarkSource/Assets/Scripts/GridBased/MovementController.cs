using System;
using UnityEngine;

namespace GridBasedMovement
{
    public class MovementController : MonoBehaviour
    {
        [SerializeField] private bool moveDiagonally = false;
        [SerializeField] private float sneakSpeed = 1f;
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float sprintSpeed = 8f;
        [SerializeField] private LayerMask obstacle;

        private Animator _animator;

        private static readonly int HorizontalMovement = Animator.StringToHash("HorizontalMovement");
        private static readonly int VerticalMovement = Animator.StringToHash("VerticalMovement");
        private static readonly int IsMoving = Animator.StringToHash("IsMoving");
        private static readonly int IsSprinting = Animator.StringToHash("IsSprinting");
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
                bool isSprintingInput = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                bool isSneakingInput = Input.GetKey(KeyCode.Space);
                float currentSpeed = walkSpeed;
                if (isSprintingInput)
                {
                    currentSpeed = sprintSpeed;
                }
                else if (isSneakingInput)
                {
                    currentSpeed = sneakSpeed;
                }
                
                UpdateAnimator(movementInput, isSprintingInput);
                
                if (movementInput != Vector3.zero)
                {
                    Vector3 nextPosition = transform.position + movementInput.normalized * currentSpeed * Time.deltaTime;
                    if (!Physics2D.OverlapCircle(nextPosition, .6f, obstacle))
                    {
                        transform.position = nextPosition;
                    }
                }              
        }
        private void UpdateAnimator(Vector2 direction, bool isSprintinginput)
        {
            bool isMoving = direction != Vector2.zero;
            _animator.SetBool(IsMoving, isMoving);
            _animator.SetBool(IsSprinting,isMoving && isSprintinginput);

            if (isMoving)
            {
                _animator.SetFloat(HorizontalMovement, direction.x);
                _animator.SetFloat(VerticalMovement, direction.y);
                
                _previousMovement = direction;
            }
            else
            {
                _animator.SetFloat(PreviousHorizontalMovement, _previousMovement.x);
                _animator.SetFloat(PreviousVerticalMovement, _previousMovement.y);
            }
        } 
    }
}
