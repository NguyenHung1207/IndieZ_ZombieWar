using UnityEngine;
using UnityEngine.EventSystems;

public enum MobileAction
{
    SwitchWeapon,
    ThrowGrenade
}

public sealed class MobileActionButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private MobileAction action;
    [SerializeField] private PlayerWeaponController weaponController;
    [SerializeField] private PlayerGrenadeController grenadeController;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameSession.Instance != null && !GameSession.Instance.IsPlaying)
            return;
        if (action == MobileAction.SwitchWeapon)
            weaponController?.SwitchWeapon();
        else
            grenadeController?.ThrowGrenade();
    }
}
