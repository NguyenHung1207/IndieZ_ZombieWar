using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ReleaseBuild
{
    private const string ProductName = "Zombie War";
    private const string ApplicationIdentifier = "com.indiez.zombiewar";
    private const string Version = "1.0.0";
    private const int VersionCode = 1;
    private const string ApkPath = "Builds/Android/ZombieWar.apk";

    private static readonly string[] BuildScenes =
    {
        "Assets/_Project/Scenes/MainMenu.unity",
        "Assets/_Project/Scenes/Gameplay_Level01.unity"
    };

    private static readonly string[] ProductionRoots =
    {
        "Assets/_Project/Scenes/MainMenu.unity",
        "Assets/_Project/Scenes/Gameplay_Level01.unity",
        "Assets/_Project/Prefabs/Combat/Grenade.prefab",
        "Assets/_Project/Prefabs/Combat/GrenadeExplosion.prefab",
        "Assets/_Project/Prefabs/Enemies/Zombie.prefab",
        "Assets/_Project/Prefabs/Weapons/AssaultRifle.prefab",
        "Assets/_Project/Prefabs/Weapons/Shotgun.prefab"
    };

    [MenuItem("Build/Zombie War/Validate Release Project")]
    public static void ValidateProject()
    {
        List<string> failures = new List<string>();
        ValidateSettings(failures);
        ValidateAssets(failures);

        if (failures.Count > 0)
            throw new BuildFailedException("Release validation failed:\n- " + string.Join("\n- ", failures));

        Debug.Log("RELEASE_VALIDATION_RESULT=SUCCEEDED");
    }

    [MenuItem("Build/Zombie War/Build Android APK")]
    public static void BuildAndroid()
    {
        ConfigureReleaseSettings();
        ValidateProject();

        string fullOutputPath = Path.GetFullPath(ApkPath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullOutputPath));

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = BuildScenes,
            locationPathName = fullOutputPath,
            target = BuildTarget.Android,
            targetGroup = BuildTargetGroup.Android,
            options = BuildOptions.CleanBuildCache
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;
        Debug.Log(
            $"ANDROID_BUILD_RESULT={summary.result}; " +
            $"OUTPUT={summary.outputPath}; " +
            $"SIZE_BYTES={summary.totalSize}; " +
            $"DURATION={summary.totalTime}; " +
            $"WARNINGS={summary.totalWarnings}; " +
            $"ERRORS={summary.totalErrors}");

        if (summary.result != BuildResult.Succeeded)
            throw new BuildFailedException($"Android build failed with result {summary.result}.");
    }

    private static void ConfigureReleaseSettings()
    {
        PlayerSettings.productName = ProductName;
        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, ApplicationIdentifier);
        PlayerSettings.bundleVersion = Version;
        PlayerSettings.Android.bundleVersionCode = VersionCode;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

        EditorUserBuildSettings.development = false;
        EditorUserBuildSettings.allowDebugging = false;
        EditorUserBuildSettings.connectProfiler = false;
        EditorUserBuildSettings.buildWithDeepProfilingSupport = false;
        EditorUserBuildSettings.buildAppBundle = false;
        AssetDatabase.SaveAssets();
    }

    private static void ValidateSettings(List<string> failures)
    {
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
        if (scenes.Length != BuildScenes.Length)
        {
            failures.Add($"Expected {BuildScenes.Length} build scenes, found {scenes.Length}.");
        }
        else
        {
            for (int i = 0; i < BuildScenes.Length; i++)
            {
                if (!scenes[i].enabled || scenes[i].path != BuildScenes[i])
                    failures.Add($"Build scene {i} must be enabled and set to {BuildScenes[i]}.");
            }
        }

        if (PlayerSettings.productName != ProductName)
            failures.Add($"Product name is '{PlayerSettings.productName}', expected '{ProductName}'.");
        if (PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Android) != ApplicationIdentifier)
            failures.Add("Android application identifier is incorrect.");
        if (PlayerSettings.bundleVersion != Version || PlayerSettings.Android.bundleVersionCode != VersionCode)
            failures.Add("Android version or version code is incorrect.");
        if (PlayerSettings.defaultInterfaceOrientation != UIOrientation.AutoRotation ||
            PlayerSettings.allowedAutorotateToPortrait ||
            PlayerSettings.allowedAutorotateToPortraitUpsideDown ||
            !PlayerSettings.allowedAutorotateToLandscapeLeft ||
            !PlayerSettings.allowedAutorotateToLandscapeRight)
        {
            failures.Add("Orientation must allow both landscapes and disable both portraits.");
        }
        if (PlayerSettings.GetScriptingBackend(NamedBuildTarget.Android) != ScriptingImplementation.IL2CPP)
            failures.Add("Android scripting backend must be IL2CPP.");
        if (PlayerSettings.Android.targetArchitectures != AndroidArchitecture.ARM64)
            failures.Add("Android architecture must be ARM64-only.");

        Debug.Log(
            $"RELEASE_SETTINGS product={PlayerSettings.productName}; " +
            $"identifier={PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Android)}; " +
            $"version={PlayerSettings.bundleVersion}; versionCode={PlayerSettings.Android.bundleVersionCode}; " +
            $"backend={PlayerSettings.GetScriptingBackend(NamedBuildTarget.Android)}; " +
            $"architecture={PlayerSettings.Android.targetArchitectures}; " +
            $"minSdk={PlayerSettings.Android.minSdkVersion}; targetSdk={PlayerSettings.Android.targetSdkVersion}; " +
            $"development={EditorUserBuildSettings.development}; debugging={EditorUserBuildSettings.allowDebugging}; " +
            $"profiler={EditorUserBuildSettings.connectProfiler}; deepProfiling={EditorUserBuildSettings.buildWithDeepProfilingSupport}");
    }

    private static void ValidateAssets(List<string> failures)
    {
        HashSet<string> dependencies = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (string root in ProductionRoots)
        {
            if (!File.Exists(root))
            {
                failures.Add($"Missing production root: {root}");
                continue;
            }

            foreach (string dependency in AssetDatabase.GetDependencies(root, true))
                dependencies.Add(dependency);
        }

        foreach (string dependency in dependencies)
        {
            if (dependency.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase) && !File.Exists(dependency))
                failures.Add($"Missing dependency file: {dependency}");

            if (dependency.EndsWith(".mat", StringComparison.OrdinalIgnoreCase))
            {
                Material material = AssetDatabase.LoadAssetAtPath<Material>(dependency);
                if (material == null || material.shader == null)
                    failures.Add($"Material has a missing shader: {dependency}");
            }
        }

        foreach (string root in ProductionRoots)
        {
            if (root.EndsWith(".unity", StringComparison.OrdinalIgnoreCase))
                ValidateScene(root, failures);
            else if (root.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase))
                ValidatePrefab(root, failures);
        }

        Debug.Log("PRODUCTION_DEPENDENCIES_BEGIN\n" + string.Join("\n", dependencies) + "\nPRODUCTION_DEPENDENCIES_END");
    }

    private static void ValidateScene(string path, List<string> failures)
    {
        Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
        foreach (GameObject root in scene.GetRootGameObjects())
            ValidateHierarchy(root, path, failures);
    }

    private static void ValidatePrefab(string path, List<string> failures)
    {
        GameObject root = PrefabUtility.LoadPrefabContents(path);
        try
        {
            ValidateHierarchy(root, path, failures);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ValidateHierarchy(GameObject root, string assetPath, List<string> failures)
    {
        foreach (Transform transform in root.GetComponentsInChildren<Transform>(true))
        {
            int missingScripts = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(transform.gameObject);
            if (missingScripts > 0)
                failures.Add($"{assetPath}: {transform.name} has {missingScripts} missing script(s).");
        }
    }
}
