using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace POLIMIGameCollective
{
    public class CharacterController : MonoBehaviour
    {
        [Header("Movement Settings")] [SerializeField]
        private float characterSpeed = 5;

        [SerializeField] private float movementThreshold = 0.01f;

        [Space(10)] [Tooltip("If false the character can move diagonally.")] [SerializeField]
        private bool fourDirectionMovement = true;

        private Vector2 _movement;
        private Vector2 _previousMovement;

        private Transform _tr;


        private void Awake()
        {
            _tr = GetComponent<Transform>();
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
            
            float ds = characterSpeed * Time.deltaTime;
            _tr.position += (Vector3) (ds * _movement);
        }
    }
}

