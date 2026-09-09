using UnityEngine;

/// <summary>URP-safe presentation for selected Wasteland LITE decoration.</summary>
public static class WastelandVisual
{
    private static Material material;

    public static void Prepare(GameObject prop)
    {
        if (material == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader != null)
            {
                material = new Material(shader) { name = "Wasteland_URP_Lit_Runtime" };
                material.SetTexture("_BaseMap", Resources.Load<Texture2D>("Wasteland/Textures/Walls_Map_1A"));
                material.SetTexture("_BumpMap", Resources.Load<Texture2D>("Wasteland/Textures/Walls_Map_1A_normal"));
                material.SetFloat("_Metallic", 0.08f);
                material.SetFloat("_Smoothness", 0.28f);
            }
        }
        foreach (Collider collider in prop.GetComponentsInChildren<Collider>(true)) collider.enabled = false;
        foreach (Renderer renderer in prop.GetComponentsInChildren<Renderer>(true))
        {
            if (material != null) renderer.sharedMaterial = material;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
        }
    }
}
