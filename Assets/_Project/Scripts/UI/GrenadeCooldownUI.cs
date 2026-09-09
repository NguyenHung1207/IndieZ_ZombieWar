using UnityEngine;
using UnityEngine.UI;

public sealed class GrenadeCooldownUI : MonoBehaviour
{
    [SerializeField] private PlayerGrenadeController grenadeController;
    [SerializeField] private Image cooldownFill;
    [SerializeField] private Text countdownText;
    [SerializeField] private Image grenadeIcon;
    [SerializeField] private Button actionButton;

    public void Configure(PlayerGrenadeController controller, Image fill, Text countdown, Image icon, Button button)
    {
        grenadeController = controller;
        cooldownFill = fill;
        countdownText = countdown;
        grenadeIcon = icon;
        actionButton = button;
    }

    private void Update()
    {
        if (grenadeController == null) return;
        float remaining = grenadeController.CooldownRemaining;
        bool ready = grenadeController.CanThrow;
        if (cooldownFill != null)
        {
            cooldownFill.fillAmount = grenadeController.CooldownNormalized;
            cooldownFill.enabled = !ready;
        }
        if (countdownText != null)
        {
            countdownText.text = ready ? string.Empty : Mathf.CeilToInt(remaining).ToString();
            countdownText.enabled = !ready;
        }
        if (grenadeIcon != null)
            grenadeIcon.color = ready ? Color.white : new Color(0.55f, 0.55f, 0.55f, 0.78f);
        if (actionButton != null)
            actionButton.interactable = ready;
    }
}
