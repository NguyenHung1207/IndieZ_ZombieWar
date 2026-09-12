using System.Collections.Generic;
using UnityEngine;

/// <summary>Presentation-only model swap; gameplay remains on the authoritative Zombie prefab.</summary>
public sealed class EnemyVisualVariant : MonoBehaviour
{
    public enum MaterialProfile { None, Mutant, Fat }
    private static readonly Dictionary<string, GameObject> VisualPrefabs = new Dictionary<string, GameObject>();
    private static Material mutantMaterial;
    private static Material fatMaterial;

    public static void Preload(string resourcesPath, MaterialProfile profile = MaterialProfile.None)
    {
        LoadVisualPrefab(resourcesPath);
        if (profile != MaterialProfile.None) GetUrpLitMaterial(profile);
    }

    public void Apply(string resourcesPath, float targetHeightRatio, bool forceUrpLit = false, MaterialProfile profile = MaterialProfile.None)
    {
        GameObject visualPrefab = LoadVisualPrefab(resourcesPath);
        if (visualPrefab == null)
            return;

        Renderer[] normalRenderers = GetComponentsInChildren<Renderer>(true);
        Bounds normalBounds = GetRendererBounds(normalRenderers);

        for (int i = 0; i < normalRenderers.Length; i++) normalRenderers[i].enabled = false;

        GameObject visual = Instantiate(visualPrefab, transform);
        visual.name = "VariantVisual";
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localRotation = Quaternion.identity;
        Renderer[] variantRenderers = visual.GetComponentsInChildren<Renderer>(true);
        Bounds variantBounds = GetRendererBounds(variantRenderers);
        float normalHeight = Mathf.Max(0.01f, normalBounds.size.y);
        float variantHeight = Mathf.Max(0.01f, variantBounds.size.y);
        float scale = normalHeight * targetHeightRatio / variantHeight;
        visual.transform.localScale = Vector3.one * scale;
        variantBounds = GetRendererBounds(variantRenderers);
        visual.transform.position += Vector3.up * (normalBounds.min.y - variantBounds.min.y);
        Collider[] variantColliders = visual.GetComponentsInChildren<Collider>(true);
        for (int i = 0; i < variantColliders.Length; i++) variantColliders[i].enabled = false;

        if (forceUrpLit)
        {
            Material material = GetUrpLitMaterial(profile);
            if (material != null)
                for (int i = 0; i < variantRenderers.Length; i++) variantRenderers[i].sharedMaterial = material;
        }

        Animator variantAnimator = visual.GetComponentInChildren<Animator>(true);
        GetComponent<ZombieAnimationController>()?.BindVariantAnimator(variantAnimator);
    }

    private static Bounds GetRendererBounds(Renderer[] renderers)
    {
        Bounds bounds = new Bounds();
        bool hasBounds = false;
        foreach (Renderer renderer in renderers)
        {
            if (renderer == null) continue;
            if (!hasBounds) { bounds = renderer.bounds; hasBounds = true; }
            else bounds.Encapsulate(renderer.bounds);
        }
        return bounds;
    }

    private static GameObject LoadVisualPrefab(string resourcesPath)
    {
        if (!VisualPrefabs.TryGetValue(resourcesPath, out GameObject prefab))
        {
            prefab = Resources.Load<GameObject>(resourcesPath);
            VisualPrefabs[resourcesPath] = prefab;
        }
        return prefab;
    }

    private static Material GetUrpLitMaterial(MaterialProfile profile)
    {
        if (profile == MaterialProfile.Mutant && mutantMaterial != null) return mutantMaterial;
        if (profile == MaterialProfile.Fat && fatMaterial != null) return fatMaterial;
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) return null;
        bool fat = profile == MaterialProfile.Fat;
        Texture2D baseMap = Resources.Load<Texture2D>(fat
            ? "M24/Fat/Fat Zombie(Low Poly)/Textures/FatZombie_AlbedoMap"
            : "M24/Mutant/textures/body/T_Body_BaseColor");
        Texture2D normalMap = Resources.Load<Texture2D>(fat
            ? "M24/Fat/Fat Zombie(Low Poly)/Textures/FatZombie_Normal"
            : "M24/Mutant/textures/body/T_Body_Normal");
        Material material = new Material(shader) { name = fat ? "FatZombie_URP_Lit_Runtime" : "GiantMutant_URP_Lit_Runtime" };
        material.SetTexture("_BaseMap", baseMap);
        material.SetTexture("_BumpMap", normalMap);
        material.SetFloat("_Metallic", fat ? 0.05f : 0.1f);
        material.SetFloat("_Smoothness", fat ? 0.3f : 0.38f);
        if (fat) fatMaterial = material; else mutantMaterial = material;
        return material;
    }
}
