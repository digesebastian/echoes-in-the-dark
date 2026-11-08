using System;
using GridBasedMovement;
using UnityEngine;
using UnityEngine.InputSystem;
using Projectiles;

public class Thrower : MonoBehaviour
{
    [SerializeField] private ProjectileBase projectilePrefab;
    [SerializeField] private float throwForce = 10f;
    
    private MovementController _movementController;

    private void Awake()
    {
        _movementController = GetComponent<MovementController>();
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
        Vector2 target = (Vector2)transform.position + _movementController.facingDirection.normalized * throwForce; 
        
        ProjectileBase projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.Throw(target, 10f);
    }
}
