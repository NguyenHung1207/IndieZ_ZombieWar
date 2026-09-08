using UnityEngine;
using UnityEngine.UI;

public sealed class GameHUD : MonoBehaviour
{
    [SerializeField] private Text healthText;
    [SerializeField] private Text timerText;
    [SerializeField] private Text weaponText;
    [SerializeField] private Image healthFill;
    [SerializeField] private Text resultText;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private GameObject mobileControls;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    private GameSession session;
    private PlayerHealth playerHealth;
    private PlayerWeaponController weaponController;
    private GameObject actionControls;

    private void Start()
    {
        session = GameSession.Instance;
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        weaponController = FindFirstObjectByType<PlayerWeaponController>();
        if (mobileControls != null && mobileControls.transform.parent != null)
        {
            Transform actionControlsTransform = mobileControls.transform.parent.Find("ActionControls");
            actionControls = actionControlsTransform != null ? actionControlsTransform.gameObject : null;
        }
        if (session != null)
            session.StateChanged += HandleStateChanged;
        if (restartButton != null && session != null)
            restartButton.onClick.AddListener(session.RestartLevel);
        if (mainMenuButton != null && session != null)
            mainMenuButton.onClick.AddListener(session.ReturnToMainMenu);
        if (weaponController != null)
        {
            weaponController.WeaponChanged += HandleWeaponChanged;
            HandleWeaponChanged(weaponController.EquippedWeapon);
        }
        HandleStateChanged(session != null ? session.State : GameSessionState.Playing);
    }

    private void Update()
    {
        if (session != null && timerText != null)
        {
            int totalSeconds = Mathf.CeilToInt(session.RemainingTime);
            timerText.text = string.Format("TIME  {0:00}:{1:00}", totalSeconds / 60, totalSeconds % 60);
        }
        if (playerHealth != null && healthText != null)
            healthText.text = string.Format("HP  {0:000}", Mathf.CeilToInt(playerHealth.CurrentHealth));
        if (playerHealth != null && healthFill != null)
            healthFill.fillAmount = playerHealth.MaxHealth > 0f ? playerHealth.CurrentHealth / playerHealth.MaxHealth : 0f;
    }

    private void HandleStateChanged(GameSessionState state)
    {
        bool ended = state != GameSessionState.Playing;
        if (resultPanel != null)
            resultPanel.SetActive(ended);
        if (mobileControls != null)
            mobileControls.SetActive(!ended);
        if (actionControls != null)
            actionControls.SetActive(!ended);
        if (resultText != null)
            resultText.text = state == GameSessionState.Victory ? "VICTORY" : "GAME OVER";
    }

    private void HandleWeaponChanged(WeaponDefinition definition)
    {
        if (weaponText != null)
            weaponText.text = definition != null ? definition.DisplayName.ToUpperInvariant() : "WEAPON";
    }

    private void OnDestroy()
    {
        if (session != null)
            session.StateChanged -= HandleStateChanged;
        if (weaponController != null)
            weaponController.WeaponChanged -= HandleWeaponChanged;
        if (restartButton != null && session != null)
            restartButton.onClick.RemoveListener(session.RestartLevel);
        if (mainMenuButton != null && session != null)
            mainMenuButton.onClick.RemoveListener(session.ReturnToMainMenu);
    }
}
