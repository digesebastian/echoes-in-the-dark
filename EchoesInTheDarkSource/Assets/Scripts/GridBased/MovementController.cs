using Player;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem;

namespace GridBasedMovement
{
    public class MovementController : MonoBehaviour
    {
        private enum SpeedState
        {
            Idle,
            Walking,
            Sprinting,
            Sneaking            
        }

        Rigidbody2D rb;

        private PlayerState _playerState;
        
        public InputAction moveAction;
        
        [SerializeField] private float sneakSpeed = 1f;
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float sprintSpeed = 8f;
        
        [SerializeField] private float noiseMin = 1f;
        [SerializeField] private float walkingNoiseMax = 10f;
        [SerializeField] private float sprintingNoiseMax = 15f;
        [SerializeField] private float sneakingNoiseMax = 5f;
        
        [SerializeField] private float noiseDecayRate = 0.1f;
        [SerializeField] private float walkingNoiseDelta = 0.1f;
        [SerializeField] private float sprintingNoiseDelta = 0.2f;
        [SerializeField] private float sneakingNoiseDelta = 0.04f;
        [SerializeField] private Light2D spotlight;

        private Animator _animator;

        private static readonly int HorizontalMovement = Animator.StringToHash("HorizontalMovement");
        private static readonly int VerticalMovement = Animator.StringToHash("VerticalMovement");
        private static readonly int IsMoving = Animator.StringToHash("IsMoving");
        private static readonly int IsSprinting = Animator.StringToHash("IsSprinting");
        private static readonly int PreviousHorizontalMovement = Animator.StringToHash("PreviousHorizontalMovement");
        private static readonly int PreviousVerticalMovement = Animator.StringToHash("PreviousVerticalMovement");
        
        private float _horizontalMovement;
        private float _verticalMovement;
        private float _noiseEmitted;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            
            _animator = GetComponent<Animator>();
            _noiseEmitted = noiseMin;

            _playerState = GetComponent<PlayerState>();
            
            moveAction.Enable();
        }

        private void Update()
        {
            Vector2 movementInput = moveAction.ReadValue<Vector2>();
            bool isMoving =  movementInput != Vector2.zero;
            bool isSprintingInput = Keyboard.current.shiftKey.isPressed;
            bool isSneakingInput = !isSprintingInput && Keyboard.current.spaceKey.isPressed;

            SpeedState currentSpeedState = GetSpeedState(isMoving, isSprintingInput, isSneakingInput);

            UpdateAnimator(movementInput, currentSpeedState);
            UpdateNoiseEmitted(currentSpeedState);
            UpdateSpotlight(); 
                
            float currentSpeed = GetSpeed(currentSpeedState);
                
            rb.linearVelocity = currentSpeed * movementInput.normalized;
        }

        private void UpdateAnimator(Vector2 direction, SpeedState speedState)
        {
            bool isMoving = speedState != SpeedState.Idle;
            _animator.SetBool(IsMoving, isMoving);
            _animator.SetBool(IsSprinting, speedState == SpeedState.Sprinting);

            if (isMoving)
            {
                _animator.SetFloat(HorizontalMovement, direction.x);
                _animator.SetFloat(VerticalMovement, direction.y);
                
                _playerState.SetFacingDirection(direction);
            }
            else
            {
                _animator.SetFloat(PreviousHorizontalMovement, _playerState.GetFacingDirection().x);
                _animator.SetFloat(PreviousVerticalMovement, _playerState.GetFacingDirection().y);
            }
        }

        private void UpdateNoiseEmitted(SpeedState speedState)
        {
            var currentNoise = _noiseEmitted;
            
            // find the maximum noise emittable at this speed state
            var noiseMax = speedState switch
            {
                SpeedState.Sprinting => sprintingNoiseMax,
                SpeedState.Sneaking => sneakingNoiseMax,
                SpeedState.Walking => walkingNoiseMax,
                _ => 1f
            };

            if (currentNoise > noiseMax)
            {
                // if the current noise is higher than the max for the speed state decrease it
                currentNoise -= noiseDecayRate;
                _noiseEmitted = Mathf.Max(noiseMin, currentNoise);
            }
            else
            {
                // if not increase the noise according to the speed state
                currentNoise += speedState switch
                {
                    SpeedState.Walking => walkingNoiseDelta,
                    SpeedState.Sprinting => sprintingNoiseDelta,
                    SpeedState.Sneaking => sneakingNoiseDelta,
                    _ => 0f
                };
                
                _noiseEmitted = Mathf.Min(currentNoise, noiseMax);
            }
        }

        private void UpdateSpotlight()
        {
            spotlight.pointLightOuterRadius = _noiseEmitted;
        }

        private SpeedState GetSpeedState(bool isMoving, bool isSprinting, bool isSneaking)
        {
            if (!isMoving)
            {
                return SpeedState.Idle;
            }
            if (isSprinting)
            {
                return SpeedState.Sprinting;
            }
            return isSneaking ? SpeedState.Sneaking : SpeedState.Walking;
        }
        private float GetSpeed(SpeedState state)
        {
            return state switch
            {
                SpeedState.Walking => walkSpeed,
                SpeedState.Sprinting => sprintSpeed,
                SpeedState.Sneaking => sneakSpeed,
                _ => 0f
            };
        }
    }
}
