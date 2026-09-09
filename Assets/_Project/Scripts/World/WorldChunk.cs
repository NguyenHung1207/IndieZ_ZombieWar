using UnityEngine;

public sealed class WorldChunk : MonoBehaviour
{
    private const int DefaultLayer = 0;
    private const float NormalBlockerClearance = 4f;
    private const float LargeBlockerClearance = 6f;

    private enum CollisionProfileType { None, Barrier, Box, Barrel, Tent, Generator, RadioStation, Tower, Hummer }

    private struct CollisionShape
    {
        public readonly Vector3 Center;
        public readonly Vector3 Size;

        public CollisionShape(Vector3 center, Vector3 size)
        {
            Center = center;
            Size = size;
        }
    }

    public Vector2Int Coordinate { get; private set; }
    private float size;
    private Material groundMaterial;
    private GameObject[] props;
    private Transform visuals;
    private Transform collision;
    private Transform player;

    public void Configure(Vector2Int coordinate, float chunkSize, Material material, GameObject[] propPrefabs, int layout)
    {
        Coordinate = coordinate;
        size = chunkSize;
        groundMaterial = material;
        props = propPrefabs;
        transform.localScale = Vector3.one;
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);

        transform.position = new Vector3(coordinate.x * size, 0f, coordinate.y * size);
        visuals = CreateContainer("Visuals");
        collision = CreateContainer("Collision");
        if (player == null)
        {
            PlayerMovement movement = FindFirstObjectByType<PlayerMovement>();
            player = movement != null ? movement.transform : null;
        }

        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "ChunkGround";
        floor.transform.SetParent(transform, false);
        floor.transform.localPosition = new Vector3(size * 0.5f, -0.12f, size * 0.5f);
        floor.transform.localScale = new Vector3(size, 0.2f, size);
        if (groundMaterial != null)
            floor.GetComponent<Renderer>().sharedMaterial = groundMaterial;
        ApplyLayout(layout);
        ApplyWastelandDressing(layout);
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
            case 0: Place(1, 0.28f, 0.25f); Place(2, -0.25f, -0.2f); Place(3, 0.28f, -0.28f); break;
            case 1: Place(0, -0.28f, 0.18f); Place(0, -0.28f, -0.18f); Place(8, 0.22f, 0f); Place(2, 0.30f, -0.25f); break;
            case 2: Place(4, -0.25f, 0.2f); Place(5, 0.25f, 0.15f); Place(1, 0.2f, -0.25f); Place(2, -0.25f, -0.25f); break;
            case 3: Place(6, 0.18f, 0.18f); Place(1, -0.22f, 0.22f); Place(2, -0.25f, -0.25f); break;
            case 4: Place(0, -0.28f, -0.2f); Place(7, 0.22f, 0.2f); Place(1, 0.22f, -0.28f); Place(3, -0.2f, 0.28f); break;
            default: Place(7, 0f, 0f); Place(1, 0.3f, -0.25f); Place(2, -0.28f, 0.25f); break;
        }
    }

    // Decoration only: WorldChunk remains the sole authority for Level 1
    // ground and explicit collision proxies, so this cannot introduce slopes
    // or imported collider behaviour.
    private void ApplyWastelandDressing(int layout)
    {
        switch (layout % 6)
        {
            case 0: PlaceWasteland("Prefabs/Fortified_Walls/Fortified_Wall_1A", new Vector3(-12f, 0f, 14f), 90f, 0.8f); break; // checkpoint
            case 1: PlaceWasteland("Prefabs/Walls, Blocks/Wall_Broken_1A", new Vector3(13f, 0f, -10f), 0f, 0.85f); break; // supply camp
            case 2: PlaceWasteland("Prefabs/Walls, Blocks/Concrete_Block_1A", new Vector3(-13f, 0f, 11f), 30f, 1.1f); break; // convoy cover
            case 3: PlaceWasteland("Prefabs/Props, Misc/Barrier_1A", new Vector3(11f, 0f, 13f), -20f, 1f); break; // defense line
            case 4: PlaceWasteland("Prefabs/Walls, Blocks/Wall_Broken_1A", new Vector3(-11f, 0f, -13f), 180f, 0.8f); break; // radio outpost
            default: PlaceWasteland("Prefabs/Walls, Blocks/Concrete_Block_1A", new Vector3(12f, 0f, 12f), -25f, 0.9f); break; // scrap yard
        }
    }

    private void PlaceWasteland(string path, Vector3 localPosition, float yaw, float scale)
    {
        GameObject prefab = Resources.Load<GameObject>("Wasteland/" + path);
        if (prefab == null) return;
        GameObject prop = Instantiate(prefab, visuals);
        prop.name = "Wasteland_" + prefab.name + "_Decor";
        prop.transform.localPosition = localPosition;
        prop.transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
        prop.transform.localScale = Vector3.one * scale;
        WastelandVisual.Prepare(prop);
    }

    private void Place(int index, float x, float z)
    {
        if (props == null || index < 0 || index >= props.Length || props[index] == null)
            return;

        Vector3 localPosition = new Vector3(x * size, 0f, z * size);
        Quaternion localRotation = Quaternion.Euler(0f, (Coordinate.x * 37 + Coordinate.y * 19) % 4 * 90f, 0f);
        CollisionProfileType profile = GetProfile(props[index].name);
        if (profile != CollisionProfileType.None && OverlapsPlayerSafetyZone(profile, localPosition, localRotation))
            return;

        GameObject visual = Instantiate(props[index], visuals);
        visual.name = props[index].name + "_Visual";
        visual.layer = DefaultLayer;
        visual.transform.localPosition = localPosition;
        visual.transform.localRotation = localRotation;
        visual.transform.localScale = Vector3.one;
        foreach (Transform child in visual.GetComponentsInChildren<Transform>(true))
            child.gameObject.layer = DefaultLayer;
        foreach (Collider importedCollider in visual.GetComponentsInChildren<Collider>(true))
            importedCollider.enabled = false;

        if (profile != CollisionProfileType.None)
            CreateCollisionProxy(props[index].name, profile, localPosition, localRotation);
    }

    private bool OverlapsPlayerSafetyZone(CollisionProfileType profile, Vector3 localPosition, Quaternion localRotation)
    {
        if (player == null)
            return false;

        CollisionShape[] shapes = GetShapes(profile);
        float clearance = IsLarge(profile) ? LargeBlockerClearance : NormalBlockerClearance;
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

    private void CreateCollisionProxy(string visualName, CollisionProfileType profile, Vector3 localPosition, Quaternion localRotation)
    {
        GameObject proxy = new GameObject(visualName + "_Collision");
        proxy.layer = DefaultLayer;
        proxy.transform.SetParent(collision, false);
        proxy.transform.localPosition = localPosition;
        proxy.transform.localRotation = localRotation;
        proxy.transform.localScale = Vector3.one;

        CollisionShape[] shapes = GetShapes(profile);
        for (int i = 0; i < shapes.Length; i++)
        {
            GameObject shape = new GameObject(shapes.Length == 1 ? "BodyCollider" : "BodyCollider_" + (i + 1));
            shape.layer = DefaultLayer;
            shape.transform.SetParent(proxy.transform, false);
            shape.transform.localPosition = shapes[i].Center;
            shape.transform.localRotation = Quaternion.identity;
            shape.transform.localScale = Vector3.one;
            BoxCollider box = shape.AddComponent<BoxCollider>();
            box.center = Vector3.zero;
            box.size = shapes[i].Size;
            box.enabled = true;
            box.isTrigger = false;
        }
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

    private static CollisionProfileType GetProfile(string prefabName)
    {
        string name = prefabName.ToLowerInvariant();
        if (name.Contains("barrier")) return CollisionProfileType.Barrier;
        if (name.Contains("crate") || name.Contains("box")) return CollisionProfileType.Box;
        if (name.Contains("barrel")) return CollisionProfileType.Barrel;
        if (name.Contains("tent")) return CollisionProfileType.Tent;
        if (name.Contains("generator")) return CollisionProfileType.Generator;
        if (name.Contains("radiostation")) return CollisionProfileType.RadioStation;
        if (name.Contains("tower")) return CollisionProfileType.Tower;
        if (name.Contains("hummer")) return CollisionProfileType.Hummer;
        return CollisionProfileType.None;
    }

    private static bool IsLarge(CollisionProfileType profile)
    {
        return profile == CollisionProfileType.Tent || profile == CollisionProfileType.RadioStation ||
               profile == CollisionProfileType.Tower || profile == CollisionProfileType.Hummer;
    }

    private static CollisionShape[] GetShapes(CollisionProfileType profile)
    {
        switch (profile)
        {
            case CollisionProfileType.Barrier: return new[] { new CollisionShape(new Vector3(0f, 0.7f, 0f), new Vector3(5.5f, 1.4f, 0.65f)) };
            case CollisionProfileType.Box: return new[] { new CollisionShape(new Vector3(0f, 0.65f, 0f), new Vector3(1.55f, 1.3f, 1.55f)) };
            case CollisionProfileType.Barrel: return new[] { new CollisionShape(new Vector3(0f, 0.6f, 0f), new Vector3(1.05f, 1.2f, 1.05f)) };
            case CollisionProfileType.Tent: return new[] { new CollisionShape(new Vector3(0f, 1.25f, 0f), new Vector3(8f, 2.5f, 5.5f)) };
            case CollisionProfileType.Generator: return new[] { new CollisionShape(new Vector3(0f, 0.8f, 0f), new Vector3(3.3f, 1.6f, 1.9f)) };
            case CollisionProfileType.RadioStation: return new[] { new CollisionShape(new Vector3(0f, 1.5f, 0f), new Vector3(4.9f, 3f, 3.9f)) };
            case CollisionProfileType.Tower: return new[] { new CollisionShape(new Vector3(0f, 1.25f, 0f), new Vector3(3.1f, 2.5f, 3.1f)) };
            case CollisionProfileType.Hummer: return new[]
            {
                new CollisionShape(new Vector3(0f, 0.85f, 0f), new Vector3(2.45f, 1.7f, 4f)),
                new CollisionShape(new Vector3(0f, 0.7f, 1.6f), new Vector3(2.45f, 1.4f, 1.4f))
            };
            default: return new CollisionShape[0];
        }
    }
}
