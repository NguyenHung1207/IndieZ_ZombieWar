using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public sealed class ZombieSpawnDirector : MonoBehaviour
{
    [SerializeField] private ZombieAI zombiePrefab;
    [SerializeField] private GameSession session;
    [SerializeField, Min(1)] private int maxActiveZombies = 24;
    [SerializeField, Min(0.1f)] private float startInterval = 2.8f;
    [SerializeField, Min(0.1f)] private float firstMinuteEndInterval = 2f;
    [SerializeField, Min(0.1f)] private float secondMinuteEndInterval = 1.3f;
    [SerializeField, Min(0.1f)] private float endInterval = 0.8f;
    [SerializeField, Min(0f)] private float initialDelay = 1f;
    [SerializeField, Min(1f)] private float minimumSpawnRadius = 20f;
    [SerializeField, Min(1f)] private float maximumSpawnRadius = 30f;
    [SerializeField, Min(1f)] private float zombieCleanupDistance = 80f;
    [Header("Level 2 Miniboss")]
    [SerializeField] private bool spawnGiantZombie;
    [SerializeField, Min(1f)] private float giantSpawnTime = 130f;
    [SerializeField, Min(1f)] private float giantHealth = 1500f;
    [SerializeField, Min(0.1f)] private float giantSpeed = 2.2f;
    [SerializeField, Min(0.1f)] private float giantDamage = 28f;

    private readonly List<ZombieAI> activeZombies = new List<ZombieAI>();
    private static readonly Collider[] SpawnOverlapResults = new Collider[16];
    private const int SpawnAttempts = 16;
    private const float SpawnFootprintRadius = 0.42f;
    private const float SpawnFootprintBaseHeight = 0.50f;
    private const float SpawnFootprintTopHeight = 1.30f;
    private Transform player;
    private float nextSpawnTime;
    private bool giantSpawned;

    private enum SpawnKind { Normal, Male, Fat, Giant }

    public int ActiveZombieCount => activeZombies.Count;

    private void Awake()
    {
        if (session == null)
            session = GameSession.Instance;

        // Recruitment levels intentionally have distinct pressure curves even if
        // a scene prefab is duplicated or its serialized values are stale.
        if (SceneManager.GetActiveScene().name == "Gameplay_Level02")
        {
            maxActiveZombies = 34;
            startInterval = 1.55f;
            firstMinuteEndInterval = 1.05f;
            secondMinuteEndInterval = 0.65f;
            endInterval = 0.5f;
            spawnGiantZombie = true;
            giantSpawnTime = 130f;
            giantHealth = 1500f;
            giantSpeed = 2.2f;
            giantDamage = 28f;
        }
        else
        {
            maxActiveZombies = 30;
            startInterval = 1.85f;
            firstMinuteEndInterval = 1.3f;
            secondMinuteEndInterval = 0.85f;
            endInterval = 0.65f;
        }
    }

    private void Start()
    {
        PlayerMovement playerMovement = FindFirstObjectByType<PlayerMovement>();
        player = playerMovement != null ? playerMovement.transform : null;
        EnemyVisualVariant.Preload("M24/Male/Prefabs/URP/ZombieMale_AAB_URP");
        EnemyVisualVariant.Preload("M24/Fat/Fat Zombie(Low Poly)/Prefab/FatZombie", EnemyVisualVariant.MaterialProfile.Fat);
        if (spawnGiantZombie)
            EnemyVisualVariant.Preload("M24/Mutant/prefab/SKM_Zombie_Mutant", EnemyVisualVariant.MaterialProfile.Mutant);
        nextSpawnTime = Time.time + initialDelay;
    }

    private void Update()
    {
        RemoveInactiveZombies();
        if (session == null || !session.IsPlaying || zombiePrefab == null || player == null)
            return;
        if (spawnGiantZombie && !giantSpawned && session.ElapsedTime >= giantSpawnTime && Time.time >= nextSpawnTime)
        {
            MakeRoomForGiant();
            SpawnZombie(SpawnKind.Giant);
            giantSpawned = true;
            nextSpawnTime = Time.time + SpawnInterval();
            return;
        }

        if (activeZombies.Count >= maxActiveZombies || Time.time < nextSpawnTime)
            return;

        SpawnZombie(ChooseSpawnKind());
        nextSpawnTime = Time.time + SpawnInterval();
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

    private float SpawnInterval()
    {
        float elapsed = session.ElapsedTime;
        if (elapsed < 60f)
            return Mathf.Lerp(startInterval, firstMinuteEndInterval, elapsed / 60f);
        if (elapsed < 120f)
            return Mathf.Lerp(firstMinuteEndInterval, secondMinuteEndInterval, (elapsed - 60f) / 60f);
        return Mathf.Lerp(secondMinuteEndInterval, endInterval, Mathf.Clamp01((elapsed - 120f) / 60f));
    }

    private void MakeRoomForGiant()
    {
        if (activeZombies.Count < maxActiveZombies)
            return;

        ZombieAI replacement = activeZombies[0];
        activeZombies.RemoveAt(0);
        if (replacement != null && !replacement.IsDead)
            Destroy(replacement.gameObject);
    }

    private SpawnKind ChooseSpawnKind()
    {
        float roll = Random.value;
        bool level2 = SceneManager.GetActiveScene().name == "Gameplay_Level02";
        if (roll < (level2 ? 0.12f : 0.06f)) return SpawnKind.Fat;
        if (roll < (level2 ? 0.25f : 0.14f)) return SpawnKind.Male;
        return SpawnKind.Normal;
    }

    private void SpawnZombie(SpawnKind kind)
    {
        for (int attempt = 0; attempt < SpawnAttempts; attempt++)
        {
            Vector2 ring = Random.insideUnitCircle.normalized * Random.Range(minimumSpawnRadius, maximumSpawnRadius);
            Vector3 candidate = player.position + new Vector3(ring.x, 0f, ring.y);
            if (!NavMesh.SamplePosition(candidate, out NavMeshHit hit, 4f, NavMesh.AllAreas))
                continue;
            if (!IsValidSpawnPosition(hit.position))
                continue;

            ZombieAI zombie = Instantiate(zombiePrefab, hit.position, Quaternion.LookRotation(player.position - hit.position, Vector3.up));
            zombie.SetTarget(player);
            if (kind == SpawnKind.Giant)
            {
                zombie.name = "Giant Zombie";
                zombie.GetComponent<ZombieHealth>().ConfigureMaxHealth(giantHealth);
                zombie.ConfigureCombatStats(giantSpeed, giantDamage);
                zombie.gameObject.AddComponent<EnemyVisualVariant>().Apply("M24/Mutant/prefab/SKM_Zombie_Mutant", 2.3f, true, EnemyVisualVariant.MaterialProfile.Mutant);
                AddGiantMarker(zombie.transform);
            }
            else if (kind == SpawnKind.Fat)
            {
                zombie.name = "Fat Zombie";
                zombie.GetComponent<ZombieHealth>().ConfigureMaxHealth(300f);
                zombie.ConfigureCombatStats(2.3f, 18f);
                zombie.gameObject.AddComponent<EnemyVisualVariant>().Apply("M24/Fat/Fat Zombie(Low Poly)/Prefab/FatZombie", 1.65f, true, EnemyVisualVariant.MaterialProfile.Fat);
            }
            else if (kind == SpawnKind.Male)
            {
                zombie.name = "Zombie Male Variant";
                zombie.GetComponent<ZombieHealth>().ConfigureMaxHealth(140f);
                zombie.gameObject.AddComponent<EnemyVisualVariant>().Apply("M24/Male/Prefabs/URP/ZombieMale_AAB_URP", 1.25f);
            }
            activeZombies.Add(zombie);
            return;
        }

        // A blocked ring is not an excuse to place an enemy in scenery. The
        // next scheduled spawn will retry with a new bounded set of candidates.
    }

    private bool IsValidSpawnPosition(Vector3 position)
    {
        if ((position - player.position).sqrMagnitude < minimumSpawnRadius * minimumSpawnRadius)
            return false;

        // An overlap at standing height rejects walls, boxes, tents, vehicles,
        // and explicit environment collision proxies, while leaving the NavMesh
        // support surface itself alone.
        Vector3 capsuleBottom = position + Vector3.up * SpawnFootprintBaseHeight;
        Vector3 capsuleTop = position + Vector3.up * SpawnFootprintTopHeight;
        int overlapCount = Physics.OverlapCapsuleNonAlloc(
            capsuleBottom, capsuleTop, SpawnFootprintRadius, SpawnOverlapResults, ~0, QueryTriggerInteraction.Ignore);
        for (int index = 0; index < overlapCount; index++)
        {
            Collider collider = SpawnOverlapResults[index];
            if (IsSolidEnvironmentCollider(collider))
                return false;
        }

        // NavMesh can be present over a prop top. Check the immediate support
        // below the sampled point as well, but permit authored floors, ramps,
        // and elevated Level 2 combat platforms.
        if (Physics.Raycast(position + Vector3.up * 1.6f, Vector3.down, out RaycastHit supportHit,
                2.1f, ~0, QueryTriggerInteraction.Ignore) && IsSolidEnvironmentCollider(supportHit.collider))
        {
            return false;
        }

        return true;
    }

    private static bool IsSolidEnvironmentCollider(Collider collider)
    {
        if (collider == null || !collider.enabled || collider.isTrigger)
            return false;
        if (collider.GetComponentInParent<ZombieAI>() != null || collider.GetComponentInParent<PlayerMovement>() != null)
            return false;

        string hierarchyName = string.Empty;
        for (Transform current = collider.transform; current != null; current = current.parent)
        {
            hierarchyName += current.name + " ";
            if (current.name.IndexOf("_Collision", System.StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
        }

        // These are deliberate traversable surfaces created by Level 2. Their
        // colliders are valid support for spawns on slopes and high ground.
        return hierarchyName.IndexOf("Ground", System.StringComparison.OrdinalIgnoreCase) < 0 &&
               hierarchyName.IndexOf("Floor", System.StringComparison.OrdinalIgnoreCase) < 0 &&
               hierarchyName.IndexOf("Terrain", System.StringComparison.OrdinalIgnoreCase) < 0 &&
               hierarchyName.IndexOf("Ramp", System.StringComparison.OrdinalIgnoreCase) < 0 &&
               hierarchyName.IndexOf("StartingZone", System.StringComparison.OrdinalIgnoreCase) < 0 &&
               hierarchyName.IndexOf("Stronghold", System.StringComparison.OrdinalIgnoreCase) < 0 &&
               hierarchyName.IndexOf("High_Ground", System.StringComparison.OrdinalIgnoreCase) < 0 &&
               hierarchyName.IndexOf("Giant_Arena", System.StringComparison.OrdinalIgnoreCase) < 0;
    }

    private static void AddGiantMarker(Transform giant)
    {
        GameObject marker = new GameObject("GiantZombieMarker", typeof(Canvas));
        marker.transform.SetParent(giant, false);
        marker.transform.localPosition = new Vector3(0f, 3.4f, 0f);
        Canvas canvas = marker.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
        RectTransform markerRect = marker.GetComponent<RectTransform>();
        markerRect.sizeDelta = new Vector2(3.8f, 0.65f);
        markerRect.localScale = Vector3.one * 0.012f;
        GameObject labelObject = new GameObject("Label", typeof(RectTransform), typeof(UnityEngine.UI.Text));
        labelObject.transform.SetParent(marker.transform, false);
        UnityEngine.UI.Text label = labelObject.GetComponent<UnityEngine.UI.Text>();
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.text = "GIANT ZOMBIE";
        label.fontSize = 42;
        label.fontStyle = FontStyle.Bold;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = new Color(1f, 0.2f, 0.12f);
        label.rectTransform.anchorMin = Vector2.zero;
        label.rectTransform.anchorMax = Vector2.one;
        label.rectTransform.offsetMin = label.rectTransform.offsetMax = Vector2.zero;
    }
}
