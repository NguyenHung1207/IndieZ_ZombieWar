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
        HashSet<Vector2Int> required = new HashSet<Vector2Int>();
        for (int x = -activeRadius; x <= activeRadius; x++)
            for (int y = -activeRadius; y <= activeRadius; y++) required.Add(center + new Vector2Int(x, y));
        List<Vector2Int> stale = new List<Vector2Int>();
        foreach (var pair in chunks) if (!required.Contains(pair.Key)) stale.Add(pair.Key);
        List<WorldChunk> recycled = new List<WorldChunk>();
        for (int i = 0; i < stale.Count; i++) { recycled.Add(chunks[stale[i]]); chunks.Remove(stale[i]); }
        foreach (Vector2Int coordinate in required)
        {
            if (!chunks.TryGetValue(coordinate, out WorldChunk chunk))
            {
                if (recycled.Count > 0)
                {
                    chunk = recycled[recycled.Count - 1];
                    recycled.RemoveAt(recycled.Count - 1);
                }
                else
                {
                    GameObject go = new GameObject("WorldChunk_" + coordinate.x + "_" + coordinate.y);
                    go.transform.SetParent(transform, false); chunk = go.AddComponent<WorldChunk>();
                }
                chunk.name = "WorldChunk_" + coordinate.x + "_" + coordinate.y;
                chunks.Add(coordinate, chunk);

                int layout = Mathf.Abs(coordinate.x * 31 + coordinate.y * 17) % 6;
                chunk.Configure(coordinate, chunkSize, groundMaterial, militaryProps, layout);
            }
        }
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
