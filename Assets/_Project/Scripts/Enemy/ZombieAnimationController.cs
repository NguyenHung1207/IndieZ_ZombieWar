using UnityEngine;
using UnityEngine.AI;

public sealed class ZombieAnimationController : MonoBehaviour
{
    private const float HitReactionCooldown = 0.25f;
    private static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int HitHash = Animator.StringToHash("Hit");
    private static readonly int DieHash = Animator.StringToHash("Die");

    private Animator animator;
    private NavMeshAgent agent;
    private float nextHitReactionTime;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>(true);
        agent = GetComponentInParent<NavMeshAgent>();
        animator.applyRootMotion = false;
    }

    private void Update()
    {
        if (agent == null || animator == null)
            return;
        float speed = agent.speed > 0.01f ? agent.velocity.magnitude / agent.speed : 0f;
        animator.SetFloat(MoveSpeedHash, Mathf.Clamp01(speed), 0.12f, Time.deltaTime);
    }

    public void PlayAttack() => animator.SetTrigger(AttackHash);
    public bool PlayHit()
    {
        if (animator == null || Time.time < nextHitReactionTime)
            return false;

        nextHitReactionTime = Time.time + HitReactionCooldown;
        animator.ResetTrigger(HitHash);
        animator.SetTrigger(HitHash);
        return true;
    }
    public void PlayDeath() => animator.SetTrigger(DieHash);
}
