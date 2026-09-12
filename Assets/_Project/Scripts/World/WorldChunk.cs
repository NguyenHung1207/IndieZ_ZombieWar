using System.Collections.Generic;
using UnityEngine;

public sealed class WorldChunk : MonoBehaviour
{
    private const int DefaultLayer = 0;
    private const float NormalBlockerClearance = 4f;
    private const float LargeBlockerClearance = 6f;

    private sealed class CollisionProxy
    {
        public readonly GameObject Root;
        public readonly List<BoxCollider> Boxes = new List<BoxCollider>(3);

        public CollisionProxy(GameObject root) => Root = root;
    }
    private static readonly Dictionary<string, GameObject> WastelandPrefabs = new Dictionary<string, GameObject>();

    public Vector2Int Coordinate { get; private set; }
    private float size;
    private Material groundMaterial;
    private GameObject[] props;
    private Transform visuals;
    private Transform collision;
    private Transform player;
    private GameObject floor;
    private readonly Dictionary<GameObject, List<GameObject>> militaryVisualPool = new Dictionary<GameObject, List<GameObject>>();
    private readonly Dictionary<GameObject, int> militaryVisualUseCounts = new Dictionary<GameObject, int>();
    private readonly Dictionary<string, List<GameObject>> wastelandVisualPool = new Dictionary<string, List<GameObject>>();
    private readonly Dictionary<string, int> wastelandVisualUseCounts = new Dictionary<string, int>();
    private readonly List<CollisionProxy> collisionPool = new List<CollisionProxy>();
    private int usedCollisionCount;

    public void Configure(Vector2Int coordinate, float chunkSize, Material material, GameObject[] propPrefabs, int layout)
    {
        Coordinate = coordinate;
        size = chunkSize;
        groundMaterial = material;
        props = propPrefabs;
        transform.position = new Vector3(coordinate.x * size, 0f, coordinate.y * size);
        transform.localScale = Vector3.one;
        EnsureInitialized();
        ResetPools();
        if (player == null)
        {
            PlayerMovement movement = FindFirstObjectByType<PlayerMovement>();
            player = movement != null ? movement.transform : null;
        }

        floor.transform.localPosition = new Vector3(size * 0.5f, -0.12f, size * 0.5f);
        floor.transform.localScale = new Vector3(size, 0.2f, size);
        if (groundMaterial != null)
            floor.GetComponent<Renderer>().sharedMaterial = groundMaterial;
        ApplyLayout(layout);
        ApplyWastelandDressing(layout);
    }

    private void EnsureInitialized()
    {
        if (visuals == null) visuals = CreateContainer("Visuals");
        if (collision == null) collision = CreateContainer("Collision");
        if (floor != null) return;
        floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "ChunkGround";
        floor.transform.SetParent(transform, false);
    }

    private void ResetPools()
    {
        militaryVisualUseCounts.Clear();
        wastelandVisualUseCounts.Clear();
        usedCollisionCount = 0;
        SetPoolActive(militaryVisualPool, false);
        SetPoolActive(wastelandVisualPool, false);
        for (int i = 0; i < collisionPool.Count; i++) collisionPool[i].Root.SetActive(false);
    }

    private static void SetPoolActive<TKey>(Dictionary<TKey, List<GameObject>> pool, bool active)
    {
        foreach (List<GameObject> instances in pool.Values)
            for (int i = 0; i < instances.Count; i++) instances[i].SetActive(active);
    }

    private Transform CreateContainer(string containerName)
    {
        GameObject container = new GameObject(containerName);
        container.layer = DefaultLayer;
        container.transform.SetParent(transform, false);
        container.transform.localPosition = Vector3.zero;
        container.transform.localRotation = Quaternion.identity;
        container.transform.localScale = Vector3.one;
        return container.transform;
    }

