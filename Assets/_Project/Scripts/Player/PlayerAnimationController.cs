using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerMovement))]
public sealed class PlayerAnimationController : MonoBehaviour
{
    private static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
    private static readonly int ShootHash = Animator.StringToHash("Shoot");

    [SerializeField] private Animator animator;
    [SerializeField, Min(0f)] private float parameterDampTime = 0.1f;

    private PlayerMovement playerMovement;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (animator != null)
        {
            animator.applyRootMotion = false;
        }
    }

    private void Update()
    {
        if (animator == null || playerMovement == null)
        {
            return;
        }

        // Animation follows the requested input only; it never supplies motion
        // to the CharacterController.
        float normalizedSpeed = playerMovement.MoveInputMagnitude;

        animator.SetFloat(
            MoveSpeedHash,
            normalizedSpeed,
            parameterDampTime,
            Time.deltaTime);
    }

    public void PlayShoot()
    {
        if (animator == null)
        {
            return;
        }

        animator.SetTrigger(ShootHash);
    }
}
