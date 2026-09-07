using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public sealed class PlayerMovement : MonoBehaviour
{
    [SerializeField, Min(0f)] private float moveSpeed = 5f;
    [SerializeField, Min(0f)] private float rotationSpeed = 720f;
    [SerializeField] private VirtualJoystick mobileJoystick;

    public float MoveSpeed => moveSpeed;

    private CharacterController characterController;
    private float verticalVelocity;
    private Vector2 externalMoveInput;
    private bool externalInputActive;

    public void SetMoveInput(Vector2 input)
    {
        externalMoveInput = Vector2.ClampMagnitude(input, 1f);
        externalInputActive = externalMoveInput.sqrMagnitude >= 0.0144f;
    }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (GameSession.Instance != null && !GameSession.Instance.IsPlaying)
            return;
        Vector2 desktopInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 selectedInput = mobileJoystick != null && mobileJoystick.IsDragging
            ? mobileJoystick.Value
            : externalInputActive ? externalMoveInput : desktopInput;
        Vector3 input = new Vector3(selectedInput.x, 0f, selectedInput.y);
        Vector3 moveDirection = Vector3.ClampMagnitude(input, 1f);

        if (moveDirection.sqrMagnitude > 0f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime);
        }

        if (characterController.isGrounded)
        {
            verticalVelocity = Physics.gravity.y * Time.deltaTime;
        }
        else
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }

        Vector3 velocity = moveDirection * moveSpeed;
        velocity.y = verticalVelocity;
        characterController.Move(velocity * Time.deltaTime);
    }
}