    private void ApplyLayout(int layout)
    {
        switch (layout % 6)
        {
            case 0:
                Place(0, -0.15f, 0.2f, 90f);
                Place(1, -0.05f, 0.25f, 0f);
                Place(3, -0.25f, 0.11f, 0f);
                break;
            case 1:
                Place(4, -0.18f, -0.17f, 90f);
                Place(5, -0.02f, -0.23f, 90f);
                Place(1, -0.03f, -0.1f, 0f);
                Place(2, -0.28f, -0.04f, 0f);
                break;
            case 2:
                Place(8, 0.18f, 0.12f, 270f);
                Place(0, 0.28f, 0.18f, 90f);
                Place(1, 0.04f, 0.2f, 0f);
                Place(3, 0.29f, 0.02f, 0f);
                break;
            case 3:
                Place(6, -0.2f, 0.16f, 90f);
                Place(5, -0.04f, 0.22f, 90f);
                Place(1, -0.04f, 0.08f, 0f);
                Place(0, -0.29f, 0.05f, 0f);
                break;
            case 4:
                Place(7, 0.18f, 0.18f, 0f);
                Place(0, 0.03f, 0.22f, 90f);
                Place(1, 0.04f, 0.08f, 0f);
                Place(3, 0.29f, 0.04f, 0f);
                break;
            default:
                Place(5, 0.2f, -0.16f, 90f);
                Place(1, 0.06f, -0.2f, 0f);
                Place(2, 0.27f, -0.06f, 0f);
                Place(3, 0.06f, -0.04f, 0f);
                break;
        }
    }

    // Decoration only: WorldChunk remains the sole authority for Level 1
    // ground and explicit collision proxies, so this cannot introduce slopes
    // or imported collider behaviour.
    private void ApplyWastelandDressing(int layout)
    {
        switch (layout % 6)
        {
            case 0: // Abandoned Checkpoint
                PlaceWasteland("Prefabs/Fortified_Walls/Fortified_Wall_1B", new Vector3(-10f, 0f, 11f), 90f, 0.7f);
                PlaceWasteland("Prefabs/Props, Misc/Barrier_1B", new Vector3(-5f, 0f, 10f), 90f, 0.64f);
                PlaceWasteland("Prefabs/Walls, Blocks/Concrete_Block_1B", new Vector3(-11f, 0f, 5f), 90f, 0.56f);
                break;
            case 1: // Supply Camp
                PlaceWasteland("Prefabs/Walls, Blocks/Wall_Broken_1B", new Vector3(-12f, 0f, -11f), 0f, 0.72f);
                PlaceWasteland("Prefabs/Props, Misc/Shanty_Wall_Board_1A", new Vector3(-12f, 0f, -5f), 0f, 0.68f);
                PlaceWasteland("Prefabs/Props, Misc/Barrier_1A", new Vector3(-3f, 0f, -11f), 90f, 0.62f);
                break;
            case 2: // Wrecked Convoy
                PlaceWasteland("Prefabs/Fortified_Walls/Fortified_Wall_1C", new Vector3(11f, 0f, 10f), 90f, 0.66f);
                PlaceWasteland("Prefabs/Walls, Blocks/Concrete_Block_1D", new Vector3(4f, 0f, 10f), 0f, 0.58f);
                PlaceWasteland("Prefabs/Props, Misc/Barrier_1C", new Vector3(12f, 0f, 1f), 90f, 0.6f);
                break;
            case 3: // Radio Outpost
                PlaceWasteland("Prefabs/Walls, Blocks/Wall_1A_Window_1", new Vector3(-11f, 0f, 11f), 0f, 0.68f);
                PlaceWasteland("Prefabs/Walls, Blocks/Wall_Broken_1C", new Vector3(-4f, 0f, 11f), 0f, 0.62f);
                PlaceWasteland("Prefabs/Props, Misc/Shanty_Wall_Board_1B", new Vector3(-12f, 0f, 4f), 0f, 0.62f);
                break;
            case 4: // Defensive Line
                PlaceWasteland("Prefabs/Fortified_Walls/Fortified_Wall_2A", new Vector3(10f, 0f, 12f), 90f, 0.68f);
                PlaceWasteland("Prefabs/Props, Misc/Barrier_1A", new Vector3(4f, 0f, 11f), 90f, 0.62f);
                PlaceWasteland("Prefabs/Walls, Blocks/Concrete_Block_1E", new Vector3(11f, 0f, 5f), 90f, 0.54f);
                break;
            default: // Scrap Yard
                PlaceWasteland("Prefabs/Fortified_Walls/Fortified_Wall_1D", new Vector3(11f, 0f, -11f), 90f, 0.64f);
                PlaceWasteland("Prefabs/Walls, Blocks/Concrete_Block_1F", new Vector3(3f, 0f, -10f), 0f, 0.52f);
                PlaceWasteland("Prefabs/Props, Misc/Barrier_1B", new Vector3(12f, 0f, -2f), 90f, 0.58f);
                break;
        }
    }

