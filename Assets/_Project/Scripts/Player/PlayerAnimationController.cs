using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerMovement))]
public sealed class PlayerAnimationController : MonoBehaviour
{
    private static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
    private static readonly int ShootHash = Animator.StringToHash("Shoot");

    [SerializeField] private Animator animator;
    [SerializeField, Min(0f)] private float parameterDampTime = 0.1f;

    private CharacterController characterController;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
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
        if (animator == null || characterController == null || playerMovement == null)
        {
            return;
        }

        Vector3 horizontalVelocity = new Vector3(
            characterController.velocity.x,
            0f,
            characterController.velocity.z);

        float normalizedSpeed = playerMovement.MoveSpeed > 0f
            ? horizontalVelocity.magnitude / playerMovement.MoveSpeed
            : 0f;
        normalizedSpeed = Mathf.Clamp01(normalizedSpeed);

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
