using UnityEngine;
using UnityEngine.EventSystems;

public sealed class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform handle;
    [SerializeField, Min(1f)] private float radius = 100f;
    [SerializeField, Range(0f, 1f)] private float deadZone = 0.12f;

    private RectTransform rectTransform;
    private int pointerId = int.MinValue;
    private Vector2 touchOrigin;
    private Vector2 value;

    public Vector2 Value => value;
    public bool IsDragging => pointerId != int.MinValue;

    // The usable travel radius is deliberately smaller than the visual base so
    // the handle stays inside its ring at full deflection.
    public void ConfigureRadius(float travelRadius)
    {
        radius = Mathf.Max(1f, travelRadius);
        UpdateHandle();
    }

    private void Awake()
    {
        rectTransform = (RectTransform)transform;
        ResetInput();
    }

    private void Update()
    {
        if (GameSession.Instance != null && !GameSession.Instance.IsPlaying && IsDragging)
            ResetInput();
    }

    private void OnDisable()
    {
        ResetInput();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameSession.Instance != null && !GameSession.Instance.IsPlaying)
            return;

        // A second finger landing on the joystick must not take ownership from
        // the finger already driving movement.
        if (IsDragging)
            return;

        // A touch can land anywhere within the visual base. Treat that point as
        // this touch's neutral origin so merely touching the joystick never
        // produces movement.
        if (!TryGetLocalPoint(eventData, out touchOrigin))
            return;

        pointerId = eventData.pointerId;
        value = Vector2.zero;
        UpdateHandle();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerId == pointerId)
            UpdateValue(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId == pointerId)
            ResetInput();
    }

    public void ResetInput()
    {
        pointerId = int.MinValue;
        touchOrigin = Vector2.zero;
        value = Vector2.zero;
        UpdateHandle();
    }

    private void UpdateValue(PointerEventData eventData)
    {
        if (!TryGetLocalPoint(eventData, out Vector2 localPoint))
            return;

        Vector2 delta = Vector2.ClampMagnitude(localPoint - touchOrigin, radius);
        float normalizedMagnitude = delta.magnitude / radius;
        if (normalizedMagnitude <= deadZone)
        {
            value = Vector2.zero;
        }
        else
        {
            // Remove the dead zone before normalizing the direction, then map
            // the remaining travel back to the full analog range.
            float analogMagnitude = (normalizedMagnitude - deadZone) / (1f - deadZone);
            value = delta.normalized * analogMagnitude;
        }
        UpdateHandle();
    }

    private bool TryGetLocalPoint(PointerEventData eventData, out Vector2 localPoint)
    {
        // pressEventCamera is the camera that owns this pointer's UI press;
        // it is correctly null for Screen Space Overlay canvases.
        return RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, eventData.position, eventData.pressEventCamera, out localPoint);
    }

    private void UpdateHandle()
    {
        if (handle != null)
            handle.anchoredPosition = value * radius;
    }
}
