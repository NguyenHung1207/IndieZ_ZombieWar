using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

/// <summary>Fixed elevated combat space for Level 2; Level 1 retains the infinite flat world.</summary>
public sealed class Level2Environment : MonoBehaviour
{
    private const float ArenaSize = 112f;

    private void Awake()
    {
        InfiniteWorldController infiniteWorld = FindFirstObjectByType<InfiniteWorldController>();
        if (infiniteWorld != null)
            infiniteWorld.enabled = false;

        GameObject oldGround = GameObject.Find("Ground");
        Material groundMaterial = oldGround != null ? oldGround.GetComponent<Renderer>()?.sharedMaterial : null;
        if (oldGround != null)
            oldGround.SetActive(false);
        GameObject oldObstacles = GameObject.Find("ArenaObstacles");
        if (oldObstacles != null)
            oldObstacles.SetActive(false);

        // A generous flat deployment zone gives way to four independently
        // traversable uphill/downhill routes and elevated combat platforms.
        CreateBlock("Level2_FlatStartingZone", Vector3.zero, new Vector3(ArenaSize, 0.4f, ArenaSize), Quaternion.identity, groundMaterial);
        CreateRamp("Ramp_NorthWest", new Vector3(-24f, 1.65f, 20f), -9f, groundMaterial);
        CreateRamp("Ramp_NorthEast", new Vector3(24f, 1.65f, 20f), -9f, groundMaterial);
        CreateRamp("Ramp_SouthWest", new Vector3(-24f, 1.65f, -20f), 9f, groundMaterial);
        CreateRamp("Ramp_SouthEast", new Vector3(24f, 1.65f, -20f), 9f, groundMaterial);
        CreateBlock("ElevatedPlatform_North", new Vector3(0f, 3.25f, 36f), new Vector3(56f, 0.5f, 18f), Quaternion.identity, groundMaterial);
        CreateBlock("ElevatedPlatform_South", new Vector3(0f, 3.25f, -36f), new Vector3(56f, 0.5f, 18f), Quaternion.identity, groundMaterial);

        CreateBlock("SlopeRouteObstacle_NW", new Vector3(-35f, 1.1f, 13f), new Vector3(5f, 2.2f, 7f), Quaternion.Euler(0f, 24f, 0f), groundMaterial);
        CreateBlock("SlopeRouteObstacle_NE", new Vector3(35f, 1.1f, 13f), new Vector3(5f, 2.2f, 7f), Quaternion.Euler(0f, -24f, 0f), groundMaterial);
        CreateBlock("SlopeRouteObstacle_SW", new Vector3(-35f, 1.1f, -13f), new Vector3(5f, 2.2f, 7f), Quaternion.Euler(0f, -24f, 0f), groundMaterial);
        CreateBlock("SlopeRouteObstacle_SE", new Vector3(35f, 1.1f, -13f), new Vector3(5f, 2.2f, 7f), Quaternion.Euler(0f, 24f, 0f), groundMaterial);
        CreateBlock("ElevatedCover_North", new Vector3(0f, 4.25f, 38f), new Vector3(10f, 2f, 3f), Quaternion.identity, groundMaterial);
        CreateBlock("ElevatedCover_South", new Vector3(0f, 4.25f, -38f), new Vector3(10f, 2f, 3f), Quaternion.identity, groundMaterial);

        // Wasteland LITE art is decoration only: the authored cube geometry
        // above remains the sole collision/NavMesh authority.
        Dress("Prefabs/Fortified_Walls/Fortified_Wall_1A", new Vector3(-45f, 0f, 10f), 90f, 1.1f);
        Dress("Prefabs/Walls, Blocks/Wall_Broken_1A", new Vector3(45f, 0f, 10f), -90f, 1.1f);
        Dress("Prefabs/Walls, Blocks/Concrete_Block_1A", new Vector3(-43f, 0f, -22f), 25f, 1.35f);
        Dress("Prefabs/Props, Misc/Barrier_1A", new Vector3(42f, 0f, -23f), -20f, 1.15f);
        Dress("Prefabs/Fortified_Walls/Fortified_Wall_1A", new Vector3(-18f, 3.55f, 39f), 0f, 0.9f);
        Dress("Prefabs/Walls, Blocks/Wall_Broken_1A", new Vector3(18f, 3.55f, -39f), 180f, 0.9f);
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
        block.transform.SetParent(transform, false);
        block.transform.position = position;
        block.transform.rotation = rotation;
        block.transform.localScale = scale;
        Renderer renderer = block.GetComponent<Renderer>();
        if (material != null)
            renderer.sharedMaterial = material;
    }

    private void Dress(string relativePath, Vector3 position, float yaw, float scale)
    {
        GameObject prefab = Resources.Load<GameObject>("Wasteland/" + relativePath);
        if (prefab == null) return;
        GameObject prop = Instantiate(prefab, position, Quaternion.Euler(0f, yaw, 0f), transform);
        prop.name = "Wasteland_" + prefab.name;
        prop.transform.localScale *= scale;
        WastelandVisual.Prepare(prop);
    }
}
