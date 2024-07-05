using System.Collections;
using UnityEngine;

namespace Berzerk
{
    public class Zombie : MonoBehaviour
    {
        private enum State
        {
            Wander,
            Patrolling,
            Chasing,
            Attacking,
            Fleeing,
            Dead,
            Investigating,
            Returning
        }

        [SerializeField] private State currentState;
        private Transform player;

        private Vector2 wanderDirection;
        private float wanderTimer;

        private Vector2 spawnPosition;
        private float currentSpeed;
        private float attackTimer;
        private float alertCooldown = 1f;
        private float lastAlertTime;
        private Vector3 lastKnownPosition;
        private float memoryDuration = 5f;
        private float memoryTimer;
        private float investigateTimer;
        private float investigateDuration = 10f;
        public bool isStaggered = false;
        public float staggerDuration;

        private bool isPlayerInSight;
        private bool isPlayerInAttackRange;
        private bool isPlayerHeard;

        [Header("Assign in Inspector")]
        public float wanderSpeed;
        public float chaseSpeed;
        public float attackSpeed;
        public float lineOfSight;
        public float hearingRadius;
        public float attackRange;
        public float wanderRadius;
        public float wanderInterval;
        public float attackInterval;
        public float deathDelay;
        public float damage;
        public float maxChaseDistance = 30f;
        public Rigidbody2D rb;
        public Animator animator;
        public AudioClip walkingAudio;
        public AudioClip attackingAudio;
        private AudioSource zombieAudio;
        private CameraShake cameraShake;

        private bool isAlerted = false;  // Track if the zombie is alerted
        private bool canAttack;

        private void Awake()
        {
            zombieAudio = GetComponent<AudioSource>();
        }

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player == null)
            {
                Debug.LogError("Player not found in the scene.");
                return;
            }

            spawnPosition = transform.position;

            wanderTimer = wanderInterval;
            currentSpeed = wanderSpeed;

            currentState = State.Wander;

            canAttack = true;
            if (walkingAudio == null)
            {
                if (walkingAudio == null)
                {
                    Debug.LogWarning("AudioSource component not found on the zombie.");
                }
            }

