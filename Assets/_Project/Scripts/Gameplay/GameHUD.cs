using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

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
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private Text resultCoinsText;
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
        UiTheme.ApplyGameplay(transform, this);
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
            restartButton.onClick.AddListener(HandlePrimaryResultAction);
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
        ShowLevelIntro();
    }

    public void ConfigurePresentation(GameObject pauseControl, Text coinsResult)
    {
        pauseButton = pauseControl;
        resultCoinsText = coinsResult;
    }

    private void Update()
    {
        if (session != null && timerText != null)
        {
            int totalSeconds = Mathf.CeilToInt(session.RemainingTime);
            timerText.text = string.Format("{0:00}:{1:00}", totalSeconds / 60, totalSeconds % 60);
            timerText.color = totalSeconds <= 30 ? new Color(1f, 0.36f, 0.28f) : Color.white;
        }
        if (playerHealth != null && healthText != null)
            healthText.text = string.Format("{0:0} / {1:0}", Mathf.CeilToInt(playerHealth.CurrentHealth), Mathf.CeilToInt(playerHealth.MaxHealth));
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
        if (pauseButton != null)
            pauseButton.SetActive(!ended);
        if (resultText != null)
            resultText.text = state == GameSessionState.Victory ? "VICTORY" : "GAME OVER";
        if (restartButton != null)
        {
            Text label = restartButton.GetComponentInChildren<Text>(true);
            if (label != null)
                label.text = state == GameSessionState.Victory && SceneManager.GetActiveScene().name == "Gameplay_Level01"
                    ? "NEXT LEVEL" : "RESTART";
        }
        if (ended && resultCoinsText != null)
        {
            CurrencyWallet wallet = FindFirstObjectByType<CurrencyWallet>();
            resultCoinsText.text = wallet != null ? "TOTAL COINS  " + wallet.Coins : "TOTAL COINS  0";
        }
    }

    private void HandlePrimaryResultAction()
    {
        if (session != null && session.State == GameSessionState.Victory && SceneManager.GetActiveScene().name == "Gameplay_Level01")
            SceneManager.LoadScene("Gameplay_Level02");
        else
            session?.RestartLevel();
    }

    private void ShowLevelIntro()
    {
        Transform safe = transform.Find("SafeArea");
        if (safe == null)
            return;
        GameObject banner = new GameObject("LevelIntro", typeof(RectTransform), typeof(CanvasGroup), typeof(Text));
        banner.transform.SetParent(safe, false);
        Text text = banner.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontStyle = FontStyle.Bold;
        text.fontSize = 46;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.text = SceneManager.GetActiveScene().name == "Gameplay_Level02"
            ? "LEVEL 2\nHILL ASSAULT\nSURVIVE 03:00"
            : "LEVEL 1\nFLAT BATTLEFIELD\nSURVIVE 03:00";
        RectTransform rect = text.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, 160f);
        rect.sizeDelta = new Vector2(620f, 170f);
        StartCoroutine(FadeLevelIntro(banner, banner.GetComponent<CanvasGroup>()));
    }

    private static IEnumerator FadeLevelIntro(GameObject banner, CanvasGroup group)
    {
        const float holdDuration = 1.75f;
        const float fadeDuration = 0.55f;
        yield return new WaitForSecondsRealtime(holdDuration);
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            if (group != null) group.alpha = 1f - Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }
        Destroy(banner);
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
            restartButton.onClick.RemoveListener(HandlePrimaryResultAction);
        if (mainMenuButton != null && session != null)
            mainMenuButton.onClick.RemoveListener(session.ReturnToMainMenu);
    }
}
