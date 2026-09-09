using UnityEngine;
using UnityEngine.UI;

public sealed class JoystickVisualFeedback : MonoBehaviour
{
    [SerializeField] private VirtualJoystick joystick;
    [SerializeField] private Image baseImage;
    [SerializeField] private Image handleImage;

    public void Configure(VirtualJoystick source, Image background, Image handle)
    {
        joystick = source;
        baseImage = background;
        handleImage = handle;
    }

    private void Update()
    {
        bool active = joystick != null && joystick.IsDragging;
        if (baseImage != null)
            baseImage.color = active ? new Color(0.12f, 0.14f, 0.16f, 0.72f) : new Color(0.08f, 0.09f, 0.11f, 0.48f);
        if (handleImage != null)
            handleImage.color = active ? new Color(0.92f, 0.94f, 0.96f, 0.92f) : new Color(0.78f, 0.80f, 0.82f, 0.72f);
    }
}
