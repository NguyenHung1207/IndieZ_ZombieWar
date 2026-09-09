using System.IO;
using UnityEditor;
using UnityEngine;

public static class WeaponPreviewGenerator
{
    private const int PreviewWidth = 512;
    private const int PreviewHeight = 256;
    private const string OutputFolder = "Assets/_Project/UI/WeaponPreviews";

    private struct PreviewSource
    {
        public readonly string PrefabPath;
        public readonly string OutputName;
        public readonly Vector3 PreviewRotation;
        public readonly string DiffusePath;
        public readonly string NormalPath;

        public PreviewSource(string prefabPath, string outputName, Vector3 previewRotation, string diffusePath, string normalPath)
        {
            PrefabPath = prefabPath;
            OutputName = outputName;
            PreviewRotation = previewRotation;
            DiffusePath = diffusePath;
            NormalPath = normalPath;
        }
    }

    private static readonly PreviewSource[] Sources =
    {
        // Low Poly Guns use +Z as their barrel axis.  These editor-only presets
        // turn that axis to screen-right and give a small top-down presentation angle.
        new PreviewSource("Assets/_Project/Prefabs/Weapons/AssaultRifle.prefab", "AssaultRifle", new Vector3(12f, 90f, 0f), "Assets/Low Poly Guns/Models/Guns/assault1/assault1_diffuse.png", "Assets/Low Poly Guns/Models/Guns/assault1/assault1_normal.png"),
        new PreviewSource("Assets/_Project/Prefabs/Weapons/Shotgun.prefab", "Shotgun", new Vector3(14f, 90f, 0f), "Assets/Low Poly Guns/Models/Guns/shotgun1/shotgun1_diffuse.png", "Assets/Low Poly Guns/Models/Guns/shotgun1/shotgun1_normal.png"),
        new PreviewSource("Assets/_Project/Prefabs/Weapons/SMG.prefab", "SMG", new Vector3(15f, 90f, 0f), "Assets/Low Poly Guns/Models/Guns/smg1/smg1_diffuse.png", "Assets/Low Poly Guns/Models/Guns/smg1/smg1_normal.png"),
        new PreviewSource("Assets/_Project/Prefabs/Weapons/Pistol.prefab", "Pistol", new Vector3(12f, 90f, 0f), "Assets/Low Poly Guns/Models/Guns/pistol1/pistol1_diffuse.png", "Assets/Low Poly Guns/Models/Guns/pistol1/pistol1_normal.png"),
        new PreviewSource("Assets/_Project/Prefabs/Weapons/Sniper.prefab", "Sniper", new Vector3(10f, 90f, 0f), "Assets/Low Poly Guns/Models/Guns/sniper1/sniper1_diffuse.png", "Assets/Low Poly Guns/Models/Guns/sniper1/sniper1_normal.png")
    };

    [MenuItem("Tools/Zombie War/Generate Shop Weapon Previews")]
    public static void Generate()
    {
        Directory.CreateDirectory(OutputFolder);
        for (int i = 0; i < Sources.Length; i++)
            RenderPreview(Sources[i]);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("SHOP_WEAPON_PREVIEWS=GENERATED");
    }

    private static void RenderPreview(PreviewSource source)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(source.PrefabPath);
        if (prefab == null)
            throw new FileNotFoundException("Weapon prefab was not found.", source.PrefabPath);

        PreviewRenderUtility preview = new PreviewRenderUtility();
        RenderTexture renderTexture = new RenderTexture(PreviewWidth, PreviewHeight, 24, RenderTextureFormat.ARGB32);
        GameObject instance = Object.Instantiate(prefab);
        GameObject previewRoot = new GameObject(source.OutputName + " Preview");
        Material generatedPreviewMaterial = null;
        try
        {
            Transform gunVisual = FindGunVisual(instance.transform);
            if (gunVisual == null)
                throw new System.InvalidOperationException(prefab.name + " has no GunVisual subtree.");

            // Only the model subtree is put into the preview scene. Disable every
            // non-mesh renderer there so muzzle flashes, tracers, trails, and VFX
            // can neither render nor influence the presentation.
            gunVisual.SetParent(previewRoot.transform, true);
            SetPreviewRenderers(gunVisual);
            generatedPreviewMaterial = ResolvePreviewMaterials(source, gunVisual);
            previewRoot.transform.rotation = Quaternion.Euler(source.PreviewRotation);

            Bounds bounds = CalculateRendererBounds(gunVisual.gameObject);
            previewRoot.transform.position = -bounds.center;
            bounds = CalculateRendererBounds(gunVisual.gameObject);
            preview.AddSingleGO(previewRoot);

            Vector3 target = bounds.center;
            Camera camera = preview.camera;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.clear;
            camera.orthographic = true;
            camera.nearClipPlane = 0.01f;
            camera.farClipPlane = 100f;
            float aspect = (float)PreviewWidth / PreviewHeight;
            camera.orthographicSize = Mathf.Max(
                bounds.extents.x / (aspect * 0.80f),
                bounds.extents.y / 0.60f);
            camera.orthographicSize = Mathf.Max(camera.orthographicSize, 0.01f);
            float cameraDistance = Mathf.Max(2f, bounds.extents.z + 1f);
            camera.transform.position = target - Vector3.forward * cameraDistance;
            camera.transform.LookAt(target);
            preview.lights[0].color = Color.white;
            preview.lights[0].intensity = 1.35f;
            preview.lights[0].transform.rotation = Quaternion.Euler(35f, -30f, 0f);
            preview.lights[1].color = new Color(0.9f, 0.93f, 1f);
            preview.lights[1].intensity = 0.45f;
            preview.lights[1].transform.rotation = Quaternion.Euler(340f, 145f, 0f);

            camera.targetTexture = renderTexture;
            camera.Render();
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTexture;
            Texture2D texture = new Texture2D(PreviewWidth, PreviewHeight, TextureFormat.RGBA32, false);
            texture.ReadPixels(new Rect(0f, 0f, PreviewWidth, PreviewHeight), 0, 0);
            texture.Apply();
            RenderTexture.active = previous;
            File.WriteAllBytes(OutputFolder + "/" + source.OutputName + ".png", texture.EncodeToPNG());
            Object.DestroyImmediate(texture);
            camera.targetTexture = null;
        }
        finally
        {
            Object.DestroyImmediate(instance);
            Object.DestroyImmediate(previewRoot);
            if (generatedPreviewMaterial != null)
                Object.DestroyImmediate(generatedPreviewMaterial);
            Object.DestroyImmediate(renderTexture);
            preview.Cleanup();
        }

