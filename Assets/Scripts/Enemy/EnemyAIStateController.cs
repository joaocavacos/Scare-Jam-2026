using System.Collections;
using ArtificeToolkit.Attributes;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIStateController : MonoBehaviour
{
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Attack = Animator.StringToHash("Attack");

    public enum AIState { Idle, Patrol, Chase, Attack, Search }

    [Header("Current State")]
    [SerializeField] private AIState currentState = AIState.Patrol;

    [Header("Patrol & Idle Settings")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float patrolSpeed = 2.0f;
    [SerializeField] private float minIdleTime = 2.0f;
    [SerializeField] private float maxIdleTime = 5.0f;

    [Header("Senses (Sight & Hearing)")]
    [SerializeField] private float sightDistance = 15f;
    [SerializeField] private float fieldOfViewAngle = 90f;
    [SerializeField] private float hearingSensitivity = 1f;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Chase & Attack Settings")]
    [SerializeField] private float chaseSpeed = 5.0f;
    [SerializeField] private float attackDistance = 1.5f;
    [SerializeField] private float attackCooldown = 2.0f;
    [SerializeField] private float animationAttackDelay = 1.8f;

    [Header("Search Settings")]
    [SerializeField] private float searchDuration = 4.0f;

    [Header("Player References")]
    [SerializeField] private PlayerMovement playerController;

    // References
    private NavMeshAgent agent;
    private Animator animator;
    [SerializeField, ReadOnly] private Transform playerTransform;

    // Tracking variables
    private int currentWaypointIndex = 0;
    private float stateTimer = 0f;
    private Vector3 lastKnownPlayerPosition;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (playerController != null)
        {
            playerTransform = playerController.GetPlayerTransform();
        }
    }

    private void OnEnable()
    {
        GlowstickController.OnNoiseGenerated += HandleNoiseHeard;
    }

    private void OnDisable()
    {
        GlowstickController.OnNoiseGenerated -= HandleNoiseHeard;
    }

    private void Update()
    {
        UpdateAnimations();

        switch (currentState)
        {
            case AIState.Idle: HandleIdle(); break;
            case AIState.Patrol: HandlePatrol(); break;
            case AIState.Chase: HandleChase(); break;
            case AIState.Attack: HandleAttack(); break;
            case AIState.Search: HandleSearch(); break;
        }
    }

    // States

    private void HandleIdle()
    {
        agent.isStopped = true;
        stateTimer -= Time.deltaTime;

        if (CanSeePlayer())
        {
            ChangeState(AIState.Chase);
            return;
        }

        if (stateTimer <= 0)
        {
            ChangeState(AIState.Patrol);
        }
    }

    private void HandlePatrol()
    {
        agent.isStopped = false;
        agent.speed = patrolSpeed;

        if (CanSeePlayer())
        {
            ChangeState(AIState.Chase);
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            // Reached waypoint, transition to Idle
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            ChangeState(AIState.Idle);
        }
    }

    private void HandleChase()
    {
        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.SetDestination(playerTransform.position);
        lastKnownPlayerPosition = playerTransform.position;

        Vector3 flatEnemyPosition = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 flatPlayerPosition = new Vector3(playerTransform.position.x, 0f, playerTransform.position.z);
        float distanceToPlayer = Vector3.Distance(flatEnemyPosition, flatPlayerPosition);

        if (distanceToPlayer <= attackDistance)
        {
            ChangeState(AIState.Attack);
            return;
        }

        if (!CanSeePlayer())
        {
            ChangeState(AIState.Search);
        }
    }

    private void HandleAttack()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        stateTimer -= Time.deltaTime;

        // Face the player while attacking
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        direction.y = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);

        if (stateTimer <= 0)
        {
            // Execute actual attack damage here
            animator.SetTrigger(Attack);

            StartCoroutine(DealDelayedDamage());

            // Reset cooldown
            stateTimer = attackCooldown;
        }

        Vector3 flatEnemyPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 flatPlayerPos = new Vector3(playerTransform.position.x, 0, playerTransform.position.z);
        float distanceToPlayer = Vector3.Distance(flatEnemyPos, flatPlayerPos);

        // If player runs away during attack cooldown, go back to chase
        if (distanceToPlayer > attackDistance + 0.5f)
        {
            ChangeState(AIState.Chase);
        }
    }

    private IEnumerator DealDelayedDamage()
    {
        // Wait for the animation to reach the impact frame
        yield return new WaitForSeconds(animationAttackDelay);

        // If the enemy was stunned or changed states during the swing, cancel the damage
        if (currentState != AIState.Attack)
        {
            yield break;
        }

        // Double-check if the player is still in range (so they can physically dodge the swing)
        Vector3 flatEnemyPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 flatPlayerPos = new Vector3(playerTransform.position.x, 0, playerTransform.position.z);
        float distanceToPlayer = Vector3.Distance(flatEnemyPos, flatPlayerPos);

        if (distanceToPlayer <= attackDistance + 0.5f)
        {
            if (playerTransform.TryGetComponent<PlayerHealth>(out PlayerHealth health))
            {
                health.TakeDamage();
            }
        }
    }

    private void HandleSearch()
    {
        agent.isStopped = false;
        agent.speed = patrolSpeed;
        agent.SetDestination(lastKnownPlayerPosition);

        stateTimer -= Time.deltaTime;

        if (CanSeePlayer())
        {
            ChangeState(AIState.Chase);
            return;
        }

        if (stateTimer <= 0)
        {
            ChangeState(AIState.Patrol);
        }
    }

    private void ChangeState(AIState newState)
    {
        if (currentState == newState)
        {
            return;
        }

        Debug.Log($"Enemy changing state from {currentState} to {newState}");
        currentState = newState;

        switch (newState)
        {
            case AIState.Idle:
                stateTimer = Random.Range(minIdleTime, maxIdleTime);
                break;
            case AIState.Patrol:
            {
                if (waypoints.Length > 0)
                {
                    agent.SetDestination(waypoints[currentWaypointIndex].position);
                }
                break;
            }
            case AIState.Attack:
                // Attack immediately upon entering state
                stateTimer = 0f;
                break;
            case AIState.Search:
                stateTimer = searchDuration;
                break;
        }
    }

    // Sensors
    private bool CanSeePlayer()
    {
        if (playerTransform == null)
        {
            return false;
        }

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        if (distance > sightDistance) return false;

        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        if (angle < fieldOfViewAngle / 2f)
        {
            // Raycast to ensure no walls are in the way
            if (!Physics.Raycast(transform.position + Vector3.up, directionToPlayer, distance, obstacleMask))
            {
                // Line of sight is clear
                return true;
            }
        }
        return false;
    }

    private void HandleNoiseHeard(Vector3 noisePosition, float noiseRadius)
    {
        float distanceToNoise = Vector3.Distance(transform.position, noisePosition);

        // If the noise is loud enough to reach the monster based on its sensitivity
        if (distanceToNoise <= noiseRadius * hearingSensitivity)
        {
            lastKnownPlayerPosition = noisePosition;
            
            // Only interrupt if not actively attacking or already chasing
            if (currentState != AIState.Chase && currentState != AIState.Attack)
            {
                ChangeState(AIState.Search);
            }
        }
    }

    // Animations
    private void UpdateAnimations()
    {
        animator.SetFloat(Speed, agent.velocity.magnitude);
    }

    // Debug
    private void OnDrawGizmosSelected()
    {
        // Attack Distance
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);

        // Sight Distance
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightDistance);

        // Field of View Cone
        Gizmos.color = Color.cyan;
        // Calculate the left and right angles based on where the enemy is facing
        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfViewAngle / 2f, 0) * transform.forward;
        Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfViewAngle / 2f, 0) * transform.forward;
        
        // Draw the rays from chest height
        Vector3 rayStart = transform.position + Vector3.up; 
        Gizmos.DrawRay(rayStart, rightBoundary * sightDistance);
        Gizmos.DrawRay(rayStart, leftBoundary * sightDistance);

        // Patrol Route
        if (waypoints != null && waypoints.Length > 0)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] != null)
                {
                    // Draw a small sphere at the waypoint
                    Gizmos.DrawSphere(waypoints[i].position, 0.3f);
                    
                    // Draw a line to the next waypoint
                    int nextIndex = (i + 1) % waypoints.Length;
                    if (waypoints[nextIndex] != null)
                    {
                        Gizmos.DrawLine(waypoints[i].position, waypoints[nextIndex].position);
                    }
                }
            }
        }
    }
}
