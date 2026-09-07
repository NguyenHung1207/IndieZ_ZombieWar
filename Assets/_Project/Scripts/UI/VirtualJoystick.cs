using UnityEngine;
using UnityEngine.EventSystems;

public sealed class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform handle;
    [SerializeField, Min(1f)] private float radius = 100f;
    [SerializeField, Range(0f, 1f)] private float deadZone = 0.12f;

    private RectTransform rectTransform;
    private int pointerId = int.MinValue;
    private Vector2 value;

    public Vector2 Value => value;
    public bool IsDragging => pointerId != int.MinValue;

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
        pointerId = eventData.pointerId;
        UpdateValue(eventData);
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
        value = Vector2.zero;
        UpdateHandle();
    }

    private void UpdateValue(PointerEventData eventData)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
            return;
        Vector2 normalized = Vector2.ClampMagnitude(localPoint / radius, 1f);
        value = normalized.magnitude >= deadZone ? normalized : Vector2.zero;
        UpdateHandle();
    }

    private void UpdateHandle()
    {
        if (handle != null)
            handle.anchoredPosition = value * radius;
    }
}
