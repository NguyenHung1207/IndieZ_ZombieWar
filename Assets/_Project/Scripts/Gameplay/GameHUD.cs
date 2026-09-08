using UnityEngine;
using UnityEngine.UI;

public sealed class GameHUD : MonoBehaviour
{
    [SerializeField] private Text healthText;
    [SerializeField] private Text timerText;
    [SerializeField] private Text weaponText;
    [SerializeField] private Text ammoText;
    [SerializeField] private Image healthFill;
    [SerializeField] private Image damageFlash;
    [SerializeField] private Text resultText;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private GameObject mobileControls;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    private GameSession session;
    private PlayerHealth playerHealth;
    private PlayerWeaponController weaponController;
    private GameObject actionControls;
    private float damageFlashAlpha;
    private const float DamageFlashFadeSpeed = 3.5f;

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
            weaponController.AmmoChanged += HandleAmmoChanged;
            HandleWeaponChanged(weaponController.EquippedWeapon);
            HandleAmmoChanged(weaponController.EquippedWeapon, weaponController.CurrentAmmo, weaponController.CurrentMagazineSize, weaponController.IsReloading);
        }
        if (playerHealth != null)
            playerHealth.Damaged += HandlePlayerDamaged;
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
        if (damageFlash != null)
        {
            damageFlashAlpha = Mathf.MoveTowards(damageFlashAlpha, 0f, DamageFlashFadeSpeed * Time.deltaTime);
            Color color = damageFlash.color;
            color.a = damageFlashAlpha;
            damageFlash.color = color;
        }
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

    private void HandleAmmoChanged(WeaponDefinition definition, int currentAmmo, int magazineSize, bool isReloading)
    {
        if (ammoText == null)
            return;

        ammoText.text = definition == null
            ? string.Empty
            : isReloading
                ? string.Format("{0}/{1}  RELOADING...", currentAmmo, magazineSize)
                : string.Format("{0}/{1}", currentAmmo, magazineSize);
    }

    private void HandlePlayerDamaged(float currentHealth, float maxHealth, float damageAmount)
    {
        damageFlashAlpha = Mathf.Max(damageFlashAlpha, 0.62f);
    }

    private void OnDestroy()
    {
        if (session != null)
            session.StateChanged -= HandleStateChanged;
        if (weaponController != null)
        {
            weaponController.WeaponChanged -= HandleWeaponChanged;
            weaponController.AmmoChanged -= HandleAmmoChanged;
        }
        if (playerHealth != null)
            playerHealth.Damaged -= HandlePlayerDamaged;
        if (restartButton != null && session != null)
            restartButton.onClick.RemoveListener(session.RestartLevel);
        if (mainMenuButton != null && session != null)
            mainMenuButton.onClick.RemoveListener(session.ReturnToMainMenu);
    }
}
