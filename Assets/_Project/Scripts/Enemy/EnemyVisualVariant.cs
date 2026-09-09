using UnityEngine;

/// <summary>Presentation-only model swap; gameplay remains on the authoritative Zombie prefab.</summary>
public sealed class EnemyVisualVariant : MonoBehaviour
{
    public enum MaterialProfile { None, Mutant, Fat }

    public void Apply(string resourcesPath, float targetHeightRatio, bool forceUrpLit = false, MaterialProfile profile = MaterialProfile.None)
    {
        GameObject visualPrefab = Resources.Load<GameObject>(resourcesPath);
        if (visualPrefab == null)
            return;

        Bounds normalBounds = GetRendererBounds(GetComponentsInChildren<Renderer>(true));

        foreach (Renderer renderer in GetComponentsInChildren<Renderer>(true))
            renderer.enabled = false;

        GameObject visual = Instantiate(visualPrefab, transform);
        visual.name = "VariantVisual";
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localRotation = Quaternion.identity;
        Bounds variantBounds = GetRendererBounds(visual.GetComponentsInChildren<Renderer>(true));
        float normalHeight = Mathf.Max(0.01f, normalBounds.size.y);
        float variantHeight = Mathf.Max(0.01f, variantBounds.size.y);
        float scale = normalHeight * targetHeightRatio / variantHeight;
        visual.transform.localScale = Vector3.one * scale;
        variantBounds = GetRendererBounds(visual.GetComponentsInChildren<Renderer>(true));
        visual.transform.position += Vector3.up * (normalBounds.min.y - variantBounds.min.y);
        foreach (Collider collider in visual.GetComponentsInChildren<Collider>(true))
            collider.enabled = false;

        if (forceUrpLit)
            ApplyUrpLitMaterial(visual, profile);

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

    private static void ApplyUrpLitMaterial(GameObject visual, MaterialProfile profile)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) return;
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
        foreach (Renderer renderer in visual.GetComponentsInChildren<Renderer>(true))
            renderer.sharedMaterial = material;
    }
}
