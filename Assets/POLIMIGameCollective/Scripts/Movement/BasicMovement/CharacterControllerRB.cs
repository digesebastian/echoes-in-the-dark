using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace POLIMIGameCollective
{
    public class CharacterControllerRB : MonoBehaviour
    {
        [Header("Movement Settings")] [SerializeField]
        private float characterSpeed = 5;

        [SerializeField] private float movementThreshold = 0.01f;

        [Space(10)] [Tooltip("If false the character can move diagonally.")] [SerializeField]
        private bool fourDirectionMovement = true;

        private Vector2 _movement;
        private Vector2 _previousMovement;

        private Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            _movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            if (fourDirectionMovement)
            {
                if (Math.Abs(_movement.x) > movementThreshold)
                {
                    _movement.y = 0;
                }
            }
        }

        private void FixedUpdate()
        {
            float ds = characterSpeed * Time.fixedDeltaTime;
            _rb.MovePosition(_rb.position + ds * _movement);
        }
    }
}

