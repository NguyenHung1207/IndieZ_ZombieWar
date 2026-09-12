using System;
using UnityEngine;
using UnityEngine.UI;

public static class M20UITheme
{
    private static readonly Font UiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
    private static readonly Color Panel = new Color(0.055f, 0.068f, 0.078f, 0.94f);
    private static readonly Color Red = new Color(0.70f, 0.055f, 0.055f, 0.94f);
    private static readonly Color OrangeRed = new Color(0.82f, 0.22f, 0.08f, 0.94f);
    private static readonly Color Gold = new Color(1f, 0.72f, 0.14f, 1f);
    private static readonly Color Green = new Color(0.18f, 0.78f, 0.34f, 1f);
    private static readonly Color Muted = new Color(0.72f, 0.76f, 0.78f, 1f);

    public static void ApplyGameplay(Transform hud, GameHUD gameHud)
    {
        Transform safe = hud.Find("SafeArea");
        if (safe == null || safe.Find("M20ThemeApplied") != null) return;
        ConfigureCanvas(hud.GetComponent<Canvas>());
        MarkApplied(safe);

        Image damageFlash = GetImage(hud, "DamageFlash");
        if (damageFlash != null) damageFlash.raycastTarget = false;

        Transform topBackdrop = EnsureUI(safe, "TopHUDBackdrop", typeof(Image));
        SetRect((RectTransform)topBackdrop, Vector2.up, Vector2.one, new Vector2(0.5f, 1f), Vector2.zero, new Vector2(0f, 118f));
        StyleImage(topBackdrop.GetComponent<Image>(), null, new Color(0.018f, 0.024f, 0.027f, 0.72f));
        topBackdrop.GetComponent<Image>().preserveAspect = false;
        topBackdrop.SetAsFirstSibling();

        Text health = GetText(safe, "HealthText");
        StyleText(health, 28, TextAnchor.MiddleLeft, Color.white, FontStyle.Bold);
        SetRect(health.rectTransform, Vector2.up, Vector2.up, Vector2.up, new Vector2(72f, -20f), new Vector2(235f, 48f));
        Image healthIcon = CreateIcon(safe, "HealthIcon", ShooterSprite("Health") ?? Sprite("Health"), Green);
        SetRect(healthIcon.rectTransform, Vector2.up, Vector2.up, Vector2.up, new Vector2(28f, -25f), new Vector2(38f, 38f));

        Image healthBar = GetImage(safe, "HealthBarBackground");
        StyleImage(healthBar, null, new Color(0.02f, 0.025f, 0.03f, 0.88f));
        SetRect(healthBar.rectTransform, Vector2.up, Vector2.up, Vector2.up, new Vector2(28f, -76f), new Vector2(280f, 18f));
        Image healthFill = GetImage(healthBar.transform, "HealthFill");
        StyleImage(healthFill, null, Green);

        Text timer = GetText(safe, "TimerText");
        StyleText(timer, 38, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
        SetRect(timer.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -18f), new Vector2(210f, 54f));

        Text weapon = GetText(safe, "WeaponText");
        StyleText(weapon, 26, TextAnchor.MiddleRight, Color.white, FontStyle.Bold);
        SetRect(weapon.rectTransform, Vector2.one, Vector2.one, Vector2.one, new Vector2(-132f, -18f), new Vector2(330f, 42f));
        Text ammo = GetText(safe, "AmmoText");
        StyleText(ammo, 26, TextAnchor.MiddleRight, new Color(0.92f, 0.94f, 0.96f), FontStyle.Bold);
        SetRect(ammo.rectTransform, Vector2.one, Vector2.one, Vector2.one, new Vector2(-132f, -54f), new Vector2(330f, 40f));

        Transform currencyCluster = EnsureUI(safe, "CurrencyCluster", typeof(Image));
        SetRect((RectTransform)currencyCluster, Vector2.one, Vector2.one, Vector2.one, new Vector2(-132f, -86f), new Vector2(132f, 38f));
        StyleImage(currencyCluster.GetComponent<Image>(), null, new Color(0.02f, 0.025f, 0.03f, 0.72f));
        Image coinIcon = CreateIcon(currencyCluster, "CoinIcon", "Coin", Gold);
        SetRect(coinIcon.rectTransform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(8f, 0f), new Vector2(28f, 28f));
        Text currency = EnsureText(currencyCluster, "CurrencyValue");
        StyleText(currency, 24, TextAnchor.MiddleRight, Gold, FontStyle.Bold);
        Stretch(currency.rectTransform, new Vector2(40f, 0f), new Vector2(-10f, 0f));
        CurrencyHUD currencyHud = UnityEngine.Object.FindFirstObjectByType<CurrencyHUD>();
        if (currencyHud == null) currencyHud = currencyCluster.gameObject.AddComponent<CurrencyHUD>();
        currencyHud.Configure(currency);

        ConfigureJoystick(safe);
        ConfigureActions(safe);
        ConfigurePause(safe);
        Text resultCoins = ConfigureResult(safe);
        Transform pauseButton = safe.Find("PauseButton");
        gameHud.ConfigurePresentation(pauseButton != null ? pauseButton.gameObject : null, resultCoins);
    }

