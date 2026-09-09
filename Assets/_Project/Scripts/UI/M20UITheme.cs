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
        StyleImage(topBackdrop.GetComponent<Image>(), null, new Color(0.018f, 0.024f, 0.029f, 0.56f));
        topBackdrop.SetAsFirstSibling();

        Text health = GetText(safe, "HealthText");
        StyleText(health, 28, TextAnchor.MiddleLeft, Color.white, FontStyle.Bold);
        SetRect(health.rectTransform, Vector2.up, Vector2.up, Vector2.up, new Vector2(72f, -20f), new Vector2(235f, 48f));
        Image healthIcon = CreateIcon(safe, "HealthIcon", "Health", Green);
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

        Transform menu = safe.Find("MenuPanel");
        if (menu == null) return;
        SetRect((RectTransform)menu, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.zero, new Vector2(820f, 580f));
        menu.GetComponent<Image>().color = new Color(0.025f, 0.035f, 0.04f, 0.78f);
        Outline menuOutline = GetOrAdd<Outline>(menu.gameObject);
        menuOutline.effectColor = new Color(0.32f, 0.035f, 0.035f, 0.9f);
        menuOutline.effectDistance = new Vector2(3f, -3f);
        Image accent = GetImage(menu, "Accent");
        StyleImage(accent, null, Red);

        Text title = GetText(menu, "Title");
        StyleText(title, 82, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
        SetRect(title.rectTransform, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f, new Vector2(0f, 178f), new Vector2(760f, 100f));
        Text subtitle = GetText(menu, "Subtitle");
        StyleText(subtitle, 27, TextAnchor.MiddleCenter, Muted, FontStyle.Bold);
        SetRect(subtitle.rectTransform, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f, new Vector2(0f, 110f), new Vector2(720f, 48f));
        ConfigureMenuButton(menu.Find("PlayButton"), new Vector2(0f, 28f), Red, 32);
        ConfigureMenuButton(menu.Find("ShopButton"), new Vector2(0f, -70f), new Color(0.13f, 0.16f, 0.18f, 0.96f), 30);
        ConfigureMenuButton(menu.Find("QuitButton"), new Vector2(0f, -168f), new Color(0.075f, 0.085f, 0.095f, 0.9f), 28);
        Text footer = GetText(menu, "Footer");
        StyleText(footer, 18, TextAnchor.MiddleCenter, new Color(0.58f, 0.62f, 0.64f), FontStyle.Normal);
        SetRect(footer.rectTransform, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f, new Vector2(0f, -258f), new Vector2(720f, 30f));

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

    private static void ConfigureJoystick(Transform safe)
    {
        Transform joystickRoot = safe.Find("MobileControls/JoystickBackground");
        if (joystickRoot == null) return;
        SetRect((RectTransform)joystickRoot, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(34f, 34f), new Vector2(204f, 204f));
        Image baseImage = joystickRoot.GetComponent<Image>();
        StyleImage(baseImage, Sprite("CircleRing"), new Color(0.08f, 0.09f, 0.11f, 0.48f));
        Transform handle = joystickRoot.Find("JoystickHandle");
        SetRect((RectTransform)handle, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.zero, new Vector2(82f, 82f));
        Image handleImage = handle.GetComponent<Image>();
        StyleImage(handleImage, Sprite("CircleFill"), new Color(0.78f, 0.80f, 0.82f, 0.72f));
        JoystickVisualFeedback feedback = GetOrAdd<JoystickVisualFeedback>(joystickRoot.gameObject);
        feedback.Configure(joystickRoot.GetComponent<VirtualJoystick>(), baseImage, handleImage);
    }

    private static void ConfigureActions(Transform safe)
    {
        Transform actions = safe.Find("ActionControls");
        if (actions == null) return;
        SetRect((RectTransform)actions, new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-26f, 26f), new Vector2(450f, 292f));
        ConfigureActionButton(actions.Find("FireButton"), "Fire", Red, new Vector2(288f, 0f), 156f, "FIRE");
        ConfigureActionButton(actions.Find("GrenadeButton"), "Grenade", OrangeRed, new Vector2(96f, 18f), 118f, string.Empty);
        ConfigureActionButton(actions.Find("ReloadButton"), "Reload", new Color(0.12f, 0.15f, 0.17f, 0.9f), new Vector2(208f, 145f), 100f, string.Empty);
        ConfigureActionButton(actions.Find("SwitchButton"), "Switch", new Color(0.12f, 0.15f, 0.17f, 0.9f), new Vector2(326f, 182f), 98f, string.Empty);

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
        Image image = transform.GetComponent<Image>();
        image.sprite = Sprite("CircleFill");
        StyleButton(transform.GetComponent<Button>(), background, true);
        Image icon = CreateIcon(transform, "Icon", iconName, Color.white);
        SetRect(icon.rectTransform, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f, new Vector2(0f, string.IsNullOrEmpty(labelText) ? 0f : 8f), Vector2.one * (size * 0.47f));
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
        card.GetComponent<Image>().color = Panel;
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
        result.GetComponent<Image>().color = Panel;
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
        shop.GetComponent<Image>().color = new Color(0.012f, 0.018f, 0.022f, 0.95f);
        Transform header = EnsureUI(shop, "Header", typeof(Image));
        SetRect((RectTransform)header, Vector2.up, Vector2.one, new Vector2(0.5f, 1f), Vector2.zero, new Vector2(0f, 104f));
        header.GetComponent<Image>().color = new Color(0.035f, 0.045f, 0.052f, 0.96f);
        header.SetAsFirstSibling();

        Text title = GetText(shop, "ShopTitle");
        StyleText(title, 42, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
        SetRect(title.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -18f), new Vector2(520f, 60f));
        Text feedback = GetText(shop, "ShopFeedback");
        StyleText(feedback, 22, TextAnchor.MiddleCenter, Gold, FontStyle.Bold);
        SetRect(feedback.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -78f), new Vector2(700f, 34f));
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
            SetRect((RectTransform)card, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f, new Vector2((i - 2) * 300f, -12f), new Vector2(282f, 552f));
            backgrounds[i] = card.GetComponent<Image>();
            backgrounds[i].color = new Color(0.075f, 0.09f, 0.105f, 0.98f);
            accents[i] = CreateIcon(card, "StateAccent", "Solid", new Color(0.27f, 0.29f, 0.31f));
            SetRect(accents[i].rectTransform, Vector2.up, Vector2.one, new Vector2(0.5f, 1f), Vector2.zero, new Vector2(0f, 7f));

            Text name = EnsureText(card, "WeaponName");
            name.text = labels[i];
            StyleText(name, 24, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
            SetRect(name.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -18f), new Vector2(250f, 42f));

            Transform preview = card.Find("WeaponPreview");
            Transform frame = EnsureUI(card, "PreviewFrame", typeof(Image));
            SetRect((RectTransform)frame, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -64f), new Vector2(252f, 150f));
            frame.GetComponent<Image>().color = new Color(0.018f, 0.024f, 0.03f, 0.86f);
            preview.SetParent(frame, false);
            Image previewImage = preview.GetComponent<Image>();
            previewImage.color = Color.white;
            previewImage.preserveAspect = true;
            previewImage.raycastTarget = false;
            Stretch((RectTransform)preview, new Vector2(9f, 9f), new Vector2(-9f, -9f));

            Text role = EnsureText(card, "Role");
            role.text = roles[i];
            StyleText(role, 16, TextAnchor.MiddleCenter, new Color(0.78f, 0.81f, 0.83f), FontStyle.Bold);
            SetRect(role.rectTransform, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f, new Vector2(0f, 52f), new Vector2(250f, 30f));

            Transform infoTransform = card.Find("Info") ?? card.Find("Stats");
            infoTransform.gameObject.name = "Stats";
            Text info = infoTransform.GetComponent<Text>();
            StyleText(info, 18, TextAnchor.MiddleLeft, new Color(0.88f, 0.90f, 0.92f), FontStyle.Normal);
            SetRect(info.rectTransform, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f, new Vector2(0f, -38f), new Vector2(218f, 142f));
            infos[i] = info;

            Text state = EnsureText(card, "State");
            StyleText(state, 17, TextAnchor.MiddleCenter, Gold, FontStyle.Bold);
            SetRect(state.rectTransform, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f, new Vector2(0f, -132f), new Vector2(240f, 32f));
            states[i] = state;

            Transform buy = card.Find("BuyButton");
            SetRect((RectTransform)buy, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f, new Vector2(0f, -185f), new Vector2(232f, 52f));
            StyleButton(buy.GetComponent<Button>(), Red, false);
            StyleButtonLabel(buy, 20);
            priceIcons[i] = CreateIcon(buy, "PriceIcon", "Coin", Gold);
            SetRect(priceIcons[i].rectTransform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(12f, 0f), new Vector2(24f, 24f));

            Transform equip = card.Find("EquipButton");
            SetRect((RectTransform)equip, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f, new Vector2(0f, -241f), new Vector2(232f, 44f));
            StyleButton(equip.GetComponent<Button>(), new Color(0.14f, 0.17f, 0.19f, 0.98f), false);
            StyleButtonLabel(equip, 18);
        }
        controller.ConfigurePresentation(shopCoins, infos, states, backgrounds, accents, priceIcons);
    }

    private static void ConfigureMenuButton(Transform transform, Vector2 position, Color color, int fontSize)
    {
        if (transform == null) return;
        SetRect((RectTransform)transform, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f, position, new Vector2(370f, 78f));
        StyleButton(transform.GetComponent<Button>(), color, false);
        StyleButtonLabel(transform, fontSize);
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
        image.color = color;
        if (circular) { image.sprite = Sprite("CircleFill"); image.preserveAspect = true; }
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

    private static void StyleButtonLabel(Transform button, int fontSize)
    {
        Text label = button.GetComponentInChildren<Text>(true);
        if (label == null) return;
        label.gameObject.SetActive(true);
        StyleText(label, fontSize, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
        Stretch(label.rectTransform, Vector2.zero, Vector2.zero);
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
