using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

/// <summary>Fixed elevated combat space for Level 2; Level 1 retains the infinite flat world.</summary>
public sealed class Level2Environment : MonoBehaviour
{
    private const float ArenaSize = 112f;
    private Transform geometryRoot;
    private Transform artRoot;
    private Transform collisionRoot;
    private ProductionDressing militaryDressing;
    private readonly Dictionary<string, int> visualNameCounts = new Dictionary<string, int>();

    private void Awake()
    {
        InfiniteWorldController infiniteWorld = FindFirstObjectByType<InfiniteWorldController>();
        if (infiniteWorld != null)
            infiniteWorld.enabled = false;
        militaryDressing = FindFirstObjectByType<ProductionDressing>();

        GameObject oldGround = GameObject.Find("Ground");
        Material groundMaterial = oldGround != null ? oldGround.GetComponent<Renderer>()?.sharedMaterial : null;
        if (oldGround != null)
            oldGround.SetActive(false);
        GameObject oldObstacles = GameObject.Find("ArenaObstacles");
        if (oldObstacles != null)
            oldObstacles.SetActive(false);

        geometryRoot = CreateRoot("GameplayGeometry");
        artRoot = CreateRoot("WastelandArt");
        collisionRoot = CreateRoot("ExplicitCollisionProxies");

        // A generous flat deployment zone gives way to four independently
        // traversable uphill/downhill routes and elevated combat platforms.
        CreateBlock("Level2_FlatStartingZone", Vector3.zero, new Vector3(ArenaSize, 0.4f, ArenaSize), Quaternion.identity, groundMaterial);
        CreateRamp("Ramp_NorthWest", new Vector3(-24f, 1.65f, 20f), -9f, groundMaterial);
        CreateRamp("Ramp_NorthEast", new Vector3(24f, 1.65f, 20f), -9f, groundMaterial);
        CreateRamp("Ramp_SouthWest", new Vector3(-24f, 1.65f, -20f), 9f, groundMaterial);
        CreateRamp("Ramp_SouthEast", new Vector3(24f, 1.65f, -20f), 9f, groundMaterial);
        CreateBlock("Mid_Stronghold", new Vector3(0f, 0.25f, 8f), new Vector3(26f, 0.5f, 18f), Quaternion.identity, groundMaterial);
        CreateBlock("High_Ground", new Vector3(0f, 3.25f, 36f), new Vector3(56f, 0.5f, 18f), Quaternion.identity, groundMaterial);
        CreateBlock("Giant_Arena", new Vector3(0f, 3.25f, -36f), new Vector3(56f, 0.5f, 18f), Quaternion.identity, groundMaterial);


        BuildStartArea();
        BuildSlopeRoutes();
        BuildMidArea();
        BuildHighGround();
        BuildGiantArea();
    }

    private void Start()
    {
        NavMeshSurface surface = FindFirstObjectByType<NavMeshSurface>();
        if (surface != null)
        {
            surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            surface.BuildNavMesh();
        }
    }

    private void CreateRamp(string objectName, Vector3 position, float angle, Material material)
    {
        CreateBlock(objectName, position, new Vector3(18f, 0.5f, 22f), Quaternion.Euler(angle, 0f, 0f), material);
    }

    private void CreateBlock(string objectName, Vector3 position, Vector3 scale, Quaternion rotation, Material material)
    {
        GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
        block.name = objectName;
        block.transform.SetParent(geometryRoot, false);
        block.transform.position = position;
        block.transform.rotation = rotation;
        block.transform.localScale = scale;
        Renderer renderer = block.GetComponent<Renderer>();
        if (material != null)
            renderer.sharedMaterial = material;
    }

    private Transform CreateRoot(string name)
    {
        GameObject root = new GameObject(name);
        root.transform.SetParent(transform, false);
        return root.transform;
    }

    private void BuildStartArea()
    {
        Dress("Prefabs/Walls, Blocks/Wall_1A_Door", new Vector3(0f, 0f, -11f), 90f, 1.05f, true);
        Dress("Prefabs/Walls, Blocks/Wall_Broken_1B", new Vector3(-8f, 0f, -9f), 90f, 0.9f);
        Dress("Prefabs/Walls, Blocks/Wall_Broken_1C", new Vector3(8f, 0f, -9f), 90f, 0.9f);
        Dress("Prefabs/Props, Misc/Barrier_1B", new Vector3(-10f, 0f, -4f), 90f, 0.72f);
        Dress("Prefabs/Props, Misc/Barrier_1C", new Vector3(10f, 0f, -4f), 90f, 0.72f);
        Dress("Prefabs/Walls, Blocks/Concrete_Block_1B", new Vector3(-11f, 0f, 3f), 0f, 0.58f);
        Dress("Prefabs/Walls, Blocks/Concrete_Block_1C", new Vector3(11f, 0f, 3f), 0f, 0.58f);
        DressMilitary("Box_003", new Vector3(-8f, 0f, 5f), 0f, 1f);
        DressMilitary("Tires_001", new Vector3(8f, 0f, 5f), 0f, 1f);
    }

