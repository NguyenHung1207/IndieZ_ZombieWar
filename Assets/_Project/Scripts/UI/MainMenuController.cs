using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    private GameObject mainMenuPanel;
    private GameObject levelSelectPanel;
    private GameObject settingsPanel;

    private void Awake()
    {
        if (playButton != null)
            playButton.onClick.AddListener(PlayGame);
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    public void PlayGame()
    {
        ShowLevelSelect();
    }

    public void QuitGame()
    {
#if !UNITY_EDITOR
        Application.Quit();
#endif
    }

    private void Start()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
            return;

        Transform safeArea = canvas.transform.Find("SafeArea");
        if (safeArea == null)
            return;

        Transform menu = safeArea.Find("MenuPanel");
        mainMenuPanel = menu != null ? menu.gameObject : null;
        BuildSettingsButton();
        levelSelectPanel = BuildLevelSelect(safeArea);
        levelSelectPanel.SetActive(false);
        settingsPanel = BuildSettings(safeArea);
        settingsPanel.SetActive(false);
        UiTheme.ApplyRuntimeMainMenuPanels(safeArea);
    }

    private void ShowLevelSelect()
    {
        if (levelSelectPanel == null)
            return;
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
    }

    private void HideLevelSelect()
    {
        if (levelSelectPanel != null)
            levelSelectPanel.SetActive(false);
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
    }

    private void BuildSettingsButton()
    {
        if (mainMenuPanel == null || mainMenuPanel.transform.Find("SettingsButton") != null)
            return;
        Button settings = CreateButton(mainMenuPanel.transform, "SETTINGS", new Vector2(0f, -168f), new Vector2(390f, 72f), new Color(0.13f, 0.16f, 0.18f, 0.96f), 28);
        settings.gameObject.name = "SettingsButton";
        Button quit = quitButton;
        if (quit != null)
            quit.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -258f);
        settings.onClick.AddListener(() => { mainMenuPanel.SetActive(false); settingsPanel.SetActive(true); });
    }

    private GameObject BuildSettings(Transform safeArea)
    {
        GameObject panel = new GameObject("SettingsPanel", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(safeArea, false);
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = rect.pivot = Vector2.one * 0.5f;
        rect.sizeDelta = new Vector2(620f, 400f);
        panel.GetComponent<Image>().color = new Color(0.025f, 0.035f, 0.04f, 0.96f);
        CreateText("SETTINGS", panel.transform, new Vector2(0f, 130f), new Vector2(500f, 64f), 46, Color.white).fontStyle = FontStyle.Bold;
        bool music = PlayerPrefs.GetInt("ZombieWar.MusicEnabled", 1) == 1;
        bool sfx = PlayerPrefs.GetInt("ZombieWar.SfxEnabled", 1) == 1;
        CreateText("MUSIC", panel.transform, new Vector2(-116f, 48f), new Vector2(190f, 58f), 25, Color.white).alignment = TextAnchor.MiddleLeft;
        CreateText("SFX", panel.transform, new Vector2(-116f, -30f), new Vector2(190f, 58f), 25, Color.white).alignment = TextAnchor.MiddleLeft;
        Button musicButton = CreateButton(panel.transform, music ? "ON" : "OFF", new Vector2(122f, 48f), new Vector2(180f, 58f), new Color(0.13f, 0.16f, 0.18f, 0.96f), 23);
        Button sfxButton = CreateButton(panel.transform, sfx ? "ON" : "OFF", new Vector2(122f, -30f), new Vector2(180f, 58f), new Color(0.13f, 0.16f, 0.18f, 0.96f), 23);
        musicButton.onClick.AddListener(() => ToggleSetting("ZombieWar.MusicEnabled", musicButton));
        sfxButton.onClick.AddListener(() => ToggleSetting("ZombieWar.SfxEnabled", sfxButton));
        Button back = CreateButton(panel.transform, "BACK", new Vector2(0f, -128f), new Vector2(280f, 60f), new Color(0.075f, 0.085f, 0.095f, 0.96f), 24);
        back.onClick.AddListener(() => { panel.SetActive(false); mainMenuPanel.SetActive(true); });
        return panel;
    }

    private static void ToggleSetting(string key, Button button)
    {
        bool enabled = PlayerPrefs.GetInt(key, 1) == 0;
        PlayerPrefs.SetInt(key, enabled ? 1 : 0);
        PlayerPrefs.Save();
        Text text = button.GetComponentInChildren<Text>();
        if (text != null) text.text = enabled ? "ON" : "OFF";
    }

    private GameObject BuildLevelSelect(Transform safeArea)
    {
        GameObject panel = new GameObject("LevelSelectPanel", typeof(RectTransform), typeof(Image), typeof(Outline));
        panel.transform.SetParent(safeArea, false);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = panelRect.anchorMax = panelRect.pivot = Vector2.one * 0.5f;
        panelRect.sizeDelta = new Vector2(820f, 620f);
        Image panelImage = panel.GetComponent<Image>();
        panelImage.color = new Color(0.025f, 0.035f, 0.04f, 0.96f);
        Outline outline = panel.GetComponent<Outline>();
        outline.effectColor = new Color(0.32f, 0.035f, 0.035f, 0.9f);
        outline.effectDistance = new Vector2(3f, -3f);

        CreateText("LEVEL SELECT", panel.transform, new Vector2(0f, 228f), new Vector2(720f, 76f), 50, Color.white);
        CreateLevelButton(panel.transform, "LEVEL 1", "FLAT BATTLEFIELD   •   SURVIVE 3 MINUTES", new Vector2(0f, 92f), new Color(0.70f, 0.055f, 0.055f, 0.94f), "Gameplay_Level01");
        CreateLevelButton(panel.transform, "LEVEL 2", "HILL ASSAULT   •   SLOPES + GIANT ZOMBIE", new Vector2(0f, -76f), new Color(0.13f, 0.16f, 0.18f, 0.98f), "Gameplay_Level02");
        Button back = CreateButton(panel.transform, "BACK", new Vector2(0f, -238f), new Vector2(280f, 62f), new Color(0.075f, 0.085f, 0.095f, 0.96f), 24);
        back.onClick.AddListener(HideLevelSelect);
        return panel;
    }

    private void CreateLevelButton(Transform parent, string title, string description, Vector2 position, Color color, string sceneName)
    {
        Button button = CreateButton(parent, title, position, new Vector2(620f, 132f), color, 32);
        Text label = button.GetComponentInChildren<Text>();
        label.rectTransform.anchoredPosition = new Vector2(0f, 24f);
        label.rectTransform.sizeDelta = new Vector2(540f, 55f);
        Text detail = CreateText(description, button.transform, new Vector2(38f, -27f), new Vector2(470f, 42f), 20, new Color(0.90f, 0.92f, 0.94f));
        detail.fontStyle = FontStyle.Bold;
        if (sceneName == "Gameplay_Level02")
        {
            Text bonus = CreateText("BONUS", button.transform, new Vector2(226f, 38f), new Vector2(104f, 30f), 17, new Color(1f, 0.72f, 0.14f));
            bonus.fontStyle = FontStyle.Bold;
        }
        button.onClick.AddListener(() => SceneManager.LoadScene(sceneName));
    }

    private static Button CreateButton(Transform parent, string label, Vector2 position, Vector2 size, Color color, int fontSize)
    {
        GameObject buttonObject = new GameObject(label + "Button", typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = rect.pivot = Vector2.one * 0.5f;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        Image image = buttonObject.GetComponent<Image>();
        image.color = color;
        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        ColorBlock colors = button.colors;
        colors.highlightedColor = Color.white;
        colors.pressedColor = new Color(0.76f, 0.76f, 0.76f, 1f);
        colors.fadeDuration = 0.08f;
        button.colors = colors;
        CreateText(label, buttonObject.transform, Vector2.zero, size, fontSize, Color.white).fontStyle = FontStyle.Bold;
        return button;
    }

    private static Text CreateText(string value, Transform parent, Vector2 position, Vector2 size, int fontSize, Color color)
    {
        GameObject textObject = new GameObject("Text", typeof(RectTransform), typeof(Text));
        textObject.transform.SetParent(parent, false);
        Text text = textObject.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = value;
        text.fontSize = fontSize;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = color;
        text.raycastTarget = false;
        RectTransform rect = text.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = Vector2.one * 0.5f;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        return text;
    }

    private void OnDestroy()
    {
        if (playButton != null)
            playButton.onClick.RemoveListener(PlayGame);
        if (quitButton != null)
            quitButton.onClick.RemoveListener(QuitGame);
    }
}