        ConfigureSpriteImporter(OutputFolder + "/" + source.OutputName + ".png");
    }

    private static Bounds CalculateRendererBounds(GameObject root)
    {
        Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
        Bounds bounds = new Bounds(root.transform.position, Vector3.zero);
        bool hasBounds = false;
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];
            if (!(renderer is MeshRenderer) && !(renderer is SkinnedMeshRenderer))
                continue;
            if (!hasBounds)
            {
                bounds = renderer.bounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(renderer.bounds);
            }
        }
        if (!hasBounds)
            throw new System.InvalidOperationException(root.name + " has no renderable weapon geometry.");
        return bounds;
    }

    private static Transform FindGunVisual(Transform root)
    {
        if (root.name == "GunVisual")
            return root;

        for (int i = 0; i < root.childCount; i++)
        {
            Transform result = FindGunVisual(root.GetChild(i));
            if (result != null)
                return result;
        }
        return null;
    }

    private static void SetPreviewRenderers(Transform gunVisual)
    {
        Renderer[] renderers = gunVisual.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].enabled = renderers[i] is MeshRenderer || renderers[i] is SkinnedMeshRenderer;
    }

    // PreviewRenderUtility does not consistently render the imported legacy Standard
    // gun materials under URP. Preserve valid production URP/Lit materials; otherwise
    // use a temporary URP/Lit material sourced from that gun model's own textures.
    private static Material ResolvePreviewMaterials(PreviewSource source, Transform gunVisual)
    {
        Renderer[] renderers = gunVisual.GetComponentsInChildren<Renderer>(true);
        Material fallback = null;
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];
            if (!(renderer is MeshRenderer) && !(renderer is SkinnedMeshRenderer))
                continue;

            Material[] materials = renderer.sharedMaterials;
            for (int materialIndex = 0; materialIndex < materials.Length; materialIndex++)
            {
                if (IsUsableUrpLitMaterial(materials[materialIndex]))
                    continue;

                if (fallback == null)
                    fallback = CreatePreviewUrpLitMaterial(source);
                materials[materialIndex] = fallback;
            }
            renderer.sharedMaterials = materials;
        }
        return fallback;
    }

    private static bool IsUsableUrpLitMaterial(Material material)
    {
        return material != null && material.shader != null &&
               material.shader.name == "Universal Render Pipeline/Lit" &&
               material.GetTexture("_BaseMap") != null;
    }

    private static Material CreatePreviewUrpLitMaterial(PreviewSource source)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
            throw new System.InvalidOperationException("Universal Render Pipeline/Lit shader was not found.");

        Texture2D diffuse = AssetDatabase.LoadAssetAtPath<Texture2D>(source.DiffusePath);
        if (diffuse == null)
            throw new FileNotFoundException("Weapon diffuse texture was not found.", source.DiffusePath);

        Material material = new Material(shader) { name = source.OutputName + " Preview URP Lit" };
        material.SetTexture("_BaseMap", diffuse);
        material.SetColor("_BaseColor", Color.white);
        Texture2D normal = AssetDatabase.LoadAssetAtPath<Texture2D>(source.NormalPath);
        if (normal != null)
        {
            material.SetTexture("_BumpMap", normal);
            material.EnableKeyword("_NORMALMAP");
        }
        return material;
    }

    private static void ConfigureSpriteImporter(string assetPath)
    {
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer == null)
            return;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.SaveAndReimport();
    }
}
