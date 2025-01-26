using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour, IAiStateMachine, ITarget
{
    [Range(0.01f, 2.0f)]
    [Tooltip("Scales the time between ai state checks")]
    public float reactionSpeed = 1.0f;
    
    [Range(0.0f, 1000.0f)]
    public float visionRange = 10.0f;
    
    [Range(0.0f, 1000.0f)]
    public float attackRange = 1.0f;
    
    [Range(1, 1000)]
    public int attackDamage = 10;

    [Range(0.0f, 20.0f)]
    [Tooltip("The time between attacks")]
    public float attackCooldown = 1.0f;
    
    [Range(0, 10000)]
    public int health = 100;
    
    private int playerMask;

    private NavMeshAgent agent;
    
    private ITarget currentTarget;

    private void Awake()
    {
        playerMask = LayerMask.GetMask("Player");
        agent = GetComponent<NavMeshAgent>();
    }
    
    // IAiStateMachine Implementation
    
    private static Collider[] sphereResults = new Collider[64];
    
    public AiStateController AiController { get; set; }
    
    private ITarget FindTarget(float radius)
    {
        // TODO - we need a player collider
        var count = Physics.OverlapSphereNonAlloc(transform.position, radius, sphereResults, playerMask);
        if (count == 0)
        {
            return null;
        }
        for (var i = 0; i < count; i++)
        {
            var player = sphereResults[i];
            var target = player.gameObject.GetComponent<ITarget>();
            if (target == null || target.TargetType != TargetType.Player)
            {
                continue;
            }

            // Check that we have line of sight to the player
            Physics.Raycast(transform.position, player.transform.position - transform.position, out var hit, radius, playerMask);
                
            if (hit.collider != player)
            {
                Debug.Log($"Player is not in line of sight {hit.collider}", gameObject);
                return null;
            }
            
            Debug.Log("Found player in radius, attacking", gameObject);
            return target;
        }
        Debug.Log("Player not found in radius", gameObject);
        return null;
    }
    
    public AiState OnIdleState(AiState previousState)
    {
        return AiPhase.Patrol.AsState(0.25f / reactionSpeed);
    }

    public AiState OnPatrolState(AiState previousState)
    {
        var findTarget = FindTarget(visionRange);

        if (findTarget == null)
        {
            return AiPhase.Patrol.AsState(0.5f / reactionSpeed);
        }

        currentTarget = findTarget;
        return AiPhase.Chase.AsState(0.25f / reactionSpeed);
    }

    public AiState OnChaseState(AiState previousState)
    {
        if (currentTarget == null)
        {
            return AiPhase.Patrol.AsState(0.5f / reactionSpeed);
        }
        agent.destination = currentTarget.GetPosition();
        if (canAttack)
        {
            return AiPhase.Attack.AsState(0.25f / reactionSpeed);
        }
        return AiPhase.Chase.AsState(0.25f / reactionSpeed);
    }

    private bool canAttack => Vector3.Distance(transform.position, currentTarget.GetPosition()) <= attackRange;

    public AiState OnAttackState(AiState previousState)
    {
        if (!canAttack)
        {
            return AiPhase.Chase.AsState(0.25f / reactionSpeed);
        }

        currentTarget.Hit(attackDamage, this);
        if (currentTarget.IsAlive)
        {
            return AiPhase.Attack.AsState(attackCooldown);
        }

        currentTarget = null;
        return AiPhase.Patrol.AsState(0.5f / reactionSpeed);
    }

    public AiState OnDeadState(AiState previousState)
    {
        // Can play a death animation or whatnot here
        gameObject.SetActive(false);
        onKill.Invoke();
        return AiPhase.Dead.AsState(10.0f);
    }
    
    // ITarget Implementation
    TargetType ITarget.TargetType => TargetType.Enemy;
    Vector3 ITarget.GetPosition() => transform.position;

    public void Hit(int damage, ITarget attacker = null)
    {
        if (attacker != null && attacker.TargetType == TargetType.Player)
        {
            currentTarget = attacker;
        }
        
        if (damage > health)
        {
            health = 0;
            
            return;
        }
        health -= damage;
    }

    public UnityEvent onHit;
    public UnityEvent onKill;
    public UnityEvent onRevive;

    UnityEvent ITarget.HitEvent => onHit;
    UnityEvent ITarget.KillEvent => onKill;
    UnityEvent ITarget.ReviveEvent => onRevive;
}