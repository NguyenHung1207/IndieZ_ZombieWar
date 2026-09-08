using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[InitializeOnLoad]
public static class M13SmokeTest
{
    private const string ActiveKey = "M13SmokeTest.Active";
    private const string PhaseKey = "M13SmokeTest.Phase";
    private const string ErrorCountKey = "M13SmokeTest.ErrorCount";
    private const string CaptureFolder = "Logs/M13-QA";

    private enum Phase
    {
        None,
        EnterMainMenu,
        WaitForGameplay,
        ExerciseGameplay,
        ValidateMovement,
        ValidateCombat,
        ValidateGrenadeCleanup,
        LongSession,
        WaitForMenuAfterVictory,
        WaitForGameOverScene,
        WaitForRestart,
        WaitForSecondGameOver,
        WaitForFinalMenu,
        StopPlayMode,
        Exit
    }

    private static double phaseStarted;
    private static Vector3 movementStart;
    private static int maximumZombieCount;
    private static int maximumObjectCount;
    private static int victoryEvents;
    private static Grenade testedGrenade;
    private static ZombieHealth grenadeTarget;
    private static float grenadeTargetHealth;
    private static readonly List<string> Failures = new List<string>();

    static M13SmokeTest()
    {
        if (SessionState.GetBool(ActiveKey, false))
            Subscribe();
    }

    public static void Run()
    {
        Directory.CreateDirectory(CaptureFolder);
        SessionState.SetBool(ActiveKey, true);
        SessionState.SetInt(ErrorCountKey, 0);
        Failures.Clear();
        Subscribe();
        EditorSceneManager.OpenScene("Assets/_Project/Scenes/MainMenu.unity", OpenSceneMode.Single);
        SetPhase(Phase.EnterMainMenu);
        EditorApplication.EnterPlaymode();
    }

