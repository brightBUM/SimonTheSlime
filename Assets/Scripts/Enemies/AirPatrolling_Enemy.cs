using System;
using UnityEngine;

namespace magar
{
    public class AirPatrollingEnemy : Enemy
    {
        [Header("Patrol Settings")]
        public Transform[] patrolTransforms; // Transforms to initialize positions
        public float patrolSpeed = 2f;
        public float waitDuration = 1f;
        public bool shouldWait = true;

        [Header("Detection Settings")]
        public float detectionRadius = 5f;
        public LayerMask targetLayer;

        [Header("Combat Settings")]
        public float chaseDelay = 0.3f;

        // Events
        public event Action OnPatrolPointReachedEvent;
        public event Action OnPlayerFoundEvent;

        private Vector2[] patrolPoints; // Actual positions used for movement
        private Rigidbody2D rb;
        private int currentPatrolIndex = 0;
        private bool isWaiting = false;
        private float waitTimer = 0f;
        private bool hasDetectedTarget = false;
        private bool isInChaseDelay = false;
        private float chaseDelayTimer = 0f;
        private bool isAlert = false;

        protected override void Start()
        {
            base.Start();
            rb = GetComponent<Rigidbody2D>();

            // Initialize patrol points from transforms
            if (patrolTransforms != null && patrolTransforms.Length > 0)
            {
                patrolPoints = new Vector2[patrolTransforms.Length];
                for (int i = 0; i < patrolTransforms.Length; i++)
                {
                    if (patrolTransforms[i] != null)
                    {
                        patrolPoints[i] = patrolTransforms[i].position;
                    }
                    else
                    {
                        Debug.LogError($"Patrol transform at index {i} is null!");
                        patrolPoints[i] = transform.position; // Fallback to current position
                    }
                }
            }
            else
            {
                Debug.LogError("No patrol transforms assigned!");
                enabled = false;
            }

            foreach (Transform t in patrolTransforms)
            {
                Destroy(t.gameObject);
            }
        }

        private void FixedUpdate()
        {
            if (hasDetectedTarget)
            {
                if (isInChaseDelay)
                {
                    chaseDelayTimer += Time.fixedDeltaTime;
                    if (chaseDelayTimer >= chaseDelay)
                    {
                        isInChaseDelay = false;
                        chaseDelayTimer = 0f;
                    }
                    return;
                }

                // Stop moving when target detected
                rb.velocity = Vector2.zero;
                return;
            }

            if (isWaiting)
            {
                waitTimer += Time.fixedDeltaTime;
                if (waitTimer >= waitDuration)
                {
                    isWaiting = false;
                    waitTimer = 0f;
                    MoveToNextPoint();
                }
                return;
            }

            CheckForTarget();

            if (!hasDetectedTarget)
            {
                Patrol();
            }
        }

        private void CheckForTarget()
        {
            Collider2D hit = Physics2D.OverlapCircle(
                transform.position,
                detectionRadius,
                targetLayer);

            if (hit != null)
            {
                if (!hasDetectedTarget)
                {
                    isInChaseDelay = true;
                    chaseDelayTimer = 0f;

                    if (!isAlert)
                    {
                        isAlert = true;
                        OnPlayerFoundEvent?.Invoke();
                    }
                }
                hasDetectedTarget = true;
            }
            else
            {
                hasDetectedTarget = false;
                isInChaseDelay = false;
                isAlert = false;
            }
        }

        private void Patrol()
        {
            Vector2 targetPosition = patrolPoints[currentPatrolIndex];
            Vector2 currentPosition = rb.position;

            Vector2 moveDirection = (targetPosition - currentPosition).normalized;
            Vector2 newPosition = currentPosition + moveDirection * patrolSpeed * Time.fixedDeltaTime;

            rb.MovePosition(newPosition);

            // Check if reached target position
            if (Vector2.Distance(newPosition, targetPosition) < 0.1f)
            {
                OnPatrolPointReachedEvent?.Invoke();

                if (shouldWait)
                {
                    isWaiting = true;
                }
                else
                {
                    MoveToNextPoint();
                }
            }
        }

        private void MoveToNextPoint()
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }

        private void OnDrawGizmosSelected()
        {
            // Draw detection radius
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);

            // Draw patrol path
            if (patrolTransforms != null && patrolTransforms.Length > 0)
            {
                Gizmos.color = Color.cyan;
                for (int i = 0; i < patrolTransforms.Length; i++)
                {
                    if (patrolTransforms[i] != null)
                    {
                        Gizmos.DrawSphere(patrolTransforms[i].position, 0.2f);
                        int nextIndex = (i + 1) % patrolTransforms.Length;
                        if (patrolTransforms[nextIndex] != null)
                        {
                            Gizmos.DrawLine(patrolTransforms[i].position, patrolTransforms[nextIndex].position);
                        }
                    }
                }
            }
        }
    }
}