    private void BuildSlopeRoutes()
    {
        Dress("Prefabs/Fortified_Walls/Fortified_Wall_1B", new Vector3(-33f, 0f, 17f), 0f, 0.66f);
        Dress("Prefabs/Fortified_Walls/Fortified_Wall_1C", new Vector3(33f, 0f, 17f), 0f, 0.66f);
        Dress("Prefabs/Fortified_Walls/Fortified_Wall_1D", new Vector3(-33f, 0f, -18f), 0f, 0.66f);
        Dress("Prefabs/Fortified_Walls/Fortified_Wall_2A", new Vector3(33f, 0f, -18f), 0f, 0.66f);
        Dress("Prefabs/Walls, Blocks/Wall_Broken_1A", new Vector3(-14f, 0f, 20f), 0f, 0.68f);
        Dress("Prefabs/Walls, Blocks/Wall_Broken_1B", new Vector3(14f, 0f, 20f), 0f, 0.68f);
        Dress("Prefabs/Walls, Blocks/Wall_Broken_1C", new Vector3(-14f, 0f, -20f), 180f, 0.68f);
        Dress("Prefabs/Walls, Blocks/Wall_Broken_1A", new Vector3(14f, 0f, -20f), 180f, 0.68f);
        Dress("Prefabs/Props, Misc/Shanty_Wall_Board_1A", new Vector3(-32f, 0f, 27f), 0f, 0.78f);
        Dress("Prefabs/Props, Misc/Shanty_Wall_Board_1B", new Vector3(32f, 0f, -27f), 180f, 0.78f);
        DressMilitary("Barrier_004", new Vector3(-31f, 0f, 8f), 0f, 1f);
    }

    private void BuildMidArea()
    {
        Dress("Prefabs/Fortified_Walls/Fortified_Wall_2D", new Vector3(14f, 0f, 8f), 0f, 0.82f, true);
        Dress("Prefabs/Props, Misc/Awning_2A", new Vector3(10f, 0f, 13f), 0f, 0.7f);
        Dress("Prefabs/Walls, Blocks/Wall_1A_Window_1", new Vector3(-13f, 0f, 12f), 0f, 0.76f);
        Dress("Prefabs/Walls, Blocks/Wall_1A_Window_2", new Vector3(-13f, 0f, 1f), 0f, 0.76f);
        Dress("Prefabs/Props, Misc/Barrier_1A", new Vector3(-8f, 0f, 4f), 90f, 0.62f);
        Dress("Prefabs/Props, Misc/Barrier_1B", new Vector3(8f, 0f, 3f), 90f, 0.62f);
        Dress("Prefabs/Walls, Blocks/Concrete_Block_1D", new Vector3(-10f, 0f, 10f), 0f, 0.62f);
        Dress("Prefabs/Walls, Blocks/Concrete_Block_1E", new Vector3(10f, 0f, -1f), 0f, 0.62f);
        DressMilitary("Generator_004", new Vector3(9f, 0f, 11f), 90f, 1f);
        DressMilitary("Box_003", new Vector3(-9f, 0f, 12f), 0f, 1f);
    }

    private void BuildHighGround()
    {
        const float y = 3.55f;
        Dress("Prefabs/Fortified_Walls/Fortified_Wall_2B", new Vector3(0f, y, 45f), 90f, 0.92f, true);
        Dress("Prefabs/Fortified_Walls/Fortified_Wall_2C", new Vector3(-11f, y, 44f), 90f, 0.62f);
        Dress("Prefabs/Walls, Blocks/Wall_Broken_1C", new Vector3(12f, y, 38f), 0f, 0.68f);
        Dress("Prefabs/Props, Misc/Awning_2B", new Vector3(-11f, y, 38f), 0f, 0.64f);
        Dress("Prefabs/Props, Misc/Hanging_Tapestry_1A", new Vector3(0f, y + 1.1f, 44.5f), 90f, 0.76f);
        Dress("Prefabs/Walls, Blocks/Concrete_Block_1F", new Vector3(-11f, y, 30f), 0f, 0.5f);
        Dress("Prefabs/Walls, Blocks/Concrete_Block_1B", new Vector3(11f, y, 31f), 0f, 0.56f);
    }

