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
        {
            PrepareMainMenuStage();
            BuildMainMenuVignette();
            return;
        }

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
            camera.transform.LookAt(new Vector3(3.9f, 1.2f, 7.0f));
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
        RenderSettings.fogStartDistance = 18f;
        RenderSettings.fogEndDistance = 36f;

        CreateStageLight("MenuKeyLight", LightType.Directional, new Vector3(42f, -32f, 0f),
            new Color(1f, 0.80f, 0.60f), 1.35f, 0f);
        CreateStageLight("MenuRimLight", LightType.Point, new Vector3(4.8f, 3.1f, 2.2f),
            new Color(0.36f, 0.54f, 0.68f), 4.2f, 9f);
        CreateStageLight("MenuWarmPractical", LightType.Point, new Vector3(4.8f, 1.8f, 6.4f),
            new Color(1f, 0.38f, 0.14f), 1.7f, 6.5f);

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

    // Keep the menu to one readable chase beat on the clear right side: a
    // foreground survivor fleeing with a rifle while three zombies pursue.
    private void BuildMainMenuVignette()
    {
        Transform root = new GameObject("MenuCombatVignette").transform;
        root.SetParent(transform, false);

        GameObject soldier = FindMenuActor("soldier", "survivalist", "player");
        GameObject zombie = FindMenuActor("zombie");
        GameObject tent = FindMenuActor("tent");
        GameObject rifle = FindMenuActor("rifle", "assault");
        GameObject soldierInstance = AddMenuActor(root, soldier, "MenuSoldier", new Vector3(4.0f, 0f, 5.8f), 165f, 1.22f);
        AttachRifleIfNeeded(soldierInstance, rifle);
        AddMenuActor(root, zombie, "MenuZombie_1", new Vector3(5.2f, 0f, 7.4f), 178f, 1f);
        AddMenuActor(root, zombie, "MenuZombie_2", new Vector3(3.1f, 0f, 8.3f), 164f, 0.94f);
        AddMenuActor(root, zombie, "MenuZombie_3", new Vector3(6.4f, 0f, 9.0f), -166f, 0.90f);

        AddMenuActor(root, tent, "MenuTent", new Vector3(8.1f, 0f, 8.1f), -24f, 0.72f);
        AddMenuResourceProp(root, "Prefabs/Props, Misc/Barrier_1B", new Vector3(8.8f, 0f, 10.7f), 14f, 0.58f);
    }

    private GameObject FindMenuActor(params string[] nameFragments)
    {
        if (props == null)
            return null;
        for (int index = 0; index < props.Length; index++)
        {
            GameObject candidate = props[index];
            if (candidate == null)
                continue;
            string name = candidate.name.ToLowerInvariant();
            for (int fragment = 0; fragment < nameFragments.Length; fragment++)
                if (name.Contains(nameFragments[fragment]))
                    return candidate;
        }

        // Some imported soldier prefabs have an asset-specific display name.
        // In the menu dressing list, the remaining animated non-environment
        // actor is the authored player character.
        if (ContainsFragment(nameFragments, "player"))
        {
            for (int index = 0; index < props.Length; index++)
            {
                GameObject candidate = props[index];
                if (candidate == null || candidate.GetComponentInChildren<Animator>(true) == null)
                    continue;
                string name = candidate.name.ToLowerInvariant();
                if (!name.Contains("zombie") && !name.Contains("tent") && !name.Contains("barrier") &&
                    !name.Contains("crate") && !name.Contains("rifle"))
                {
                    return candidate;
                }
            }
        }
        return null;
    }

    private static GameObject AddMenuActor(Transform root, GameObject prefab, string name, Vector3 position, float yaw, float scale)
    {
        if (prefab == null)
            return null;
        GameObject actor = Instantiate(prefab, position, Quaternion.Euler(0f, yaw, 0f), root);
        actor.name = name;
        actor.SetActive(true);
        actor.transform.localScale *= scale;
        foreach (Collider collider in actor.GetComponentsInChildren<Collider>(true))
            collider.enabled = false;
        EnsureVisualHierarchy(actor);
        DisableMenuGameplayBehaviours(actor);
        if (actor.GetComponentInChildren<Animator>(true) != null)
            actor.AddComponent<MenuRunPose>();
        return actor;
    }

    private static void EnsureVisualHierarchy(GameObject actor)
    {
        foreach (Renderer renderer in actor.GetComponentsInChildren<Renderer>(true))
        {
            // Imported Survivalist prefabs can keep their render child inactive.
            // Reactivate its chain, enable the renderer, and use Default so the
            // MainMenu camera's standard culling mask always includes it.
            for (Transform current = renderer.transform; current != null; current = current.parent)
            {
                current.gameObject.SetActive(true);
                current.gameObject.layer = 0;
                if (current == actor.transform)
                    break;
            }
            renderer.enabled = true;
        }
    }

    private static bool ContainsFragment(string[] fragments, string target)
    {
        for (int index = 0; index < fragments.Length; index++)
            if (fragments[index] == target)
                return true;
        return false;
    }

    private static void DisableMenuGameplayBehaviours(GameObject actor)
    {
        foreach (ZombieAI zombie in actor.GetComponentsInChildren<ZombieAI>(true)) zombie.enabled = false;
        foreach (ZombieHealth health in actor.GetComponentsInChildren<ZombieHealth>(true)) health.enabled = false;
        foreach (ZombieAnimationController animation in actor.GetComponentsInChildren<ZombieAnimationController>(true)) animation.enabled = false;
        foreach (PlayerMovement movement in actor.GetComponentsInChildren<PlayerMovement>(true)) movement.enabled = false;
        foreach (PlayerWeaponController weapon in actor.GetComponentsInChildren<PlayerWeaponController>(true)) weapon.enabled = false;
        foreach (PlayerAnimationController animation in actor.GetComponentsInChildren<PlayerAnimationController>(true)) animation.enabled = false;
    }

    private static void AttachRifleIfNeeded(GameObject soldier, GameObject riflePrefab)
    {
        if (soldier == null || riflePrefab == null || HasNamedChild(soldier.transform, "rifle", "weapon", "gun"))
            return;

        Transform rightHand = FindNamedChild(soldier.transform, "righthand", "right_hand", "hand_r");
        if (rightHand == null)
            return;

        GameObject rifle = Instantiate(riflePrefab, rightHand);
        rifle.name = "MenuRifle";
        rifle.transform.localPosition = new Vector3(0.05f, 0f, 0.16f);
        rifle.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
        rifle.transform.localScale = Vector3.one;
        foreach (Collider collider in rifle.GetComponentsInChildren<Collider>(true))
            collider.enabled = false;
    }

    private static bool HasNamedChild(Transform root, params string[] nameFragments) => FindNamedChild(root, nameFragments) != null;

    private static Transform FindNamedChild(Transform root, params string[] nameFragments)
    {
        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
        {
            string name = child.name.ToLowerInvariant();
            for (int index = 0; index < nameFragments.Length; index++)
                if (name.Contains(nameFragments[index]))
                    return child;
        }
        return null;
    }

    private static void AddMenuResourceProp(Transform root, string path, Vector3 position, float yaw, float scale)
    {
        GameObject prefab = Resources.Load<GameObject>("Wasteland/" + path);
        if (prefab == null)
            return;

        GameObject prop = Instantiate(prefab, position, Quaternion.Euler(0f, yaw, 0f), root);
        prop.name = "Menu_" + prefab.name;
        prop.transform.localScale *= scale;
        // WastelandVisual resolves the legacy source materials through its
        // cached URP-safe shared materials, matching gameplay dressing and
        // preventing pink legacy-shader fallbacks in the menu.
        WastelandVisual.Prepare(prop, position.y);
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

/// <summary>Menu-only animation driver; it keeps the chase readable without live AI/navigation.</summary>
public sealed class MenuRunPose : MonoBehaviour
{
    private static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
    private Animator[] animators;

    private void Awake()
    {
        animators = GetComponentsInChildren<Animator>(true);
    }

    private void Update()
    {
        for (int index = 0; index < animators.Length; index++)
        {
            Animator animator = animators[index];
            if (animator != null && animator.runtimeAnimatorController != null)
                animator.SetFloat(MoveSpeedHash, 0.85f);
        }
    }
}