    public static void DumpGameplayScene()
    {
        Scene scene = EditorSceneManager.OpenScene("Assets/_Project/Scenes/Gameplay_Level01.unity", OpenSceneMode.Single);
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Transform transform in root.GetComponentsInChildren<Transform>(true))
            {
                if (transform.GetComponent<Camera>() != null || transform.GetComponent<Canvas>() != null ||
                    transform.GetComponent<GameHUD>() != null || transform.GetComponent<VirtualJoystick>() != null ||
                    transform.GetComponent<FireHoldButton>() != null || transform.GetComponent<MobileActionButton>() != null)
                {
                    Debug.Log($"M13_HIERARCHY path={GetPath(transform)} active={transform.gameObject.activeSelf} " +
                              $"position={transform.position} scale={transform.lossyScale}");
                }
            }
        }

        GameHUD hud = UnityEngine.Object.FindFirstObjectByType<GameHUD>(FindObjectsInactive.Include);
        if (hud != null)
        {
            SerializedObject serializedHud = new SerializedObject(hud);
            SerializedProperty controls = serializedHud.FindProperty("mobileControls");
            Debug.Log("M13_MOBILE_CONTROLS_REFERENCE=" +
                      (controls.objectReferenceValue != null ? controls.objectReferenceValue.name : "NULL"));
        }

        foreach (Renderer renderer in UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (renderer.name.IndexOf("ground", StringComparison.OrdinalIgnoreCase) >= 0 ||
                renderer.name.IndexOf("arena", StringComparison.OrdinalIgnoreCase) >= 0 ||
                renderer.name.IndexOf("bound", StringComparison.OrdinalIgnoreCase) >= 0 ||
                renderer.name.IndexOf("wall", StringComparison.OrdinalIgnoreCase) >= 0 ||
                renderer.name.IndexOf("obstacle", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                Debug.Log($"M13_RENDERER path={GetPath(renderer.transform)} bounds={renderer.bounds} material={renderer.sharedMaterial?.name}");
            }
        }
        Debug.Log("M13_DUMP_RESULT=SUCCEEDED");
    }

    private static void Subscribe()
    {
        EditorApplication.update -= Update;
        EditorApplication.update += Update;
        Application.logMessageReceived -= OnLog;
        Application.logMessageReceived += OnLog;
        phaseStarted = EditorApplication.timeSinceStartup;
    }

    private static void Update()
    {
        Phase phase = (Phase)SessionState.GetInt(PhaseKey, 0);
        if (phase == Phase.Exit)
        {
            Finish();
            return;
        }

        if (!EditorApplication.isPlaying)
        {
            if (phase == Phase.StopPlayMode)
                SetPhase(Phase.Exit);
            return;
        }

        try
        {
            switch (phase)
            {
                case Phase.EnterMainMenu:
                    EnterMainMenu();
                    break;
                case Phase.WaitForGameplay:
                    WaitForGameplay();
                    break;
                case Phase.ExerciseGameplay:
                    ExerciseGameplay();
                    break;
                case Phase.ValidateMovement:
                    ValidateMovement();
                    break;
                case Phase.ValidateCombat:
                    ValidateCombat();
                    break;
                case Phase.ValidateGrenadeCleanup:
                    ValidateGrenadeCleanup();
                    break;
                case Phase.LongSession:
                    ValidateLongSession();
                    break;
                case Phase.WaitForMenuAfterVictory:
                    WaitForMenuAfterVictory();
                    break;
                case Phase.WaitForGameOverScene:
                    WaitForGameOverScene();
                    break;
                case Phase.WaitForRestart:
                    WaitForRestart();
                    break;
                case Phase.WaitForSecondGameOver:
                    WaitForSecondGameOver();
                    break;
                case Phase.WaitForFinalMenu:
                    WaitForFinalMenu();
                    break;
            }
        }
        catch (Exception exception)
        {
            Fail("Runner exception: " + exception);
            Stop();
        }
    }

    private static void EnterMainMenu()
    {
        if (Elapsed < 1.0)
            return;
        Require(SceneManager.GetActiveScene().name == "MainMenu", "Launch scene is MainMenu.");
        Require(UnityEngine.Object.FindFirstObjectByType<MainMenuController>() != null, "MainMenuController exists.");
        CaptureAllAspects("MainMenu");
        Button play = FindButton("PLAY");
        Require(play != null, "PLAY button exists.");
        if (play == null)
        {
            Stop();
            return;
        }
        play.onClick.Invoke();
        SetPhase(Phase.WaitForGameplay);
    }

    private static void WaitForGameplay()
    {
        if (SceneManager.GetActiveScene().name == "Gameplay_Level01")
        {
            SetPhase(Phase.ExerciseGameplay);
            return;
        }
        if (Elapsed > 5.0)
        {
            Fail("PLAY did not load Gameplay_Level01.");
            Stop();
        }
    }

    private static void ExerciseGameplay()
    {
        if (Elapsed < 3.0)
            return;

        GameSession session = UnityEngine.Object.FindFirstObjectByType<GameSession>();
        PlayerMovement movement = UnityEngine.Object.FindFirstObjectByType<PlayerMovement>();
        PlayerHealth health = UnityEngine.Object.FindFirstObjectByType<PlayerHealth>();
        PlayerWeaponController weapons = UnityEngine.Object.FindFirstObjectByType<PlayerWeaponController>();
        PlayerAutoAim aim = UnityEngine.Object.FindFirstObjectByType<PlayerAutoAim>();
        PlayerGrenadeController grenades = UnityEngine.Object.FindFirstObjectByType<PlayerGrenadeController>();
        ZombieSpawnDirector spawner = UnityEngine.Object.FindFirstObjectByType<ZombieSpawnDirector>();
        GameHUD hud = UnityEngine.Object.FindFirstObjectByType<GameHUD>();

        Require(session != null && session.IsPlaying, "Gameplay session starts in Playing state.");
        Require(movement != null && health != null, "Player movement and health exist.");
        Require(weapons != null && aim != null && grenades != null, "Weapon, auto-aim, and grenade controllers exist.");
        Require(spawner != null && hud != null, "Spawn director and HUD exist.");
        Require(weapons != null && weapons.EquippedWeapon != null && weapons.EquippedWeapon.DisplayName == "Assault Rifle",
            "Assault Rifle is initially equipped.");
        CaptureAllAspects("Gameplay");

        if (session == null || movement == null || health == null || weapons == null || aim == null || grenades == null)
        {
            Stop();
            return;
        }

        SetPrivateFloat(health, "maxHealth", 1000000f);
        SetPrivateFloat(health, "currentHealth", 1000000f);
        session.StateChanged += state =>
        {
            if (state == GameSessionState.Victory)
                victoryEvents++;
        };

        movementStart = movement.transform.position;
        movement.SetMoveInput(new Vector2(1f, 1f));
        SetPhase(Phase.ValidateMovement);
    }

    private static void ValidateMovement()
    {
        if (Elapsed < 0.8)
            return;
        PlayerMovement movement = UnityEngine.Object.FindFirstObjectByType<PlayerMovement>();
        if (movement == null)
        {
            Fail("PlayerMovement disappeared during movement test.");
            Stop();
            return;
        }
        movement.SetMoveInput(Vector2.zero);
        Vector3 delta = movement.transform.position - movementStart;
        Require(delta.x > 0.2f && delta.z > 0.2f, "Diagonal external/joystick-equivalent movement works.");
        SetPhase(Phase.ValidateCombat);
    }

    private static void ValidateCombat()
    {
        if (Elapsed < 0.5)
            return;

        PlayerMovement movement = UnityEngine.Object.FindFirstObjectByType<PlayerMovement>();
        PlayerHealth playerHealth = UnityEngine.Object.FindFirstObjectByType<PlayerHealth>();
        PlayerWeaponController weapons = UnityEngine.Object.FindFirstObjectByType<PlayerWeaponController>();
        PlayerAutoAim aim = UnityEngine.Object.FindFirstObjectByType<PlayerAutoAim>();
        PlayerGrenadeController grenades = UnityEngine.Object.FindFirstObjectByType<PlayerGrenadeController>();
        ZombieAI source = UnityEngine.Object.FindFirstObjectByType<ZombieAI>();
        if (movement == null || playerHealth == null || weapons == null || aim == null || grenades == null || source == null)
        {
            Fail("Required combat object was not available.");
            Stop();
            return;
        }

        Transform player = movement.transform;
        ZombieAI target = CreateZombie(source, player.position + player.forward * 5f, player);
        Physics.SyncTransforms();
        aim.ClearTarget();
        aim.RefreshTarget();
        Require(aim.CurrentTarget == target, "Auto-aim acquires a zombie inside the forward cone.");

        Warp(target, player.position - player.forward * 5f);
        Physics.SyncTransforms();
        aim.ClearTarget();
        aim.RefreshTarget();
        Require(aim.CurrentTarget != target, "Auto-aim ignores a zombie behind the player.");

        Warp(target, player.position + player.forward * 5f);
        GameObject obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obstacle.name = "M13_LOS_Obstacle";
        obstacle.transform.position = player.position + player.forward * 2.5f + Vector3.up;
        obstacle.transform.localScale = new Vector3(1.5f, 3f, 1.5f);
        Physics.SyncTransforms();
        aim.ClearTarget();
        aim.RefreshTarget();
        Require(aim.CurrentTarget != target, "Auto-aim rejects an obstacle-blocked zombie.");
        UnityEngine.Object.DestroyImmediate(obstacle);

        Warp(target, player.position + player.forward * 5f);
        Physics.SyncTransforms();
        ZombieHealth targetHealth = target.GetComponent<ZombieHealth>();
        float rifleHealth = targetHealth.CurrentHealth;
        aim.ClearTarget();
        aim.RefreshTarget();
        Require(weapons.TryFire(), "Rifle fires.");
        Require(targetHealth.CurrentHealth < rifleHealth, "Rifle damages the aimed zombie.");

        targetHealth.TakeDamage(100000f);
        aim.ClearTarget();
        aim.RefreshTarget();
        Require(aim.CurrentTarget != target, "Auto-aim ignores dead/dissolving zombies.");

        movement.SetMoveInput(Vector2.right);
        weapons.SwitchWeapon();
        Require(weapons.EquippedWeapon != null && weapons.EquippedWeapon.DisplayName == "Shotgun" &&
                weapons.EquippedWeapon.FireMode == WeaponFireMode.SemiAutomatic && weapons.EquippedWeapon.PelletCount > 1,
            "Weapon switching while moving equips the semi-automatic multi-pellet Shotgun.");
        movement.SetMoveInput(Vector2.zero);

        ZombieAI shotgunZombie = CreateZombie(source, player.position + player.forward * 4f, player);
        Physics.SyncTransforms();
        ZombieHealth shotgunHealth = shotgunZombie.GetComponent<ZombieHealth>();
        float beforeShotgun = shotgunHealth.CurrentHealth;
        aim.ClearTarget();
        aim.RefreshTarget();
        Require(weapons.TryFire(), "Shotgun fires.");
        Require(shotgunHealth.CurrentHealth < beforeShotgun, "Shotgun pellets damage an aimed zombie.");

        ZombieAI grenadeZombie = CreateZombie(source, player.position + player.forward * 3f, player);
        grenadeTarget = grenadeZombie.GetComponent<ZombieHealth>();
        grenadeTargetHealth = grenadeTarget.CurrentHealth;
        grenades.ThrowGrenade();
        testedGrenade = UnityEngine.Object.FindFirstObjectByType<Grenade>();
        Require(testedGrenade != null && testedGrenade.GetComponent<Rigidbody>() != null,
            "Grenade throw creates a Rigidbody projectile.");
        if (testedGrenade != null)
        {
            testedGrenade.transform.position = grenadeZombie.transform.position;
            testedGrenade.Explode();
        }
        SetPhase(Phase.ValidateGrenadeCleanup);
    }

    private static void ValidateGrenadeCleanup()
    {
        if (Elapsed < 2.3)
            return;
        Require(testedGrenade == null, "Exploded grenade is cleaned up.");
        Require(grenadeTarget != null && grenadeTarget.CurrentHealth < grenadeTargetHealth,
            "Grenade explosion damages a nearby zombie.");

        ZombieAI[] zombies = UnityEngine.Object.FindObjectsByType<ZombieAI>(FindObjectsSortMode.None);
        ZombieAI live = Array.Find(zombies, zombie => zombie != null && !zombie.IsDead);
        PlayerMovement movement = UnityEngine.Object.FindFirstObjectByType<PlayerMovement>();
        PlayerHealth playerHealth = UnityEngine.Object.FindFirstObjectByType<PlayerHealth>();
        if (live != null && movement != null && playerHealth != null)
        {
            Warp(live, movement.transform.position + movement.transform.forward * 1.1f);
            live.SetTarget(movement.transform);
            float beforeAttack = playerHealth.CurrentHealth;
            SetPrivateFloat(live, "nextAttackTime", 0f);
            live.SendMessage("Update", SendMessageOptions.DontRequireReceiver);
            Require(playerHealth.CurrentHealth < beforeAttack, "Zombie attack damages the player.");
        }
        else
        {
            Fail("A live zombie was unavailable for chase/attack validation.");
        }

        SetPhase(Phase.LongSession);
    }

    private static void ValidateLongSession()
    {
        GameSession session = UnityEngine.Object.FindFirstObjectByType<GameSession>();
        ZombieSpawnDirector spawner = UnityEngine.Object.FindFirstObjectByType<ZombieSpawnDirector>();
        if (session == null || spawner == null)
        {
            Fail("Session or spawner disappeared during the long-session test.");
            Stop();
            return;
        }

        maximumZombieCount = Mathf.Max(maximumZombieCount, spawner.ActiveZombieCount);
        maximumObjectCount = Mathf.Max(maximumObjectCount, UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None).Length);
        if (spawner.ActiveZombieCount > 24)
            Fail("Active zombie cap exceeded 24.");

        if (session.IsPlaying)
            return;

        Require(session.State == GameSessionState.Victory, "The production 180-second session reaches Victory.");
        Require(victoryEvents == 1, "Victory occurs exactly once.");
        Require(!spawner.enabled, "Spawning stops at Victory.");
        Require(!UnityEngine.Object.FindFirstObjectByType<PlayerMovement>().enabled, "Player movement stops at Victory.");
        Require(!UnityEngine.Object.FindFirstObjectByType<PlayerWeaponController>().enabled, "Combat stops at Victory.");
        Require(AreMobileControlsHidden(), "Mobile controls hide behind the Victory result panel.");
        Debug.Log($"M13_LONG_SESSION elapsed={session.ElapsedTime:F2}; maxActiveZombies={maximumZombieCount}; maxGameObjects={maximumObjectCount}");
        CaptureAllAspects("Victory");

        Button mainMenu = FindButton("MAIN MENU");
        Require(mainMenu != null, "Victory MAIN MENU button exists.");
        if (mainMenu == null)
        {
            Stop();
            return;
        }
        mainMenu.onClick.Invoke();
        SetPhase(Phase.WaitForMenuAfterVictory);
    }

    private static void WaitForMenuAfterVictory()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            if (Elapsed > 5.0)
            {
                Fail("Victory MAIN MENU button did not load MainMenu.");
                Stop();
            }
            return;
        }

        Button play = FindButton("PLAY");
        Require(play != null, "PLAY button remains functional after returning from Victory.");
        play?.onClick.Invoke();
        SetPhase(Phase.WaitForGameOverScene);
    }

    private static void WaitForGameOverScene()
    {
        if (SceneManager.GetActiveScene().name != "Gameplay_Level01" || Elapsed < 1.0)
            return;
        PlayerHealth health = UnityEngine.Object.FindFirstObjectByType<PlayerHealth>();
        Require(health != null, "PlayerHealth exists in second gameplay session.");
        health?.TakeDamage(100000f);
        Require(GameSession.Instance != null && GameSession.Instance.State == GameSessionState.GameOver,
            "Lethal damage reaches Game Over.");
        Require(AreMobileControlsHidden(), "Mobile controls hide behind the Game Over result panel.");
        CaptureAllAspects("GameOver");
        Button restart = FindButton("RESTART");
        Require(restart != null, "Game Over RESTART button exists.");
        restart?.onClick.Invoke();
        SetPhase(Phase.WaitForRestart);
    }

    private static void WaitForRestart()
    {
        if (SceneManager.GetActiveScene().name != "Gameplay_Level01" || Elapsed < 1.0)
            return;
        Require(GameSession.Instance != null && GameSession.Instance.IsPlaying && GameSession.Instance.ElapsedTime < 5f,
            "Restart creates a clean Playing session.");
        UnityEngine.Object.FindFirstObjectByType<PlayerHealth>()?.TakeDamage(100000f);
        SetPhase(Phase.WaitForSecondGameOver);
    }

    private static void WaitForSecondGameOver()
    {
        if (GameSession.Instance == null || GameSession.Instance.State != GameSessionState.GameOver)
            return;
        Button mainMenu = FindButton("MAIN MENU");
        Require(mainMenu != null, "Game Over MAIN MENU button exists.");
        mainMenu?.onClick.Invoke();
        SetPhase(Phase.WaitForFinalMenu);
    }

    private static void WaitForFinalMenu()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            if (Elapsed > 5.0)
            {
                Fail("Game Over MAIN MENU button did not load MainMenu.");
                Stop();
            }
            return;
        }
        Require(UnityEngine.Object.FindFirstObjectByType<MainMenuController>() != null,
            "Final return reaches a functional MainMenu.");
        Stop();
    }

    private static ZombieAI CreateZombie(ZombieAI source, Vector3 desiredPosition, Transform player)
    {
        NavMesh.SamplePosition(desiredPosition, out NavMeshHit hit, 4f, NavMesh.AllAreas);
        Vector3 position = hit.hit ? hit.position : desiredPosition;
        ZombieAI clone = UnityEngine.Object.Instantiate(source, position, Quaternion.identity);
        clone.name = "M13_TestZombie";
        clone.SetTarget(player);
        return clone;
    }

    private static void Warp(ZombieAI zombie, Vector3 desiredPosition)
    {
        NavMesh.SamplePosition(desiredPosition, out NavMeshHit hit, 4f, NavMesh.AllAreas);
        Vector3 position = hit.hit ? hit.position : desiredPosition;
        NavMeshAgent agent = zombie.GetComponent<NavMeshAgent>();
        if (agent != null && agent.isOnNavMesh)
            agent.Warp(position);
        else
            zombie.transform.position = position;
    }

    private static Button FindButton(string label)
    {
        foreach (Button button in UnityEngine.Object.FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            Text text = button.GetComponentInChildren<Text>(true);
            if (text != null && string.Equals(text.text.Trim(), label, StringComparison.OrdinalIgnoreCase))
                return button;
        }
        return null;
    }

    private static bool AreMobileControlsHidden()
    {
        foreach (FireHoldButton button in UnityEngine.Object.FindObjectsByType<FireHoldButton>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (button.gameObject.activeInHierarchy)
                return false;
        }
        foreach (MobileActionButton button in UnityEngine.Object.FindObjectsByType<MobileActionButton>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (button.gameObject.activeInHierarchy)
                return false;
        }
        foreach (VirtualJoystick joystick in UnityEngine.Object.FindObjectsByType<VirtualJoystick>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (joystick.gameObject.activeInHierarchy)
                return false;
        }
        return true;
    }

    private static void CaptureAllAspects(string state)
    {
        Capture(state, 1600, 900, "16x9");
        Capture(state, 1800, 900, "18x9");
        Capture(state, 1950, 900, "19_5x9");
        Capture(state, 2000, 900, "20x9");
    }

    private static void Capture(string state, int width, int height, string aspect)
    {
        Camera camera = Camera.main ?? UnityEngine.Object.FindFirstObjectByType<Camera>();
        GameObject temporaryCamera = null;
        if (camera == null)
        {
            temporaryCamera = new GameObject("M13_CaptureCamera");
            camera = temporaryCamera.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.025f, 0.035f, 0.045f, 1f);
        }

        Canvas[] canvases = UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        RenderMode[] modes = new RenderMode[canvases.Length];
        Camera[] worldCameras = new Camera[canvases.Length];
        float[] planeDistances = new float[canvases.Length];
        RenderTexture previousTarget = camera.targetTexture;
        RenderTexture previousActive = RenderTexture.active;
        RenderTexture target = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        Texture2D image = new Texture2D(width, height, TextureFormat.RGB24, false);
        try
        {
            for (int i = 0; i < canvases.Length; i++)
            {
                modes[i] = canvases[i].renderMode;
                worldCameras[i] = canvases[i].worldCamera;
                planeDistances[i] = canvases[i].planeDistance;
                if (canvases[i].renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    canvases[i].renderMode = RenderMode.ScreenSpaceCamera;
                    canvases[i].worldCamera = camera;
                    canvases[i].planeDistance = Mathf.Max(camera.nearClipPlane + 0.1f, 1f);
                }
            }

            camera.targetTexture = target;
            Canvas.ForceUpdateCanvases();
            camera.Render();
            RenderTexture.active = target;
            image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            image.Apply();
            string path = Path.Combine(CaptureFolder, $"{state}_{aspect}.png");
            File.WriteAllBytes(path, image.EncodeToPNG());
            Debug.Log($"M13_CAPTURE {path} {width}x{height}");
        }
        finally
        {
            camera.targetTexture = previousTarget;
            RenderTexture.active = previousActive;
            for (int i = 0; i < canvases.Length; i++)
            {
                canvases[i].renderMode = modes[i];
                canvases[i].worldCamera = worldCameras[i];
                canvases[i].planeDistance = planeDistances[i];
            }
            UnityEngine.Object.DestroyImmediate(image);
            target.Release();
            UnityEngine.Object.DestroyImmediate(target);
            if (temporaryCamera != null)
                UnityEngine.Object.DestroyImmediate(temporaryCamera);
        }
    }

    private static void SetPrivateFloat(object target, string fieldName, float value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        field?.SetValue(target, value);
    }

    private static string GetPath(Transform transform)
    {
        string path = transform.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }
        return path;
    }

    private static void Require(bool condition, string message)
    {
        if (condition)
            Debug.Log("M13_QA_PASS " + message);
        else
            Fail(message);
    }

    private static void Fail(string message)
    {
        Failures.Add(message);
        Debug.LogError("M13_QA_FAIL " + message);
    }

    private static void OnLog(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
            SessionState.SetInt(ErrorCountKey, SessionState.GetInt(ErrorCountKey, 0) + 1);
    }

    private static void Stop()
    {
        Debug.Log($"M13_QA_SUMMARY failures={Failures.Count}; runtimeErrorLogs={SessionState.GetInt(ErrorCountKey, 0)}; " +
                  $"maxActiveZombies={maximumZombieCount}; maxGameObjects={maximumObjectCount}");
        SetPhase(Phase.StopPlayMode);
        EditorApplication.ExitPlaymode();
    }

    private static void Finish()
    {
        EditorApplication.update -= Update;
        Application.logMessageReceived -= OnLog;
        bool succeeded = Failures.Count == 0 && SessionState.GetInt(ErrorCountKey, 0) == 0;
        SessionState.EraseBool(ActiveKey);
        SessionState.EraseInt(PhaseKey);
        Debug.Log(succeeded ? "M13_SMOKE_RESULT=SUCCEEDED" : "M13_SMOKE_RESULT=FAILED");
        EditorApplication.Exit(succeeded ? 0 : 1);
    }

    private static double Elapsed => EditorApplication.timeSinceStartup - phaseStarted;

    private static void SetPhase(Phase phase)
    {
        SessionState.SetInt(PhaseKey, (int)phase);
        phaseStarted = EditorApplication.timeSinceStartup;
    }
}
