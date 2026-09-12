using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public sealed class InfiniteWorldController : MonoBehaviour
{
    [SerializeField, Min(10f)] private float chunkSize = 40f;
    [SerializeField, Min(1)] private int activeRadius = 1;
    [SerializeField] private Material groundMaterial;
    [SerializeField] private GameObject[] militaryProps;
    [SerializeField] private NavMeshSurface navigationSurface;
    private readonly Dictionary<Vector2Int, WorldChunk> chunks = new Dictionary<Vector2Int, WorldChunk>();
    private readonly HashSet<Vector2Int> requiredCoordinates = new HashSet<Vector2Int>();
    private readonly List<Vector2Int> staleCoordinates = new List<Vector2Int>();
    private readonly List<WorldChunk> recycledChunks = new List<WorldChunk>();
    private Vector2Int playerCoordinate;
    private Transform player;
    private AsyncOperation navMeshUpdate;
    private bool navMeshUpdateQueued;
    public int ActiveChunkCount => chunks.Count;
    public float ChunkSize => chunkSize;

    private void Start()
    {
        PlayerMovement movement = FindFirstObjectByType<PlayerMovement>();
        player = movement != null ? movement.transform : null;
        if (groundMaterial == null)
        {
            GameObject oldGround = GameObject.Find("Ground");
            Renderer renderer = oldGround != null ? oldGround.GetComponent<Renderer>() : null;
            groundMaterial = renderer != null ? renderer.sharedMaterial : null;
            if (oldGround != null) oldGround.SetActive(false);
        }
        GameObject oldBounds = GameObject.Find("ArenaBounds"); if (oldBounds != null) oldBounds.SetActive(false);
        GameObject oldCameraBounds = GameObject.Find("CameraBounds"); if (oldCameraBounds != null) oldCameraBounds.SetActive(false);
        GameObject oldObstacles = GameObject.Find("ArenaObstacles"); if (oldObstacles != null) oldObstacles.SetActive(false);
        GameObject cameraRig = GameObject.Find("Cinemachine Camera");
        if (cameraRig != null)
            foreach (MonoBehaviour component in cameraRig.GetComponents<MonoBehaviour>())
                if (component != null && component.GetType().Name.Contains("Confiner3D")) component.enabled = false;
        if (navigationSurface == null) navigationSurface = FindFirstObjectByType<NavMeshSurface>();
        BuildGrid(ChunkCoordinate());
        BuildInitialNavigation();
    }

    private void Update()
    {
        if (player == null) return;
        Vector2Int coordinate = ChunkCoordinate();
        if (coordinate != playerCoordinate)
        {
            BuildGrid(coordinate);
            RequestNavigationUpdate();
        }
        if (navMeshUpdate != null && navMeshUpdate.isDone)
        {
            navMeshUpdate = null;
            if (navMeshUpdateQueued)
            {
                navMeshUpdateQueued = false;
                RequestNavigationUpdate();
            }
        }
    }

    private Vector2Int ChunkCoordinate()
    {
        if (player == null) return Vector2Int.zero;
        return new Vector2Int(Mathf.FloorToInt(player.position.x / chunkSize), Mathf.FloorToInt(player.position.z / chunkSize));
    }

    private void BuildGrid(Vector2Int center)
    {
        playerCoordinate = center;
        requiredCoordinates.Clear();
        for (int x = -activeRadius; x <= activeRadius; x++)
            for (int y = -activeRadius; y <= activeRadius; y++) requiredCoordinates.Add(center + new Vector2Int(x, y));
        staleCoordinates.Clear();
        recycledChunks.Clear();
        foreach (var pair in chunks) if (!requiredCoordinates.Contains(pair.Key)) staleCoordinates.Add(pair.Key);
        for (int i = 0; i < staleCoordinates.Count; i++) { recycledChunks.Add(chunks[staleCoordinates[i]]); chunks.Remove(staleCoordinates[i]); }
        foreach (Vector2Int coordinate in requiredCoordinates)
        {
            if (!chunks.TryGetValue(coordinate, out WorldChunk chunk))
            {
                if (recycledChunks.Count > 0)
                {
                    chunk = recycledChunks[recycledChunks.Count - 1];
                    recycledChunks.RemoveAt(recycledChunks.Count - 1);
                }
                else
                {
                    GameObject go = new GameObject("WorldChunk_" + coordinate.x + "_" + coordinate.y);
                    go.transform.SetParent(transform, false); chunk = go.AddComponent<WorldChunk>();
                }
                chunks.Add(coordinate, chunk);

                int layout = LayoutForCoordinate(coordinate);
                chunk.Configure(coordinate, chunkSize, groundMaterial, militaryProps, layout);
            }
        }
    }

    private static int LayoutForCoordinate(Vector2Int coordinate)
    {
        int value = (coordinate.x * 31 + coordinate.y * 17) % 6;
        return value < 0 ? value + 6 : value;
    }

    private void BuildInitialNavigation()
    {
        if (navigationSurface == null) return;
        navigationSurface.collectObjects = CollectObjects.All;
        navigationSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
        navigationSurface.BuildNavMesh();
    }

    private void RequestNavigationUpdate()
    {
        if (navigationSurface == null || navigationSurface.navMeshData == null)
            return;
        if (navMeshUpdate != null && !navMeshUpdate.isDone)
        {
            navMeshUpdateQueued = true;
            return;
        }
        navigationSurface.collectObjects = CollectObjects.All;
        navigationSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
        navMeshUpdate = navigationSurface.UpdateNavMesh(navigationSurface.navMeshData);
    }
}
