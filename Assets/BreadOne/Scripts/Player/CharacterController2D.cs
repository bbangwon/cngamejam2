using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace cngamejam{

    public class CharacterController2D : MonoBehaviour
    {
        [SerializeField]
        float maxSpeed = 3.4f;

        [SerializeField]
        float jumpHeight = 6.5f;

        float moveVector;

        bool isGrounded = false;

        private Rigidbody2D rigidbody2D;
        private Collider2D collider2D;

        public bool Interactable = true;

        public float MoveDirection { get; private set; }

        private void Awake()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();
            collider2D = GetComponent<Collider2D>();
        }

        void CheckGround()
        {
            isGrounded = false;

            Bounds colliderBounds = collider2D.bounds;
            Vector3 groundCheckPos = colliderBounds.min + new Vector3(colliderBounds.size.x * 0.5f, 0f, 0f);

            Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckPos, 0.2f);

            if (colliders.Count(c => c != collider2D) > 0)
                isGrounded = true;
        }

        private void FixedUpdate()
        {
            if (!Interactable)
                return;

            CheckGround();

            rigidbody2D.velocity = new Vector2(moveVector * maxSpeed, rigidbody2D.velocity.y);

            if(Input.GetButton("Jump") && isGrounded)
            {
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpHeight);
            }
        }

        // Update is called once per frame
        void Update()
        {
            moveVector = Input.GetAxis("Horizontal");

            if(moveVector < 0f)
            {
                MoveDirection = -1;
            }
            else if(moveVector > 0f)
            {
                MoveDirection = 1f;
            }

        }
    }

}