    public static void ApplyMainMenu(ShopController shopController)
    {
        Canvas canvas = UnityEngine.Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) return;
        Transform safe = canvas.transform.Find("SafeArea");
        if (safe == null || safe.Find("M20ThemeApplied") != null) return;
        ConfigureCanvas(canvas);
        MarkApplied(safe);
        Image safeBackdrop = safe.GetComponent<Image>();
        if (safeBackdrop != null)
        {
            safeBackdrop.color = Color.clear;
            safeBackdrop.raycastTarget = false;
        }

        Transform menu = safe.Find("MenuPanel");
        if (menu == null) return;
        SetRect((RectTransform)menu, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f, new Vector2(-470f, 0f), new Vector2(650f, 880f));
        StylePanel(menu, null, new Color(0.018f, 0.024f, 0.028f, 0.86f));
        Outline menuOutline = GetOrAdd<Outline>(menu.gameObject);
        menuOutline.effectColor = new Color(0.32f, 0.035f, 0.035f, 0.9f);
        menuOutline.effectDistance = new Vector2(3f, -3f);
        Image accent = GetImage(menu, "Accent");
        StyleImage(accent, null, Gold);
        SetRect(accent.rectTransform, Vector2.zero, Vector2.up, new Vector2(0f, 0.5f), new Vector2(0f, 0f), new Vector2(7f, 0f));

        Text title = GetText(menu, "Title");
        StyleText(title, 76, TextAnchor.MiddleLeft, Color.white, FontStyle.Bold);
        SetRect(title.rectTransform, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f, new Vector2(28f, 320f), new Vector2(530f, 92f));
        Text subtitle = GetText(menu, "Subtitle");
        StyleText(subtitle, 24, TextAnchor.MiddleLeft, Muted, FontStyle.Bold);
        SetRect(subtitle.rectTransform, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f, new Vector2(28f, 260f), new Vector2(530f, 42f));
        ConfigureMenuButton(menu.Find("PlayButton"), new Vector2(28f, 154f), Red, 33, "Play", new Vector2(500f, 86f));
        ConfigureMenuButton(menu.Find("ShopButton"), new Vector2(28f, 50f), new Color(0.13f, 0.16f, 0.18f, 0.96f), 29, "Shop", new Vector2(500f, 76f));
        ConfigureMenuButton(menu.Find("QuitButton"), new Vector2(28f, -158f), new Color(0.075f, 0.085f, 0.095f, 0.82f), 25, "Quit", new Vector2(500f, 68f));
        Text footer = GetText(menu, "Footer");
        StyleText(footer, 18, TextAnchor.MiddleCenter, new Color(0.58f, 0.62f, 0.64f), FontStyle.Normal);
        SetRect(footer.rectTransform, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f, new Vector2(28f, -350f), new Vector2(530f, 30f));

        Text mainCoins = GetText(safe, "CurrencyText");
        Transform menuCurrency = EnsureUI(safe, "MenuCurrencyPanel", typeof(Image));
        SetRect((RectTransform)menuCurrency, Vector2.one, Vector2.one, Vector2.one, new Vector2(-28f, -24f), new Vector2(160f, 46f));
        StyleImage(menuCurrency.GetComponent<Image>(), null, new Color(0.025f, 0.035f, 0.04f, 0.84f));
        mainCoins.transform.SetParent(menuCurrency, false);
        StyleText(mainCoins, 24, TextAnchor.MiddleRight, Gold, FontStyle.Bold);
        Stretch(mainCoins.rectTransform, new Vector2(45f, 0f), new Vector2(-12f, 0f));
        Image menuCoin = CreateIcon(menuCurrency, "CoinIcon", "Coin", Gold);
        SetRect(menuCoin.rectTransform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(10f, 0f), new Vector2(30f, 30f));
        CurrencyHUD currencyHud = UnityEngine.Object.FindFirstObjectByType<CurrencyHUD>();
        if (currencyHud == null) currencyHud = menuCurrency.gameObject.AddComponent<CurrencyHUD>();
        currencyHud.Configure(mainCoins);

