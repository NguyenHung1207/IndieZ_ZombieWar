using UnityEngine;
using UnityEngine.UI;

public sealed class ReloadButtonUI : MonoBehaviour
{
    [SerializeField] private PlayerWeaponController weaponController;
    [SerializeField] private Image icon;
    [SerializeField] private Button actionButton;

    public void Configure(PlayerWeaponController controller, Image reloadIcon, Button button)
    {
        weaponController = controller;
        icon = reloadIcon;
        actionButton = button;
    }

    private void Update()
    {
        if (weaponController == null) return;
        bool reloading = weaponController.IsReloading;
        if (icon != null)
            icon.color = reloading ? new Color(0.48f, 0.48f, 0.48f, 0.72f) : Color.white;
        if (actionButton != null)
            actionButton.interactable = !reloading;
    }
}
