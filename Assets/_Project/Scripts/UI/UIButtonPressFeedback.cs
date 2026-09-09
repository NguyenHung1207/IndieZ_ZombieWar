using UnityEngine;
using UnityEngine.EventSystems;

public sealed class UIButtonPressFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField, Range(0.85f, 1f)] private float pressedScale = 0.94f;
    private Vector3 restingScale;

    private void Awake() => restingScale = transform.localScale;
    private void OnDisable() => transform.localScale = restingScale == Vector3.zero ? Vector3.one : restingScale;
    public void OnPointerDown(PointerEventData eventData) => transform.localScale = restingScale * pressedScale;
    public void OnPointerUp(PointerEventData eventData) => transform.localScale = restingScale;
    public void OnPointerExit(PointerEventData eventData) => transform.localScale = restingScale;
}
