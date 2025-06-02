using System;
using System.Collections;
using UnityEngine;

namespace magar
{
    public class HorizontalMovingEnemy : Enemy
    {
        [Header("Horizontal patroller")]
        public Transform TransformA;  // Left position reference
        public Transform TransformB; // Right position reference
        public float patrolSpeed = 2f;
        public float waitDuration = 1f;
        public bool shouldWait = true;
        public LayerMask targetLayer; // Layer to detect player collision

        private Vector2 pointA, pointB;
        private Rigidbody2D rb;
        private bool isMovingLeft = true;
        private bool isWaiting = false;
        private float waitTimer = 0f;

        public event Action OnMovePointReachedEvent;
        protected override void Start()
        {
            base.Start();

            pointA = new Vector2(TransformA.position.x, transform.position.y);
            pointB = new Vector2(TransformB.position.x, transform.position.y);

            rb = GetComponent<Rigidbody2D>();
            isMovingLeft = true;

            Destroy(TransformA.gameObject);
            Destroy(TransformB.gameObject);
        }

        private void FixedUpdate()
        {
            if (isWaiting)
            {
                waitTimer += Time.fixedDeltaTime;
                if (waitTimer >= waitDuration)
                {
                    isWaiting = false;
                    waitTimer = 0f;
                    isMovingLeft = !isMovingLeft;
                }
                return;
            }

            Patrol();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // Check if collided object is on target layer (player)
            if (((1 << collision.gameObject.layer) & targetLayer) != 0)
            {
                //BlastItself();
                //give damage
            }
        }

        private void Patrol()
        {
            Vector2 targetPosition = isMovingLeft ? pointA : pointB;
            Vector2 currentPosition = rb.position;

            Vector2 moveDirection = (targetPosition - currentPosition).normalized;
            Vector2 newPosition = currentPosition + moveDirection * patrolSpeed * Time.fixedDeltaTime;

            rb.MovePosition(newPosition);

            // Flip sprite based on direction
            if ((isMovingLeft && newPosition.x < currentPosition.x) ||
                (!isMovingLeft && newPosition.x > currentPosition.x))
            {
                Vector3 scale = transform.localScale;
                scale.x = isMovingLeft ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
                transform.localScale = scale;
            }

            // Check if reached target position
            if (Vector2.Distance(newPosition, targetPosition) < 0.1f)
            {
                if (shouldWait)
                {
                    isWaiting = true;
                }
                else
                {
                    isMovingLeft = !isMovingLeft;
                }
                OnMovePointReachedEvent?.Invoke();
            }
        }
    }
}