    private void BuildGiantArea()
    {
        const float y = 3.55f;
        Dress("Prefabs/Fortified_Walls/Fortified_Wall_2D", new Vector3(0f, y, -47f), 90f, 1.05f, true);
        Dress("Prefabs/Fortified_Walls/Fortified_Wall_1D", new Vector3(-14f, y, -38f), 0f, 0.72f);
        Dress("Prefabs/Fortified_Walls/Fortified_Wall_2A", new Vector3(14f, y, -38f), 0f, 0.72f);
        Dress("Prefabs/Walls, Blocks/Wall_Broken_1B", new Vector3(-13f, y, -31f), 0f, 0.68f);
        Dress("Prefabs/Walls, Blocks/Wall_Broken_1C", new Vector3(13f, y, -31f), 0f, 0.68f);
        Dress("Prefabs/Props, Misc/Barrier_1C", new Vector3(-9f, y, -29f), 90f, 0.62f);
        Dress("Prefabs/Props, Misc/Barrier_1B", new Vector3(9f, y, -29f), 90f, 0.62f);
        Dress("Prefabs/Props, Misc/Hanging_Tapestry_1B", new Vector3(0f, y + 1.35f, -46.5f), 90f, 0.86f);
        Dress("Prefabs/Walls, Blocks/Concrete_Block_1C", new Vector3(-12f, y, -42f), 0f, 0.54f);
        Dress("Prefabs/Walls, Blocks/Concrete_Block_1E", new Vector3(12f, y, -42f), 0f, 0.54f);
        DressMilitary("Barrel_005", new Vector3(-10f, y, -34f), 0f, 1f);
        DressMilitary("Barrier_004", new Vector3(10f, y, -34f), 90f, 1f);
    }


    private void Dress(string relativePath, Vector3 position, float yaw, float scale, bool majorShadow = false)
    {
        GameObject prefab = Resources.Load<GameObject>("Wasteland/" + relativePath);
        if (prefab == null) return;
        GameObject prop = Instantiate(prefab, position, Quaternion.Euler(0f, yaw, 0f), artRoot);
        prop.name = NextVisualName(prefab.name);
        prop.transform.localScale *= scale;
        WastelandVisual.Prepare(prop, position.y, majorShadow);
        EnvironmentCollisionProfiles.Shape[] shapes = EnvironmentCollisionProfiles.Get(prefab.name);
        if (shapes != null) CreateCollisionProxy(prop, shapes);
    }

    private void DressMilitary(string prefabName, Vector3 position, float yaw, float scale)
    {
        GameObject prefab = militaryDressing != null ? militaryDressing.FindProp(prefabName) : null;
        if (prefab == null) return;
        GameObject prop = Instantiate(prefab, position, Quaternion.Euler(0f, yaw, 0f), artRoot);
        prop.name = NextVisualName(prefab.name);
        prop.transform.localScale *= scale;
        Collider[] importedColliders = prop.GetComponentsInChildren<Collider>(true);
        for (int index = 0; index < importedColliders.Length; index++) importedColliders[index].enabled = false;
        EnvironmentCollisionProfiles.Shape[] shapes = EnvironmentCollisionProfiles.Get(prefab.name);
        if (shapes != null) CreateCollisionProxy(prop, shapes);
    }

    private string NextVisualName(string prefabName)
    {
        visualNameCounts.TryGetValue(prefabName, out int count);
        visualNameCounts[prefabName] = count + 1;
        return count == 0 ? prefabName + "_Visual" : prefabName + "_" + (count + 1) + "_Visual";
    }

    private void CreateCollisionProxy(GameObject visual, EnvironmentCollisionProfiles.Shape[] shapes)
    {
        GameObject proxy = new GameObject(visual.name.Substring(0, visual.name.Length - "_Visual".Length) + "_Collision");
        proxy.transform.SetParent(collisionRoot, false);
        proxy.transform.localPosition = visual.transform.localPosition;
        proxy.transform.localRotation = visual.transform.localRotation;
        proxy.transform.localScale = visual.transform.localScale;
        for (int index = 0; index < shapes.Length; index++)
        {
            BoxCollider box;
            if (index == 0)
            {
                box = proxy.AddComponent<BoxCollider>();
            }
            else
            {
                GameObject shape = new GameObject("BodyCollider_" + (index + 1), typeof(BoxCollider));
                shape.transform.SetParent(proxy.transform, false);
                box = shape.GetComponent<BoxCollider>();
            }
            box.center = shapes[index].Center;
            box.size = shapes[index].Size;
            box.enabled = true;
            box.isTrigger = false;
        }
    }
}
