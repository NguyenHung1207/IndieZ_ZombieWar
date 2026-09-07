using UnityEngine;
using UnityEngine.AI;

public sealed class ZombieAnimationController : MonoBehaviour
{
    private static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int HitHash = Animator.StringToHash("Hit");
    private static readonly int DieHash = Animator.StringToHash("Die");

    private Animator animator;
    private NavMeshAgent agent;

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
    public void PlayHit() => animator.SetTrigger(HitHash);
    public void PlayDeath() => animator.SetTrigger(DieHash);
}
