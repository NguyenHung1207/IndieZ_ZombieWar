using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(ZombieHealth))]
public sealed class ZombieAI : MonoBehaviour
{
    private enum State
    {
        Chase,
        Attack,
        Dead
    }

    [SerializeField] private Transform target;
    [SerializeField, Min(0.1f)] private float moveSpeed = 3f;
    [SerializeField, Min(0.1f)] private float attackRange = 1.5f;
    [SerializeField, Min(0.1f)] private float attackCooldown = 1.2f;
    [SerializeField, Min(0.1f)] private float attackDamage = 12f;
    [SerializeField, Min(0f)] private float cleanupDelay = 2f;

    private NavMeshAgent agent;
    private ZombieHealth health;
    private State state;
    private float nextPathTime;
    private float nextAttackTime;
    private Vector3 knockbackVelocity;

    public bool IsDead => state == State.Dead;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void StopForGameEnd()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
        enabled = false;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<ZombieHealth>();
        health.Died += Die;

        agent.speed = moveSpeed;
        agent.stoppingDistance = attackRange;
        agent.angularSpeed = 720f;
        agent.acceleration = 12f;
        agent.autoBraking = true;
        state = State.Chase;
    }

    private void Start()
    {
        if (target == null)
        {
            PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
            target = player != null ? player.transform : null;
        }
    }

    private void Update()
    {
        if (state == State.Dead || target == null)
            return;
        if (GameSession.Instance != null && !GameSession.Instance.IsPlaying)
            return;

        if (knockbackVelocity.sqrMagnitude > 0.001f && agent.isOnNavMesh)
        {
            agent.Move(knockbackVelocity * Time.deltaTime);
            knockbackVelocity = Vector3.MoveTowards(
                knockbackVelocity,
                Vector3.zero,
                10f * Time.deltaTime);
        }

        Vector3 targetPosition = target.position;
        Vector3 offset = targetPosition - transform.position;
        offset.y = 0f;
        float distance = offset.magnitude;

        if (distance > attackRange)
        {
            state = State.Chase;
            if (!agent.isOnNavMesh)
                return;

            agent.isStopped = false;
            if (Time.time >= nextPathTime)
            {
                agent.SetDestination(targetPosition);
                nextPathTime = Time.time + 0.1f;
            }
            return;
        }

        state = State.Attack;
        if (agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        if (offset.sqrMagnitude > 0.001f)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(offset, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                desiredRotation,
                agent.angularSpeed * Time.deltaTime);
        }

        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            IDamageable damageable = target.GetComponent<PlayerHealth>();
            damageable?.TakeDamage(attackDamage);
        }
    }

    public void ApplyExplosionKnockback(Vector3 impulse)
    {
        if (state == State.Dead)
            return;
        impulse.y = 0f;
        knockbackVelocity += impulse;
    }

    private void Die()
    {
        if (state == State.Dead)
            return;

        state = State.Dead;
        if (agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        foreach (Collider collider in GetComponentsInChildren<Collider>())
            collider.enabled = false;

        if (cleanupDelay <= 0f)
            Destroy(gameObject);
        else
            Destroy(gameObject, cleanupDelay);
    }

    private void OnDestroy()
    {
        if (health != null)
            health.Died -= Die;
    }
}
