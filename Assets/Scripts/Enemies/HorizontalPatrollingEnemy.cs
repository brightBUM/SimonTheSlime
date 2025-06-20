using System;
using System.Collections;
using UnityEngine;

namespace magar
{
    public class HorizontalPatrollingEnemy : Enemy
    {
        [Header("Horizontal patroller")]
        public Transform TransformA;  // Left position reference
        public Transform TransformB; // Right position reference
        public float patrolSpeed = 2f;
        public float chaseSpeed = 5f;
        public float waitDuration = 1f;
        public bool shouldWait = true;
        public LayerMask targetLayer; // Layer to detect for player
        public float blastRange = 1f; // Distance to trigger explosion
        public float chaseDelay = 0.3f; // Delay before chasing
        public float blastDelay = 1f;

        private Vector2 pointA, pointB;
        private Rigidbody2D rb;
        private bool isMovingLeft = true;
        private bool isWaiting = false;
        private float waitTimer = 0f;
        private bool hasDetectedTarget = false;
        private Vector2? targetHitPoint; // Nullable to track if we have a target
        private float chaseDelayTimer = 0f;
        private bool isInChaseDelay = false;
        private bool isAlert = false;

        public event Action OnMovePointReachedEvent;
        public event Action OnPlayerFoundEvent;
        public event Action OnBlastInitiatedEvent;
        protected override void Start()
        {
            base.Start();

            pointA = new Vector2(TransformA.position.x, transform.position.y);
            pointB = new Vector2(TransformB.position.x, transform.position.y);

            rb = GetComponent<Rigidbody2D>();
            isMovingLeft = true;
            targetHitPoint = null;
            isInChaseDelay = false;

            //Destroy(TransformA.gameObject);
           // Destroy(TransformB.gameObject);
        }
        private void FixedUpdate()
        {
            if (hasDetectedTarget && targetHitPoint.HasValue)
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

                ChaseTarget();
                //return;
            }

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

            CheckForTarget();

            if (!hasDetectedTarget)
            {
                Patrol();
            }
        }

        private void CheckForTarget()
        {
            Vector2 currentPosition = transform.position;
            Vector2 patrolTarget = isMovingLeft ? pointA : pointB;
            Vector2 direction = (patrolTarget - currentPosition).normalized;
            float distance = Vector2.Distance(currentPosition, patrolTarget);

            RaycastHit2D hit = Physics2D.Raycast(
                currentPosition,
                direction,
                distance,
                targetLayer);

            if (hit.collider != null)
            {
                Debug.DrawLine(currentPosition, patrolTarget, Color.red);
                if (!hasDetectedTarget)
                {
                    // Only set delay when first detecting target
                    isInChaseDelay = true;
                    chaseDelayTimer = 0f;

                    if (!isAlert)
                    {
                        isAlert = true;
                        OnPlayerFoundEvent?.Invoke();
                    }
                }
                hasDetectedTarget = true;
                targetHitPoint = hit.point;
            }
            else
            {
                Debug.DrawLine(currentPosition, patrolTarget, Color.green);
                hasDetectedTarget = false;
                targetHitPoint = null;
                isInChaseDelay = false;
            }
        }

        private void ChaseTarget()
        {
            if (!targetHitPoint.HasValue) return;


            Vector2 currentPosition = rb.position;
            Vector2 direction = (targetHitPoint.Value - currentPosition).normalized;
            Vector2 newPosition = currentPosition + direction * chaseSpeed * Time.fixedDeltaTime;

            rb.MovePosition(newPosition);

            // Flip sprite based on movement direction
            Vector3 scale = transform.localScale;
            scale.x = direction.x < 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
            transform.localScale = scale;

            // Check if reached blast range
            if (Vector2.Distance(currentPosition, targetHitPoint.Value) <= blastRange)
            {
                BlastItself();
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

        private void BlastItself()
        {
           
            rb.velocity = Vector2.zero;
            rb.simulated = false;
            OnBlastInitiatedEvent?.Invoke();    
            StartCoroutine(DamageCoroutine());

            this.enabled = false;
        }
        IEnumerator DamageCoroutine()
        {
            yield return new WaitForSeconds(blastDelay);
            DamageAround();
            TakeDamage(new DamageInfo(startingHealth+1, Vector3.zero));
        }
        private void DamageAround()
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(
                transform.position,
                blastRange,
                targetLayer);

            foreach (Collider2D hitCollider in hitColliders)
            {
                LivingEntity damageable = hitCollider.GetComponent<LivingEntity>();
                if (damageable != null)
                {
                    damageable.TakeDamage(new DamageInfo(1, (transform.position - damageable.transform.position).normalized));
                }
            }
            Debug.DrawLine(transform.position, transform.position + Vector3.right * blastRange, Color.red, 1f);
            Debug.DrawLine(transform.position, transform.position + Vector3.left * blastRange, Color.red, 1f);
            Debug.DrawLine(transform.position, transform.position + Vector3.up * blastRange, Color.red, 1f);
            Debug.DrawLine(transform.position, transform.position + Vector3.down * blastRange, Color.red, 1f);
        }
    }
}