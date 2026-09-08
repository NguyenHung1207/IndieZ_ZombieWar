using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public sealed class ZombieSpawnDirector : MonoBehaviour
{
    [SerializeField] private ZombieAI zombiePrefab;
    [SerializeField] private GameSession session;
    [SerializeField, Min(1)] private int maxActiveZombies = 24;
    [SerializeField, Min(0.1f)] private float startInterval = 2.8f;
    [SerializeField, Min(0.1f)] private float endInterval = 0.8f;
    [SerializeField, Min(0f)] private float initialDelay = 1f;
    [SerializeField, Min(1f)] private float minimumSpawnRadius = 20f;
    [SerializeField, Min(1f)] private float maximumSpawnRadius = 30f;
    [SerializeField, Min(1f)] private float zombieCleanupDistance = 80f;

    private readonly List<ZombieAI> activeZombies = new List<ZombieAI>();
    private Transform player;
    private float nextSpawnTime;

    public int ActiveZombieCount => activeZombies.Count;

    private void Awake()
    {
        if (session == null)
            session = GameSession.Instance;
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
        if (session == null || !session.IsPlaying || zombiePrefab == null || player == null)
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
            if (zombie == null || !zombie.isActiveAndEnabled || zombie.IsDead ||
                (player != null && (zombie.transform.position - player.position).sqrMagnitude > zombieCleanupDistance * zombieCleanupDistance))
            {
                if (zombie != null && zombie.isActiveAndEnabled && !zombie.IsDead)
                    Destroy(zombie.gameObject);
                activeZombies.RemoveAt(i);
            }
        }
    }

    private void SpawnZombie()
    {
        for (int attempt = 0; attempt < 16; attempt++)
        {
            Vector2 ring = Random.insideUnitCircle.normalized * Random.Range(minimumSpawnRadius, maximumSpawnRadius);
            Vector3 candidate = player.position + new Vector3(ring.x, 0f, ring.y);
            if (!NavMesh.SamplePosition(candidate, out NavMeshHit hit, 4f, NavMesh.AllAreas))
                continue;

            ZombieAI zombie = Instantiate(zombiePrefab, hit.position, Quaternion.LookRotation(player.position - hit.position, Vector3.up));
            zombie.SetTarget(player);
            activeZombies.Add(zombie);
            return;
        }
    }
}
