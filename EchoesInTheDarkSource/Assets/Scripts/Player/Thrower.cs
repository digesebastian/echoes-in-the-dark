using Projectiles;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class Thrower : MonoBehaviour
    {
        [SerializeField] private ProjectileBase projectilePrefab;
        [SerializeField] private float throwForceMax = 10f;
        [SerializeField] private GameObject chargeBar;
        [SerializeField] private Sprite[] chargeBarSprites;
        [SerializeField] private float maxChargeTime = 1f;

        private SpriteRenderer _chargeBarRenderer;
        private PlayerState _playerState;
        private float _throwForce = 0;
        private float _throwKeyHeldTime = 0;

        private void Awake()
        {
            _playerState = GetComponent<PlayerState>();
            _chargeBarRenderer = chargeBar.GetComponent<SpriteRenderer>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                chargeBar.SetActive(true);
                _throwForce = 0;
                _throwKeyHeldTime = 0;
            }

            if (Keyboard.current.enterKey.isPressed)
            {
                // cycle charge bar state
                float currentCharge = Mathf.PingPong(_throwKeyHeldTime, maxChargeTime);
                float chargePercent = currentCharge / maxChargeTime;
                _throwForce = chargePercent;
                int frameIndex = Mathf.RoundToInt(chargePercent * (chargeBarSprites.Length - 1));
                _chargeBarRenderer.sprite = chargeBarSprites[frameIndex];
                
                // Update charge bar sprite
                _throwKeyHeldTime += Time.deltaTime;
            }

            if (Keyboard.current.enterKey.wasReleasedThisFrame)
            {
                chargeBar.SetActive(false);
                ThrowObject(_throwForce * throwForceMax);
            }
        }

        private void ThrowObject(float throwForce)
        {
            Vector2 target = (Vector2)transform.position +
                             _playerState.GetFacingDirection() * throwForce;

            ProjectileBase projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            projectile.Throw(target, 10f);
        }
    }
}