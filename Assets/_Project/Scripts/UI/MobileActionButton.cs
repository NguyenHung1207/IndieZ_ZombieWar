using UnityEngine;
using UnityEngine.EventSystems;

public enum MobileAction
{
    SwitchWeapon,
    ThrowGrenade,
    Reload
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
        switch (action)
        {
            case MobileAction.SwitchWeapon:
                weaponController?.SwitchWeapon();
                break;
            case MobileAction.Reload:
                weaponController?.Reload();
                break;
            default:
                grenadeController?.ThrowGrenade();
                break;
        }
    }
}
