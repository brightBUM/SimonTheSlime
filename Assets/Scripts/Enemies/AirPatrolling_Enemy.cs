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
        public LayerMask obstacleLayer; // Layers that block vision

        [Header("Combat Settings")]
        public float chaseDelay = 0.3f;
        public float shootInterval = 1f;
        private float lastShootTime = 0f;
        public GameObject projectilePrefab;
        // Events
        public event Action OnPatrolPointReachedEvent;
        public event Action OnPlayerFoundEvent;
        public event Action OnShootProjectileEvent;

        private Vector2[] patrolPoints; // Actual positions used for movement
        private Rigidbody2D rb;
        private int currentPatrolIndex = 0;
        private bool isWaiting = false;
        private float waitTimer = 0f;
        private bool hasDetectedTarget = false;
        private bool isInChaseDelay = false;
        private float chaseDelayTimer = 0f;
        private bool isAlert = false;
        private Transform currentTarget; // Store the detected target

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
            if (hasDetectedTarget && currentTarget != null)
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

                // Check if target is still visible
                if (IsTargetVisible(currentTarget))
                {
                    // Stop moving when target detected and visible
                    rb.velocity = Vector2.zero;

                    // Handle shooting
                    if (Time.time > lastShootTime + shootInterval)
                    {
                        ShootProjectile();
                        lastShootTime = Time.time;
                    }
                }
                else
                {
                    // Target not visible, resume patrol
                    hasDetectedTarget = false;
                    isAlert = false;
                }
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
                // Check if target is visible with raycast
                if (IsTargetVisible(hit.transform))
                {
                    if (!hasDetectedTarget)
                    {
                        isInChaseDelay = true;
                        chaseDelayTimer = 0f;
                        currentTarget = hit.transform;

                        if (!isAlert)
                        {
                            isAlert = true;
                            OnPlayerFoundEvent?.Invoke();
                        }
                    }
                    hasDetectedTarget = true;
                    return;
                }
            }

            // No valid target found
            hasDetectedTarget = false;
            isInChaseDelay = false;
            isAlert = false;
            currentTarget = null;
        }

        private bool IsTargetVisible(Transform target)
        {
            Vector2 direction = target.position - transform.position;
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                direction,
                detectionRadius,
                obstacleLayer | targetLayer);

            // If we hit something and it's our target, then it's visible
            if (hit.collider != null && hit.transform == target)
            {
                Debug.DrawRay(transform.position, direction, Color.green, 0.1f);
                return true;
            }

            Debug.DrawRay(transform.position, direction, Color.red, 0.1f);
            return false;
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

        private void ShootProjectile()
        {
            if (currentTarget == null || projectilePrefab == null) return;

            // Calculate direction to target
            Vector2 shootDirection = (currentTarget.position - transform.position).normalized;

            // Instantiate projectile
            GameObject projectile = Instantiate(projectilePrefab, transform.position+((Vector3)shootDirection.normalized*2f), Quaternion.identity);

            // Initialize projectile
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.Init(shootDirection);
            }
            Debug.DrawLine(transform.position, currentTarget.position, Color.magenta, 0.5f);
            OnShootProjectileEvent?.Invoke();
        }

        private void OnDrawGizmosSelected()
        {
            // Draw detection radius
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);

            // Draw patrol path
            if (patrolPoints != null && patrolPoints.Length > 0)
            {
                Gizmos.color = Color.cyan;
                for (int i = 0; i < patrolPoints.Length; i++)
                {
                    Gizmos.DrawSphere(patrolPoints[i], 0.2f);
                    int nextIndex = (i + 1) % patrolPoints.Length;
                    Gizmos.DrawLine(patrolPoints[i], patrolPoints[nextIndex]);
                }
            }
        }
    }
}