    private void PlaceWasteland(string path, Vector3 localPosition, float yaw, float scale)
    {
        if (!WastelandPrefabs.TryGetValue(path, out GameObject prefab))
        {
            prefab = Resources.Load<GameObject>("Wasteland/" + path);
            WastelandPrefabs[path] = prefab;
        }
        if (prefab == null) return;
        GameObject prop = RentVisual(path, prefab, wastelandVisualPool, wastelandVisualUseCounts, out _);
        Vector3 centeredPosition = localPosition + new Vector3(size * 0.5f, 0f, size * 0.5f);
        prop.transform.localPosition = centeredPosition;
        prop.transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
        prop.transform.localScale = Vector3.one * scale;
        float groundOffset = WastelandVisual.Prepare(prop);
        prop.transform.localPosition = centeredPosition + Vector3.up * (groundOffset * scale);
        EnvironmentCollisionProfiles.Shape[] shapes = EnvironmentCollisionProfiles.Get(prefab.name);
        if (shapes != null) ConfigureCollisionProxy(prop, shapes);
    }

    private void Place(int index, float x, float z, float yaw)
    {
        if (props == null || index < 0 || index >= props.Length || props[index] == null)
            return;

        Vector3 localPosition = new Vector3((0.5f + x) * size, 0f, (0.5f + z) * size);
        Quaternion localRotation = Quaternion.Euler(0f, yaw, 0f);
        EnvironmentCollisionProfiles.Shape[] shapes = EnvironmentCollisionProfiles.Get(props[index].name);
        if (shapes != null && OverlapsPlayerSafetyZone(shapes, localPosition, localRotation))
            return;

        GameObject visual = RentVisual(props[index], props[index], militaryVisualPool, militaryVisualUseCounts, out bool created);
        visual.transform.localPosition = localPosition;
        visual.transform.localRotation = localRotation;
        visual.transform.localScale = Vector3.one;
        if (created)
        {
            foreach (Transform child in visual.GetComponentsInChildren<Transform>(true)) child.gameObject.layer = DefaultLayer;
            foreach (Collider importedCollider in visual.GetComponentsInChildren<Collider>(true)) importedCollider.enabled = false;
        }

        if (shapes != null)
            ConfigureCollisionProxy(visual, shapes);
    }

    private GameObject RentVisual<TKey>(TKey key, GameObject prefab, Dictionary<TKey, List<GameObject>> pool,
        Dictionary<TKey, int> useCounts, out bool created)
    {
        if (!pool.TryGetValue(key, out List<GameObject> instances))
        {
            instances = new List<GameObject>();
            pool.Add(key, instances);
        }
        useCounts.TryGetValue(key, out int usedCount);
        if (usedCount < instances.Count)
        {
            GameObject reused = instances[usedCount];
            useCounts[key] = usedCount + 1;
            reused.SetActive(true);
            created = false;
            return reused;
        }
        GameObject instance = Instantiate(prefab, visuals);
        instance.name = VisualName(prefab.name, usedCount);
        instance.layer = DefaultLayer;
        instances.Add(instance);
        useCounts[key] = usedCount + 1;
        created = true;
        return instance;
    }

