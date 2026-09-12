using UnityEngine;

public sealed class ProductionDressing : MonoBehaviour
{
    [SerializeField] private GameObject[] props;
    [SerializeField] private Vector3[] positions;
    [SerializeField] private Vector3[] rotations;
    [SerializeField] private Vector3[] scales;
    [SerializeField] private bool disableColliders = true;

    private void Start()
    {
        // Authored gameplay environments own their visual and collision placement.
        if (FindFirstObjectByType<InfiniteWorldController>() != null ||
            FindFirstObjectByType<Level2Environment>() != null)
        {
            gameObject.SetActive(false);
            return;
        }

        bool isMainMenuDiorama = gameObject.name == "MainMenuProductionDressing";
        if (isMainMenuDiorama)
            PrepareMainMenuStage();

        if (props == null)
            return;
        for (int i = 0; i < props.Length; i++)
        {
            GameObject prefab = props[i];
            if (prefab == null)
                continue;
            Vector3 position = positions != null && i < positions.Length ? positions[i] : transform.position;
            Quaternion rotation = rotations != null && i < rotations.Length ? Quaternion.Euler(rotations[i]) : Quaternion.identity;
            Vector3 scale = scales != null && i < scales.Length && scales[i] != Vector3.zero ? scales[i] : Vector3.one;
            GameObject instance = Instantiate(prefab, position, rotation, transform);
            instance.transform.localScale = scale;
            if (isMainMenuDiorama) instance.SetActive(true);
            if (disableColliders)
            {
                foreach (Collider collider in instance.GetComponentsInChildren<Collider>(true))
                    collider.enabled = false;
            }
        }
    }

    private static void PrepareMainMenuStage()
    {
        Camera camera = Camera.main;
        if (camera != null)
        {
            camera.transform.position = new Vector3(0f, 2.05f, -9.5f);
            camera.transform.LookAt(new Vector3(2.15f, 1.15f, 5.6f));
            camera.fieldOfView = 48f;
            camera.backgroundColor = new Color(0.018f, 0.022f, 0.024f, 1f);
        }

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = new Color(0.20f, 0.23f, 0.25f);
        RenderSettings.ambientEquatorColor = new Color(0.095f, 0.105f, 0.105f);
        RenderSettings.ambientGroundColor = new Color(0.025f, 0.026f, 0.024f);
        RenderSettings.ambientIntensity = 0.72f;
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogColor = new Color(0.055f, 0.06f, 0.06f);
        RenderSettings.fogStartDistance = 17f;
        RenderSettings.fogEndDistance = 38f;

        CreateStageLight("MenuKeyLight", LightType.Directional, new Vector3(42f, -32f, 0f),
            new Color(1f, 0.80f, 0.60f), 1.35f, 0f);
        CreateStageLight("MenuRimLight", LightType.Point, new Vector3(4.8f, 3.1f, 2.2f),
            new Color(0.36f, 0.54f, 0.68f), 4.2f, 9f);
        CreateStageLight("MenuWarmPractical", LightType.Point, new Vector3(-1.4f, 1.5f, 5.2f),
            new Color(1f, 0.31f, 0.11f), 2.4f, 6.5f);

        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "MenuDioramaGround";
        ground.transform.position = new Vector3(2f, -0.035f, 7f);
        ground.transform.localScale = new Vector3(2.7f, 1f, 2.9f);
        Collider groundCollider = ground.GetComponent<Collider>();
        if (groundCollider != null) groundCollider.enabled = false;
        Renderer renderer = ground.GetComponent<Renderer>();
        Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        if (renderer != null && shader != null)
        {
            Material material = new Material(shader) { name = "Menu Ground (Runtime)" };
            material.color = new Color(0.10f, 0.085f, 0.066f, 1f);
            renderer.material = material;
        }
    }

    private static void CreateStageLight(string name, LightType type, Vector3 positionOrEuler, Color color, float intensity, float range)
    {
        GameObject lightObject = new GameObject(name, typeof(Light));
        Light light = lightObject.GetComponent<Light>();
        light.type = type;
        light.color = color;
        light.intensity = intensity;
        light.range = range;
        light.shadows = type == LightType.Directional ? LightShadows.Soft : LightShadows.None;
        if (type == LightType.Directional)
            lightObject.transform.rotation = Quaternion.Euler(positionOrEuler);
        else
            lightObject.transform.position = positionOrEuler;
    }

    public GameObject FindProp(string prefabName)
    {
        if (props == null) return null;
        for (int index = 0; index < props.Length; index++)
            if (props[index] != null && props[index].name == prefabName)
                return props[index];
        return null;
    }
}
