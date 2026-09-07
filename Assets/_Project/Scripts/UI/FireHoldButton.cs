using UnityEngine;
using UnityEngine.EventSystems;

public sealed class FireHoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private PlayerWeaponController weaponController;

    public void OnPointerDown(PointerEventData eventData)
    {
        weaponController?.SetFireHeld(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Release();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerId < 0)
            return;
        Release();
    }

    private void OnDisable()
    {
        Release();
    }

    private void Release()
    {
        weaponController?.SetFireHeld(false);
    }
}