    private static string VisualName(string prefabName, int usedCount)
    {
        return usedCount == 0 ? prefabName + "_Visual" : prefabName + "_" + (usedCount + 1) + "_Visual";
    }

    private bool OverlapsPlayerSafetyZone(EnvironmentCollisionProfiles.Shape[] shapes, Vector3 localPosition, Quaternion localRotation)
    {
        if (player == null)
            return false;

        float clearance = EnvironmentCollisionProfiles.IsLarge(shapes) ? LargeBlockerClearance : NormalBlockerClearance;
        Vector3 playerPosition = player.position;
        for (int i = 0; i < shapes.Length; i++)
        {
            Vector3 worldCenter = transform.TransformPoint(localPosition + localRotation * shapes[i].Center);
            Vector3 delta = worldCenter - playerPosition;
            delta.y = 0f;
            float shapeRadius = 0.5f * Mathf.Sqrt(shapes[i].Size.x * shapes[i].Size.x + shapes[i].Size.z * shapes[i].Size.z);
            float safeDistance = clearance + shapeRadius;
            if (delta.sqrMagnitude < safeDistance * safeDistance)
                return true;
        }
        return false;
    }

    private void ConfigureCollisionProxy(GameObject visual, EnvironmentCollisionProfiles.Shape[] shapes)
    {
        CollisionProxy proxy;
        if (usedCollisionCount < collisionPool.Count)
        {
            proxy = collisionPool[usedCollisionCount];
            proxy.Root.SetActive(true);
        }
        else
        {
            GameObject root = new GameObject();
            root.layer = DefaultLayer;
            root.transform.SetParent(collision, false);
            proxy = new CollisionProxy(root);
            collisionPool.Add(proxy);
        }
        usedCollisionCount++;
        proxy.Root.name = visual.name.Substring(0, visual.name.Length - "_Visual".Length) + "_Collision";
        proxy.Root.transform.localPosition = visual.transform.localPosition;
        proxy.Root.transform.localRotation = visual.transform.localRotation;
        proxy.Root.transform.localScale = visual.transform.localScale;

        for (int index = 0; index < shapes.Length; index++)
        {
            BoxCollider box;
            if (index < proxy.Boxes.Count)
            {
                box = proxy.Boxes[index];
                box.gameObject.SetActive(true);
            }
            else
            {
                GameObject shape = new GameObject(index == 0 ? "BodyCollider" : "BodyCollider_" + (index + 1), typeof(BoxCollider));
                shape.layer = DefaultLayer;
                shape.transform.SetParent(proxy.Root.transform, false);
                box = shape.GetComponent<BoxCollider>();
                proxy.Boxes.Add(box);
            }
            box.transform.localPosition = Vector3.zero;
            box.transform.localRotation = Quaternion.identity;
            box.transform.localScale = Vector3.one;
            box.center = shapes[index].Center;
            box.size = shapes[index].Size;
            box.enabled = true;
            box.isTrigger = false;
        }
        for (int index = shapes.Length; index < proxy.Boxes.Count; index++)
            proxy.Boxes[index].gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (collision == null)
            return;

        BoxCollider[] boxes = collision.GetComponentsInChildren<BoxCollider>();
        Matrix4x4 previousMatrix = Gizmos.matrix;
        Color previousColor = Gizmos.color;
        Gizmos.color = Color.cyan;
        for (int i = 0; i < boxes.Length; i++)
        {
            BoxCollider box = boxes[i];
            if (box == null || !box.enabled)
                continue;
            Gizmos.matrix = box.transform.localToWorldMatrix;
            Gizmos.DrawWireCube(box.center, box.size);
        }
        Gizmos.matrix = previousMatrix;
        Gizmos.color = previousColor;
    }
#endif

}
