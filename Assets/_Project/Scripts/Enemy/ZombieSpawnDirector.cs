using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public sealed class ZombieSpawnDirector : MonoBehaviour
{
    [SerializeField] private ZombieAI zombiePrefab;
    [SerializeField] private GameSession session;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField, Min(1)] private int maxActiveZombies = 24;
    [SerializeField, Min(0.1f)] private float startInterval = 2.8f;
    [SerializeField, Min(0.1f)] private float endInterval = 0.8f;
    [SerializeField, Min(0f)] private float initialDelay = 1f;
    [SerializeField, Min(0f)] private float minimumPlayerDistance = 8f;

    private readonly List<ZombieAI> activeZombies = new List<ZombieAI>();
    private Transform player;
    private int nextSpawnPoint;
    private float nextSpawnTime;

    public int ActiveZombieCount => activeZombies.Count;

    private void Awake()
    {
        if (session == null)
            session = GameSession.Instance;
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            List<Transform> points = new List<Transform>();
            foreach (Transform child in transform)
                points.Add(child);
            spawnPoints = points.ToArray();
        }
    }

    private void Start()
    {
        PlayerMovement playerMovement = FindFirstObjectByType<PlayerMovement>();
        player = playerMovement != null ? playerMovement.transform : null;
        nextSpawnTime = Time.time + initialDelay;
    }

    private void Update()
    {
        RemoveInactiveZombies();
        if (session == null || !session.IsPlaying || zombiePrefab == null || spawnPoints.Length == 0)
            return;
        if (activeZombies.Count >= maxActiveZombies || Time.time < nextSpawnTime)
            return;

        SpawnZombie();
        nextSpawnTime = Time.time + Mathf.Lerp(startInterval, endInterval, session.Progress01);
    }

    private void RemoveInactiveZombies()
    {
        for (int i = activeZombies.Count - 1; i >= 0; i--)
        {
            ZombieAI zombie = activeZombies[i];
            if (zombie == null || !zombie.isActiveAndEnabled || zombie.IsDead)
                activeZombies.RemoveAt(i);
        }
    }

    private void SpawnZombie()
    {
        for (int attempt = 0; attempt < spawnPoints.Length; attempt++)
        {
            Transform point = spawnPoints[nextSpawnPoint++ % spawnPoints.Length];
            if (player != null && Vector3.Distance(point.position, player.position) < minimumPlayerDistance)
                continue;
            if (!NavMesh.SamplePosition(point.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                continue;

            ZombieAI zombie = Instantiate(zombiePrefab, hit.position, point.rotation);
            zombie.SetTarget(player);
            activeZombies.Add(zombie);
            return;
        }
    }
}