            if (cameraShake == null)
            {
                Camera mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    cameraShake = mainCamera.GetComponent<CameraShake>();
                }
            }

            if (cameraShake == null)
            {
                Debug.LogWarning("CameraShake component not found on the main camera.");
            }
        }

        private void Update()
        {
            if (currentState == State.Dead) return;

            DetectPlayer();
            DetectSound();

            HandleState();

            if (memoryTimer > 0)
            {
                memoryTimer -= Time.deltaTime;
            }
            else if (lastKnownPosition != Vector3.zero && !isAlerted)
            {
                currentState = State.Returning;
                currentSpeed = wanderSpeed;
                lastKnownPosition = Vector3.zero;
            }
        }

        private void HandleState()
        {
            switch (currentState)
            {
                case State.Wander:
                case State.Patrolling:
                    Wander();
                    break;
                case State.Chasing:
                    MoveZombie(player.position);
                    PlayWalkingAudio();
                    if (Vector2.Distance(transform.position, player.position) < attackRange)
                    {
                        currentState = State.Attacking;
                        attackTimer = 0f;
                    }
                    break;
                case State.Attacking:
                    Attack();
                    break;
                case State.Fleeing:
                    Flee();
                    break;
                case State.Investigating:
                    Investigate();
                    break;
                case State.Returning:
                    Return();
                    break;
                case State.Dead:
                    Die();
                    break;
            }
        }
        private void PlayWalkingAudio()
        {
            if (!zombieAudio.isPlaying && !canAttack)
            {
                zombieAudio.clip = walkingAudio;
                zombieAudio.Play();
            }
        }

        private void MoveZombie(Vector2 targetPosition)
        {
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, currentSpeed * Time.deltaTime);
            if (hit.collider != null)
            {
                Vector2 avoidanceDirection = Vector2.Perpendicular(direction).normalized * 0.5f;
                direction = (direction + avoidanceDirection).normalized;
            }
            Vector2 newPosition = rb.position + direction * currentSpeed * Time.deltaTime;
            rb.MovePosition(newPosition);

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rb.rotation = Mathf.LerpAngle(rb.rotation, angle, 0.1f); // Adjusted damping factor for smoother rotation

            animator.SetBool("isMoving", true);
        }


        private void Wander()
        {
            wanderTimer += Time.deltaTime;
            if (wanderTimer >= wanderInterval)
            {
                wanderTimer = 0;
                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                wanderDirection = spawnPosition + randomDirection * wanderRadius;
            }
            MoveZombie(wanderDirection);
        }

        private void Investigate()
        {
            investigateTimer += Time.deltaTime;
            if (investigateTimer >= investigateDuration)
            {
                investigateTimer = 0;
                currentState = State.Returning;
                currentSpeed = wanderSpeed;
            }
            else
            {
                MoveZombie(lastKnownPosition);
            }
        }

        private void Return()
        {
            if (Vector2.Distance(transform.position, spawnPosition) < 0.1f)
            {
                currentState = State.Wander;
                animator.SetBool("isMoving", false);
            }
            else
            {
                MoveZombie(spawnPosition);
            }
        }

        private void Attack()
        {
            if (!canAttack || isStaggered) return;  // Prevent attacking if staggered or during cooldown

            attackTimer += Time.deltaTime;
            if (attackTimer >= attackInterval)
            {
                attackTimer = 0;
                if (isPlayerInAttackRange)
                {
                    var playerComponent = player.GetComponent<Player>();
                    if (playerComponent != null)
                    {
                        playerComponent.DecreaseHealth(damage);
                        zombieAudio.PlayOneShot(attackingAudio);
                        cameraShake?.Shake();
                        StartCoroutine(AttackCooldown()); // Start cooldown after attack
                        animator.SetBool("isAttacking", true);
                    }
                }
                else
                {
                    MoveZombie(player.position);
                }
            }
        }

        private IEnumerator AttackCooldown()
        {
            canAttack = false;
            yield return new WaitForSeconds(attackInterval); // Cooldown duration
            canAttack = true;

            animator.SetBool("isAttacking", false);
        }


        private void Flee()
        {
            Vector2 fleeDirection = (transform.position - player.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, fleeDirection, currentSpeed * Time.deltaTime);
            if (hit.collider != null)
            {
                Vector2 avoidanceDirection = Vector2.Perpendicular(fleeDirection).normalized;
                fleeDirection += avoidanceDirection * 0.5f;
                fleeDirection.Normalize();
            }
            Vector2 newPosition = rb.position + fleeDirection * currentSpeed * Time.deltaTime;
            rb.MovePosition(newPosition);
            float angle = Mathf.Atan2(fleeDirection.y, fleeDirection.x) * Mathf.Rad2Deg;
            rb.rotation = Mathf.LerpAngle(rb.rotation, angle, Time.deltaTime * currentSpeed); // Smooth rotation

            animator.SetBool("isMoving", true);

        }

        private void Die()
        {
            animator.SetTrigger("Die");

            animator.SetTrigger("Die");
            currentSpeed = 0;
            Destroy(gameObject, deathDelay);
        }

        private void DetectPlayer()
        {
            float distanceFromPlayer = Vector2.Distance(player.position, transform.position);
            isPlayerInSight = distanceFromPlayer < lineOfSight;
            isPlayerInAttackRange = distanceFromPlayer < attackRange;

            if (isPlayerInAttackRange)
            {
                currentState = State.Attacking;
                currentSpeed = attackSpeed;
            }
            else if (isPlayerInSight)
            {
                currentState = State.Chasing;
                currentSpeed = chaseSpeed;
                lastKnownPosition = player.position; // Update last known position to current player position
            }
            else if (distanceFromPlayer <= maxChaseDistance && lastKnownPosition != Vector3.zero)
            {
                currentState = State.Investigating;
                currentSpeed = chaseSpeed;
            }
            else if (!isAlerted)
            {
                currentState = State.Wander;
                currentSpeed = wanderSpeed;
            }
        }

        public void OnGunShotHeard(Vector2 shotPosition)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, (shotPosition - (Vector2)transform.position).normalized, hearingRadius);
            if (hit.collider == null || hit.collider.CompareTag("Player"))
            {
                isPlayerHeard = true;
                AlertNearbyZombies(shotPosition);
                currentState = State.Chasing;
                currentSpeed = chaseSpeed;
                lastKnownPosition = shotPosition;
                memoryTimer = memoryDuration;
                isAlerted = true;
            }
        }

        private void AlertNearbyZombies(Vector2 shotPosition)
        {
            if (Time.time - lastAlertTime < alertCooldown) return;
            lastAlertTime = Time.time;

            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(shotPosition, hearingRadius);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Zombie") && hitCollider.gameObject != this.gameObject)
                {
                    Zombie nearbyZombie = hitCollider.GetComponent<Zombie>();
                    if (nearbyZombie != null)
                    {
                        nearbyZombie.OnGunShotHeard(shotPosition);
                    }
                }
            }
        }

        private void DetectSound()
        {
            isPlayerHeard = false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, lineOfSight);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, hearingRadius);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(spawnPosition, wanderRadius);

            if (lastKnownPosition != Vector3.zero)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(lastKnownPosition, 0.5f);
            }
        }

        public void Stagger(float duration)
        {
            staggerDuration = duration;
            StartCoroutine(StaggerCoroutine());
        }

        private IEnumerator StaggerCoroutine()
        {
            Debug.Log("Zombie staggered started.");
            isStaggered = true;
            currentSpeed = 0; // Stop movement
            canAttack = false; // Disable attacks

            yield return new WaitForSeconds(staggerDuration);

            isStaggered = false;
            canAttack = true; // Re-enable attacks
            Debug.Log("Zombie staggered ended.");

            // Restore movement speed based on the current state
            currentSpeed = (currentState == State.Chasing) ? chaseSpeed : wanderSpeed;
        }
    }
}
