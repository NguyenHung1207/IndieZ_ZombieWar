using UnityEngine;
using UnityEngine.EventSystems;

public sealed class FireHoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private PlayerWeaponController weaponController;
    private int pointerId = int.MinValue;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (pointerId != int.MinValue)
            return;

        pointerId = eventData.pointerId;
        weaponController?.SetFireHeld(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Release(eventData.pointerId);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Release(eventData.pointerId);
    }

    private void OnDisable()
    {
        Release(pointerId);
    }

    private void Release(int releasedPointerId)
    {
        if (releasedPointerId != pointerId)
            return;

        pointerId = int.MinValue;
        weaponController?.SetFireHeld(false);
    }
}
