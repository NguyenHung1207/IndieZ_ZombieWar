using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public sealed class PlayerMovement : MonoBehaviour
{
    [SerializeField, Min(0f)] private float moveSpeed = 5f;
    [SerializeField, Min(0f)] private float rotationSpeed = 720f;
    [SerializeField] private VirtualJoystick mobileJoystick;

    public float MoveSpeed => moveSpeed;
    public float MoveInputMagnitude { get; private set; }

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
        {
            MoveInputMagnitude = 0f;
            return;
        }
        Vector2 desktopInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 selectedInput = mobileJoystick != null && mobileJoystick.IsDragging
            ? mobileJoystick.Value
            : externalInputActive ? externalMoveInput : desktopInput;
        // Keep the analog magnitude for translation, but use a unit vector for
        // facing so rotation can never reduce the requested move speed.
        float inputMagnitude = selectedInput.magnitude;
        float moveMagnitude = Mathf.Clamp01(inputMagnitude);
        MoveInputMagnitude = moveMagnitude;
        Vector3 moveDirection = inputMagnitude > 0f
            ? new Vector3(selectedInput.x / inputMagnitude, 0f, selectedInput.y / inputMagnitude)
            : Vector3.zero;

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

        Vector3 velocity = moveDirection * (moveSpeed * moveMagnitude);
        velocity.y = verticalVelocity;
        characterController.Move(velocity * Time.deltaTime);
    }
}
