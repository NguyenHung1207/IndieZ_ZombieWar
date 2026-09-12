using UnityEngine;

/// <summary>URP-safe presentation for selected Wasteland LITE decoration.</summary>
public static class WastelandVisual
{
    private static Material wallsMaterial;
    private static Material structuresMaterial;
    private static Material metalsMaterial;
    private static Material tapestryMaterial;

    public static float Prepare(GameObject prop, bool castMajorShadow = false)
    {
        WastelandRuntimeVisual cached = prop.GetComponent<WastelandRuntimeVisual>();
        if (cached != null) return cached.GroundOffset;
        EnsureMaterials();
        Collider[] colliders = prop.GetComponentsInChildren<Collider>(true);
        Renderer[] renderers = prop.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < colliders.Length; i++) colliders[i].enabled = false;
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];
            Material resolved = ResolveMaterial(prop.name, renderer.sharedMaterial != null ? renderer.sharedMaterial.name : string.Empty);
            if (resolved != null) renderer.sharedMaterial = resolved;
            renderer.shadowCastingMode = castMajorShadow
                ? UnityEngine.Rendering.ShadowCastingMode.On
                : UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = castMajorShadow;
            renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
        }

        float groundOffset = 0f;
        if (renderers.Length > 0)
        {
            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++) bounds.Encapsulate(renderers[i].bounds);
            float scaleY = Mathf.Max(0.0001f, Mathf.Abs(prop.transform.lossyScale.y));
            groundOffset = (prop.transform.position.y - bounds.min.y) / scaleY;
        }

        cached = prop.AddComponent<WastelandRuntimeVisual>();
        cached.GroundOffset = groundOffset;
        return groundOffset;
    }

    public static void Prepare(GameObject prop, float groundY, bool castMajorShadow = false)
    {
        float groundOffset = Prepare(prop, castMajorShadow);
        Vector3 position = prop.transform.position;
        position.y = groundY + groundOffset * Mathf.Abs(prop.transform.lossyScale.y);
        prop.transform.position = position;
    }

    private static void EnsureMaterials()
    {
        if (wallsMaterial != null) return;
        wallsMaterial = CreateMaterial("Wasteland_Walls_URP", "Walls_Map_1A", "Walls_Map_1A_normal", "Walls_Map_1A_occlusion", 0.03f, 0.18f);
        structuresMaterial = CreateMaterial("Wasteland_Structures_URP", "Structures_Map_1A", "Structures_Map_1A Normal", "Structures_Map_1A Occlusion", 0.08f, 0.22f);
        metalsMaterial = CreateMaterial("Wasteland_Metals_URP", "Metals_Map_1A", "Metals_Map_1A_ Normal", "Metals_Map_1A_occlusion", 0.42f, 0.28f);
        tapestryMaterial = CreateMaterial("Wasteland_Tapestry_URP", "Tapestry_1A", "Tapestry_1A Normal", "Tapestry_1A Occlusion", 0f, 0.14f);
    }

    private static Material CreateMaterial(string name, string baseMap, string normalMap, string occlusionMap, float metallic, float smoothness)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) return null;
        Material result = new Material(shader) { name = name };
        Texture2D albedo = Resources.Load<Texture2D>("Wasteland/Textures/" + baseMap);
        Texture2D normal = Resources.Load<Texture2D>("Wasteland/Textures/" + normalMap);
        Texture2D occlusion = Resources.Load<Texture2D>("Wasteland/Textures/" + occlusionMap);
        if (albedo != null) result.SetTexture("_BaseMap", albedo);
        if (normal != null)
        {
            result.SetTexture("_BumpMap", normal);
            result.EnableKeyword("_NORMALMAP");
        }
        if (occlusion != null) result.SetTexture("_OcclusionMap", occlusion);
        result.SetFloat("_Metallic", metallic);
        result.SetFloat("_Smoothness", smoothness);
        return result;
    }

    private static Material ResolveMaterial(string propName, string sourceMaterialName)
    {
        string key = (propName + " " + sourceMaterialName).ToLowerInvariant();
        if (key.Contains("tapestry") || key.Contains("awning")) return tapestryMaterial;
        if (key.Contains("barrier") || key.Contains("metal")) return metalsMaterial;
        if (key.Contains("fortified") || key.Contains("shanty") || key.Contains("structure")) return structuresMaterial;
        return wallsMaterial;
    }
}

public sealed class WastelandRuntimeVisual : MonoBehaviour
{
    public float GroundOffset { get; set; }
}