        ConfigureShop(safe, shopController);
    }

    public static void ApplyRuntimeMainMenuPanels(Transform safe)
    {
        if (safe == null) return;
        Transform menu = safe.Find("MenuPanel");
        ConfigureMenuButton(menu != null ? menu.Find("SettingsButton") : null, new Vector2(28f, -50f), new Color(0.13f, 0.16f, 0.18f, 0.96f), 28, "Settings", new Vector2(500f, 76f));
        ConfigureMenuButton(menu != null ? menu.Find("QuitButton") : null, new Vector2(28f, -150f), new Color(0.075f, 0.085f, 0.095f, 0.82f), 25, "Quit", new Vector2(500f, 68f));

        Transform levelSelect = safe.Find("LevelSelectPanel");
        if (levelSelect != null)
        {
            StylePanel(levelSelect, "PanelFrame", Color.white);
            AddTacticalShade(levelSelect);
            AddHeaderSeparator(levelSelect);
            StyleRuntimeButton(levelSelect.Find("LEVEL 1Button"), "Level1", new Color(0.86f, 0.92f, 1f, 1f));
            StyleRuntimeButton(levelSelect.Find("LEVEL 2Button"), "Level2", new Color(1f, 0.86f, 0.82f, 1f));
            StyleRuntimeButton(levelSelect.Find("BACKButton"), null, Color.white);
        }

        Transform settings = safe.Find("SettingsPanel");
        if (settings != null)
        {
            StylePanel(settings, "PanelFrame", Color.white);
            AddTacticalShade(settings);
            AddHeaderSeparator(settings);
            foreach (Button button in settings.GetComponentsInChildren<Button>(true))
                StyleButton(button, new Color(0.13f, 0.16f, 0.18f, 0.96f), false);
        }
    }

    private static void ConfigureJoystick(Transform safe)
    {
        Transform joystickRoot = safe.Find("MobileControls/JoystickBackground");
        if (joystickRoot == null) return;
        SetRect((RectTransform)joystickRoot, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(34f, 34f), new Vector2(204f, 204f));
        Image baseImage = joystickRoot.GetComponent<Image>();
        StyleImage(baseImage, Sprite("CircleRing"), new Color(0.08f, 0.09f, 0.11f, 0.48f));
        // This Graphic is the pointer surface for VirtualJoystick. StyleImage
        // disables raycasts for decorative images, so restore it here only.
        baseImage.raycastTarget = true;
        Transform handle = joystickRoot.Find("JoystickHandle");
        SetRect((RectTransform)handle, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.zero, new Vector2(82f, 82f));
        Image handleImage = handle.GetComponent<Image>();
        StyleImage(handleImage, Sprite("CircleFill"), new Color(0.78f, 0.80f, 0.82f, 0.72f));
        handleImage.raycastTarget = false;
        JoystickVisualFeedback feedback = GetOrAdd<JoystickVisualFeedback>(joystickRoot.gameObject);
        feedback.Configure(joystickRoot.GetComponent<VirtualJoystick>(), baseImage, handleImage);
    }

    private static void ConfigureActions(Transform safe)
    {
        Transform actions = safe.Find("ActionControls");
        if (actions == null) return;
        SetRect((RectTransform)actions, new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-26f, 26f), new Vector2(450f, 292f));
        ConfigureActionButton(actions.Find("FireButton"), "Fire", Red, new Vector2(288f, 0f), 150f, string.Empty);
        ConfigureActionButton(actions.Find("GrenadeButton"), "Grenade", OrangeRed, new Vector2(96f, 18f), 115f, string.Empty);
        ConfigureActionButton(actions.Find("ReloadButton"), "Reload", new Color(0.12f, 0.15f, 0.17f, 0.9f), new Vector2(208f, 145f), 100f, string.Empty);
        ConfigureActionButton(actions.Find("SwitchButton"), "SwitchWeapon", new Color(0.12f, 0.15f, 0.17f, 0.9f), new Vector2(326f, 182f), 100f, string.Empty);

        PlayerGrenadeController grenades = UnityEngine.Object.FindFirstObjectByType<PlayerGrenadeController>();
        Transform grenadeTransform = actions.Find("GrenadeButton");
        Image grenadeIcon = GetImage(grenadeTransform, "Icon");
        Image cooldown = CreateIcon(grenadeTransform, "CooldownFill", "CircleFill", new Color(0.01f, 0.015f, 0.02f, 0.78f));
        cooldown.type = Image.Type.Filled;
        cooldown.fillMethod = Image.FillMethod.Radial360;
        cooldown.fillOrigin = 2;
        cooldown.fillClockwise = false;
        Stretch(cooldown.rectTransform, new Vector2(4f, 4f), new Vector2(-4f, -4f));
        Text countdown = EnsureText(grenadeTransform, "CooldownText");
        StyleText(countdown, 30, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
        Stretch(countdown.rectTransform, Vector2.zero, Vector2.zero);
        GrenadeCooldownUI cooldownUi = GetOrAdd<GrenadeCooldownUI>(grenadeTransform.gameObject);
        cooldownUi.Configure(grenades, cooldown, countdown, grenadeIcon, grenadeTransform.GetComponent<Button>());

        Transform reloadTransform = actions.Find("ReloadButton");
        ReloadButtonUI reloadUi = GetOrAdd<ReloadButtonUI>(reloadTransform.gameObject);
        reloadUi.Configure(UnityEngine.Object.FindFirstObjectByType<PlayerWeaponController>(), GetImage(reloadTransform, "Icon"), reloadTransform.GetComponent<Button>());
    }

    private static void ConfigureActionButton(Transform transform, string iconName, Color background, Vector2 position, float size, string labelText)
    {
        if (transform == null) return;
        SetRect((RectTransform)transform, Vector2.zero, Vector2.zero, Vector2.zero, position, Vector2.one * size);
        StyleButton(transform.GetComponent<Button>(), background, true);
        Image ring = CreateIcon(transform, "Ring", Sprite("CircleRing"), new Color(background.r, background.g, background.b, 0.72f));
        Stretch(ring.rectTransform, new Vector2(3f, 3f), new Vector2(-3f, -3f));
        ring.raycastTarget = false;
        ring.transform.SetAsFirstSibling();
        Image icon = CreateIcon(transform, "Icon", ShooterSprite(iconName) ?? Sprite(iconName), Color.white);
        SetRect(icon.rectTransform, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.zero, Vector2.one * (size * 0.47f));
        Text label = GetText(transform, "Label");
        if (label == null) label = GetText(transform, "Text");
        if (label != null)
        {
            label.gameObject.SetActive(!string.IsNullOrEmpty(labelText));
            if (!string.IsNullOrEmpty(labelText))
            {
                label.text = labelText;
                StyleText(label, 17, TextAnchor.LowerCenter, Color.white, FontStyle.Bold);
                Stretch(label.rectTransform, new Vector2(0f, 10f), new Vector2(0f, -8f));
            }
        }
    }

    private static void ConfigurePause(Transform safe)
    {
        Transform pauseButton = safe.Find("PauseButton");
        if (pauseButton == null) return;
        SetRect((RectTransform)pauseButton, Vector2.one, Vector2.one, Vector2.one, new Vector2(-28f, -24f), new Vector2(82f, 82f));
        StyleButton(pauseButton.GetComponent<Button>(), new Color(0.07f, 0.085f, 0.095f, 0.92f), true);
        Image icon = CreateIcon(pauseButton, "Icon", "Pause", Color.white);
        SetRect(icon.rectTransform, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.zero, new Vector2(42f, 42f));
        Text legacyText = GetText(pauseButton, "Text");
        if (legacyText != null) legacyText.gameObject.SetActive(false);

        Transform pausePanel = safe.Find("PausePanel");
        if (pausePanel == null) return;
        pausePanel.GetComponent<Image>().color = new Color(0.01f, 0.012f, 0.015f, 0.78f);
        Transform card = EnsureUI(pausePanel, "PauseCard", typeof(Image), typeof(Outline));
        SetRect((RectTransform)card, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.zero, new Vector2(520f, 370f));
        StylePanel(card, "WidePanelFrame", Color.white);
        Outline outline = card.GetComponent<Outline>();
        outline.effectColor = new Color(0.65f, 0.07f, 0.07f, 0.9f);
        outline.effectDistance = new Vector2(3f, -3f);
        card.SetAsFirstSibling();

        Transform titleTransform = pausePanel.Find("PauseTitle");
        if (titleTransform != null) titleTransform.SetParent(card, false); else titleTransform = card.Find("PauseTitle");
        Text title = titleTransform.GetComponent<Text>();
        StyleText(title, 46, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
        SetRect(title.rectTransform, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f, new Vector2(0f, 105f), new Vector2(450f, 70f));
        ConfigurePauseButton(pausePanel, card, "ResumeButton", new Vector2(0f, 10f), Red, 26);
        ConfigurePauseButton(pausePanel, card, "MainMenuButton", new Vector2(0f, -86f), new Color(0.14f, 0.17f, 0.19f, 0.96f), 24);
    }

    private static void ConfigurePauseButton(Transform panel, Transform card, string name, Vector2 position, Color color, int fontSize)
    {
        Transform button = panel.Find(name);
        if (button != null) button.SetParent(card, false); else button = card.Find(name);
        SetRect((RectTransform)button, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f, position, new Vector2(300f, 70f));
        StyleButton(button.GetComponent<Button>(), color, false);
        StyleButtonLabel(button, fontSize);
    }

    private static Text ConfigureResult(Transform safe)
    {
        Transform result = safe.Find("ResultPanel");
        if (result == null) return null;
        SetRect((RectTransform)result, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.zero, new Vector2(680f, 410f));
        StylePanel(result, "WidePanelFrame", Color.white);
        Outline outline = GetOrAdd<Outline>(result.gameObject);
        outline.effectColor = new Color(0.66f, 0.07f, 0.07f, 0.9f);
        outline.effectDistance = new Vector2(3f, -3f);
        Image accent = CreateIcon(result, "Accent", "Solid", Red);
        SetRect(accent.rectTransform, new Vector2(0f, 1f), Vector2.one, new Vector2(0.5f, 1f), Vector2.zero, new Vector2(0f, 8f));
        Text title = GetText(result, "ResultText");
        StyleText(title, 68, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
        SetRect(title.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -52f), new Vector2(620f, 100f));
        Text coins = EnsureText(result, "ResultCoinsText");
        coins.text = "TOTAL COINS  0";
        StyleText(coins, 25, TextAnchor.MiddleCenter, Gold, FontStyle.Bold);
        SetRect(coins.rectTransform, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f, new Vector2(14f, 8f), new Vector2(360f, 44f));
        Image coin = CreateIcon(result, "ResultCoinIcon", "Coin", Gold);
        SetRect(coin.rectTransform, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f, new Vector2(-172f, 8f), new Vector2(30f, 30f));
        ConfigureResultButton(result.Find("RestartButton"), new Vector2(-145f, -115f), Red, 26);
        ConfigureResultButton(result.Find("MainMenuButton"), new Vector2(145f, -115f), new Color(0.14f, 0.17f, 0.19f, 0.96f), 25);
        return coins;
    }

    private static void ConfigureResultButton(Transform transform, Vector2 position, Color color, int fontSize)
    {
        if (transform == null) return;
        SetRect((RectTransform)transform, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f, position, new Vector2(250f, 72f));
        StyleButton(transform.GetComponent<Button>(), color, false);
        StyleButtonLabel(transform, fontSize);
    }

    private static void ConfigureShop(Transform safe, ShopController controller)
    {
        Transform shop = safe.Find("ShopPanel");
        if (shop == null) return;
        shop.GetComponent<Image>().color = new Color(0.012f, 0.018f, 0.022f, 0.97f);
        shop.GetComponent<Image>().raycastTarget = false;
        Transform shopFrame = shop.Find("ShooterPackFrame");
        if (shopFrame != null) shopFrame.gameObject.SetActive(false);
        Transform header = EnsureUI(shop, "Header", typeof(Image));
        SetRect((RectTransform)header, Vector2.up, Vector2.one, new Vector2(0.5f, 1f), Vector2.zero, new Vector2(0f, 88f));
        header.GetComponent<Image>().color = new Color(0.035f, 0.045f, 0.052f, 0.96f);
        header.SetAsFirstSibling();

        Text title = GetText(shop, "ShopTitle");
        StyleText(title, 42, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
        SetRect(title.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -10f), new Vector2(520f, 56f));
        Text feedback = GetText(shop, "ShopFeedback");
        StyleText(feedback, 22, TextAnchor.MiddleCenter, Gold, FontStyle.Bold);
        SetRect(feedback.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -62f), new Vector2(700f, 30f));
        Transform back = shop.Find("BackButton");
        SetRect((RectTransform)back, Vector2.up, Vector2.up, Vector2.up, new Vector2(30f, -22f), new Vector2(150f, 60f));
        StyleButton(back.GetComponent<Button>(), new Color(0.12f, 0.145f, 0.16f, 0.98f), false);
        StyleButtonLabel(back, 23);

        Text shopCoins = GetText(shop, "ShopCoins");
        Transform shopCurrency = EnsureUI(shop, "ShopCurrencyPanel", typeof(Image));
        SetRect((RectTransform)shopCurrency, Vector2.one, Vector2.one, Vector2.one, new Vector2(-30f, -22f), new Vector2(160f, 60f));
        shopCurrency.GetComponent<Image>().color = new Color(0.015f, 0.02f, 0.025f, 0.82f);
        shopCoins.transform.SetParent(shopCurrency, false);
        StyleText(shopCoins, 25, TextAnchor.MiddleRight, Gold, FontStyle.Bold);
        Stretch(shopCoins.rectTransform, new Vector2(45f, 0f), new Vector2(-12f, 0f));
        Image coin = CreateIcon(shopCurrency, "CoinIcon", "Coin", Gold);
        SetRect(coin.rectTransform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(10f, 0f), new Vector2(32f, 32f));

        string[] names = { "Card_Assault Rifle", "Card_Shotgun", "Card_SMG", "Card_Pistol", "Card_Sniper Rifle" };
        string[] labels = { "ASSAULT RIFLE", "SHOTGUN", "SMG", "PISTOL", "SNIPER RIFLE" };
        string[] roles = { "BALANCED AUTOMATIC", "CLOSE-RANGE BURST", "FAST SUSTAINED FIRE", "ACCURATE SIDEARM", "ONE-SHOT PRECISION" };
        Text[] infos = new Text[names.Length];
        Text[] states = new Text[names.Length];
        Image[] backgrounds = new Image[names.Length];
        Image[] accents = new Image[names.Length];
        Image[] priceIcons = new Image[names.Length];
        for (int i = 0; i < names.Length; i++)
        {
            Transform card = shop.Find(names[i]);
            if (card == null) continue;
            SetRect((RectTransform)card, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f, new Vector2((i - 2) * 300f, 78f), new Vector2(282f, 530f));
            backgrounds[i] = card.GetComponent<Image>();
            backgrounds[i].sprite = Sprite("Solid");
            backgrounds[i].color = new Color(0.10f, 0.12f, 0.135f, 0.98f);
            backgrounds[i].preserveAspect = false;
            backgrounds[i].raycastTarget = false;
            Outline cardOutline = GetOrAdd<Outline>(card.gameObject);
            cardOutline.effectColor = new Color(0.18f, 0.20f, 0.22f, 0.9f);
            cardOutline.effectDistance = new Vector2(2f, -2f);
            accents[i] = CreateIcon(card, "StateAccent", "Solid", new Color(0.27f, 0.29f, 0.31f));
            SetRect(accents[i].rectTransform, Vector2.up, Vector2.one, new Vector2(0.5f, 1f), Vector2.zero, new Vector2(0f, 7f));

            Text name = EnsureText(card, "WeaponName");
            name.text = labels[i];
            StyleText(name, 24, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
            SetRect(name.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -18f), new Vector2(250f, 42f));

            Transform preview = card.Find("WeaponPreview");
            Transform frame = EnsureUI(card, "PreviewFrame", typeof(Image), typeof(RectMask2D));
            SetRect((RectTransform)frame, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -64f), new Vector2(264f, 190f));
            frame.GetComponent<Image>().color = new Color(0.006f, 0.009f, 0.012f, 0.98f);
            frame.GetComponent<Image>().raycastTarget = false;
            preview.SetParent(frame, false);
            Image previewImage = preview.GetComponent<Image>();
            previewImage.color = Color.white;
            previewImage.preserveAspect = true;
            previewImage.raycastTarget = false;
            Stretch((RectTransform)preview, new Vector2(-55f, -28f), new Vector2(55f, 28f));

            Text role = EnsureText(card, "Role");
            role.text = roles[i];
            StyleText(role, 16, TextAnchor.MiddleCenter, new Color(0.78f, 0.81f, 0.83f), FontStyle.Bold);
            SetRect(role.rectTransform, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f, new Vector2(0f, 8f), new Vector2(250f, 28f));

            Transform infoTransform = card.Find("Info") ?? card.Find("Stats");
            infoTransform.gameObject.name = "Stats";
            Text info = infoTransform.GetComponent<Text>();
            StyleText(info, 17, TextAnchor.MiddleLeft, new Color(0.78f, 0.82f, 0.85f), FontStyle.Normal);
            SetRect(info.rectTransform, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f, new Vector2(0f, -68f), new Vector2(218f, 126f));
            infos[i] = info;

            Text state = EnsureText(card, "State");
            StyleText(state, 17, TextAnchor.MiddleCenter, Gold, FontStyle.Bold);
            SetRect(state.rectTransform, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f, new Vector2(0f, -143f), new Vector2(240f, 30f));
            states[i] = state;

            Transform buy = card.Find("BuyButton");
            SetRect((RectTransform)buy, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f, new Vector2(0f, -185f), new Vector2(232f, 52f));
            StyleButton(buy.GetComponent<Button>(), Red, false);
            StyleButtonLabel(buy, 20);
            priceIcons[i] = CreateIcon(buy, "PriceIcon", "Coin", Gold);
            SetRect(priceIcons[i].rectTransform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(12f, 0f), new Vector2(24f, 24f));

            Transform equip = card.Find("EquipButton");
            SetRect((RectTransform)equip, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f, new Vector2(0f, -231f), new Vector2(232f, 44f));
            StyleButton(equip.GetComponent<Button>(), new Color(0.14f, 0.17f, 0.19f, 0.98f), false);
            StyleButtonLabel(equip, 18);
        }
        controller.ConfigurePresentation(shopCoins, infos, states, backgrounds, accents, priceIcons);
    }

    private static void ConfigureMenuButton(Transform transform, Vector2 position, Color color, int fontSize, string iconName, Vector2? requestedSize = null)
    {
        if (transform == null) return;
        SetRect((RectTransform)transform, Vector2.one * 0.5f, Vector2.one * 0.5f, new Vector2(0.5f, 0.5f), position, requestedSize ?? new Vector2(370f, 78f));
        StyleButton(transform.GetComponent<Button>(), color, false);
        StyleButtonLabel(transform, fontSize);
        if (!string.IsNullOrEmpty(iconName))
        {
            Image icon = CreateIcon(transform, "ShooterIcon", ShooterSprite(iconName), Color.white);
            SetRect(icon.rectTransform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(26f, 0f), new Vector2(46f, 46f));
            Text label = transform.GetComponentInChildren<Text>(true);
            if (label != null) Stretch(label.rectTransform, new Vector2(78f, 0f), new Vector2(-18f, 0f));
        }
    }

    private static void ConfigureCanvas(Canvas canvas)
    {
        if (canvas == null) return;
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler == null) return;
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
    }

    private static void StyleButton(Button button, Color color, bool circular)
    {
        if (button == null) return;
        Image image = button.GetComponent<Image>();
        Sprite packSprite = circular ? Sprite("CircleFill") : ShooterSprite("ButtonFrame");
        image.sprite = packSprite != null ? packSprite : image.sprite;
        image.color = circular ? new Color(0.025f, 0.032f, 0.04f, 0.88f) : (packSprite != null ? Color.white : color);
        image.preserveAspect = circular;
        image.raycastTarget = true;
        button.targetGraphic = image;
        button.transition = Selectable.Transition.ColorTint;
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = Color.white;
        colors.pressedColor = new Color(0.76f, 0.76f, 0.76f, 1f);
        colors.selectedColor = Color.white;
        colors.disabledColor = new Color(0.42f, 0.44f, 0.46f, 0.58f);
        colors.fadeDuration = 0.08f;
        button.colors = colors;
        GetOrAdd<UIButtonPressFeedback>(button.gameObject);
    }

    private static void StylePanel(Transform panel, string spriteName, Color color)
    {
        if (panel == null) return;
        Image image = panel.GetComponent<Image>();
        if (image == null) image = panel.gameObject.AddComponent<Image>();
        image.sprite = ShooterSprite(spriteName);
        image.color = color;
        image.preserveAspect = false;
        image.raycastTarget = false;
    }

    private static void AddHeaderSeparator(Transform panel)
    {
        Transform separator = EnsureUI(panel, "ShooterHeaderSeparator", typeof(Image));
        SetRect((RectTransform)separator, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -96f), new Vector2(560f, 8f));
        StyleImage(separator.GetComponent<Image>(), ShooterSprite("Separator"), Color.white);
        separator.GetComponent<Image>().preserveAspect = false;
    }

    private static void StyleRuntimeButton(Transform transform, string iconName, Color iconColor)
    {
        if (transform == null) return;
        StyleButton(transform.GetComponent<Button>(), new Color(0.12f, 0.15f, 0.17f, 0.96f), false);
        if (string.IsNullOrEmpty(iconName)) return;
        Image icon = CreateIcon(transform, "ShooterIcon", ShooterSprite(iconName), iconColor);
        SetRect(icon.rectTransform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(34f, 0f), new Vector2(76f, 76f));
    }

    private static void StyleButtonLabel(Transform button, int fontSize)
    {
        Text label = button.GetComponentInChildren<Text>(true);
        if (label == null) return;
        label.gameObject.SetActive(true);
        StyleText(label, fontSize, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
        Stretch(label.rectTransform, Vector2.zero, Vector2.zero);
    }

    private static void AddTacticalShade(Transform panel)
    {
        Transform shade = EnsureUI(panel, "TacticalShade", typeof(Image));
        Stretch((RectTransform)shade, new Vector2(5f, 5f), new Vector2(-5f, -5f));
        shade.GetComponent<Image>().color = new Color(0.018f, 0.022f, 0.025f, 0.91f);
        shade.GetComponent<Image>().raycastTarget = false;
        shade.SetAsFirstSibling();
    }

    private static void StyleText(Text text, int size, TextAnchor alignment, Color color, FontStyle style)
    {
        if (text == null) return;
        text.font = UiFont;
        text.fontSize = size;
        text.fontStyle = style;
        text.alignment = alignment;
        text.color = color;
        text.raycastTarget = false;
        text.resizeTextForBestFit = false;
    }

    private static Image CreateIcon(Transform parent, string name, string spriteName, Color color) => CreateIcon(parent, name, Sprite(spriteName), color);
    private static Image CreateIcon(Transform parent, string name, Sprite sprite, Color color)
    {
        Transform transform = EnsureUI(parent, name, typeof(Image));
        Image image = transform.GetComponent<Image>();
        StyleImage(image, sprite, color);
        return image;
    }

    private static void StyleImage(Image image, Sprite sprite, Color color)
    {
        if (image == null) return;
        if (sprite != null) image.sprite = sprite;
        image.color = color;
        image.preserveAspect = sprite != null;
        image.raycastTarget = false;
    }

    private static Text EnsureText(Transform parent, string name)
    {
        Transform existing = parent.Find(name);
        if (existing != null) return existing.GetComponent<Text>();
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);
        return go.GetComponent<Text>();
    }

    private static Transform EnsureUI(Transform parent, string name, params Type[] componentTypes)
    {
        Transform existing = parent.Find(name);
        if (existing != null)
        {
            foreach (Type type in componentTypes) if (existing.GetComponent(type) == null) existing.gameObject.AddComponent(type);
            return existing;
        }
        Type[] types = new Type[componentTypes.Length + 1];
        types[0] = typeof(RectTransform);
        Array.Copy(componentTypes, 0, types, 1, componentTypes.Length);
        GameObject go = new GameObject(name, types);
        go.transform.SetParent(parent, false);
        return go.transform;
    }

    private static void MarkApplied(Transform safe)
    {
        Transform marker = EnsureUI(safe, "M20ThemeApplied");
        marker.gameObject.SetActive(false);
    }

    private static Sprite Sprite(string name) => Resources.Load<Sprite>("UI/Icons/" + name);
    private static Sprite ShooterSprite(string name) => Resources.Load<Sprite>("UI/Shooter/" + name);
    private static Text GetText(Transform root, string path) { Transform child = root != null ? root.Find(path) : null; return child != null ? child.GetComponent<Text>() : null; }
    private static Image GetImage(Transform root, string path) { Transform child = root != null ? root.Find(path) : null; return child != null ? child.GetComponent<Image>() : null; }
    private static T GetOrAdd<T>(GameObject gameObject) where T : Component => gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();

    private static void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 position, Vector2 size)
    {
        if (rect == null) return;
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        rect.localScale = Vector3.one;
    }

    private static void Stretch(RectTransform rect, Vector2 offsetMin, Vector2 offsetMax)
    {
        if (rect == null) return;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = Vector2.one * 0.5f;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
        rect.localScale = Vector3.one;
    }
}
