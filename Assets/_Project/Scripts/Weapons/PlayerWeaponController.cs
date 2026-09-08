using System;
using UnityEngine;

[Serializable]
public sealed class WeaponSlot
{
    public WeaponDefinition definition;
    public GameObject weaponObject;
    public Transform muzzle;
    public WeaponRecoil recoil;
    public ParticleSystem muzzleFlash;
    public GameObject muzzleEffectPrefab;
    public WeaponTracer tracer;
    [NonSerialized] public int currentAmmo;
    [NonSerialized] public bool isReloading;
}

[RequireComponent(typeof(PlayerAnimationController))]
public sealed class PlayerWeaponController : MonoBehaviour
{
    [SerializeField] private WeaponSlot[] weapons;
    [SerializeField] private int[] loadoutSlotIndices = { 0, 1 };
    [SerializeField, Min(0)] private int startingWeaponIndex;
    [SerializeField] private CombatAudio combatAudio;
    [SerializeField] private GameObject zombieBulletImpactPrefab;

    private static readonly Vector3[] PelletEndpoints = new Vector3[8];
    private PlayerAnimationController playerAnimationController;
    private PlayerAutoAim autoAim;
    private bool fireHeld;
    private bool desktopFireHeld;
    private bool mobileFireHeld;
    private bool semiAutomaticShotConsumed;
    private float nextFireTime;
    private int currentWeaponIndex;
    private Coroutine reloadCoroutine;

    public int CurrentWeaponIndex => currentWeaponIndex;
    public int WeaponCount => loadoutSlotIndices != null ? loadoutSlotIndices.Length : 0;
    public WeaponDefinition EquippedWeapon => GetCurrentSlot()?.definition;
    public event Action<WeaponDefinition> WeaponChanged;
    public event Action<WeaponDefinition, int, int, bool> AmmoChanged;
    public int CurrentAmmo => GetCurrentSlot()?.currentAmmo ?? 0;
    public int CurrentMagazineSize => EquippedWeapon != null ? EquippedWeapon.MagazineSize : 0;
    public bool IsReloading => GetCurrentSlot()?.isReloading ?? false;

    private void Awake()
    {
        playerAnimationController = GetComponent<PlayerAnimationController>();
        autoAim = GetComponent<PlayerAutoAim>();
        if (combatAudio == null)
            combatAudio = GetComponent<CombatAudio>();
        ResolveSavedLoadout();
        currentWeaponIndex = Mathf.Clamp(startingWeaponIndex, 0, Mathf.Max(0, WeaponCount - 1));
        InitializeAmmo();
        ApplyActiveWeapon();
        WeaponChanged?.Invoke(EquippedWeapon);
        NotifyAmmoChanged();
    }

    private void Update()
    {
        if (PauseController.IsPaused)
        {
            StopInput();
            return;
        }
        if (GameSession.Instance != null && !GameSession.Instance.IsPlaying)
        {
            autoAim?.ClearTarget();
            return;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwitchWeapon();
            return;
        }
        if (Input.GetKeyDown(KeyCode.R))
            Reload();

        SetDesktopFireHeld(Input.GetMouseButton(0));
        WeaponDefinition definition = EquippedWeapon;
        if (!fireHeld || definition == null)
        {
            if (!fireHeld)
                autoAim?.ClearTarget();
            return;
        }

        WeaponSlot currentSlot = GetCurrentSlot();
        autoAim?.SetAimOrigin(currentSlot?.muzzle);
        autoAim?.RefreshTarget(definition.Range);
        autoAim?.RotateTowardTarget(true);

        if (definition.FireMode == WeaponFireMode.Automatic || !semiAutomaticShotConsumed)
        {
            if (TryFire() && definition.FireMode == WeaponFireMode.SemiAutomatic)
                semiAutomaticShotConsumed = true;
        }
    }

    public void SetFireHeld(bool held)
    {
        mobileFireHeld = held;
        UpdateFireHeldState();
    }

    public void StopInput()
    {
        desktopFireHeld = false;
        mobileFireHeld = false;
        UpdateFireHeldState();
        autoAim?.ClearTarget();
        CancelReload();
    }

    public void SwitchWeapon()
    {
        if (WeaponCount < 2)
            return;
        EquipWeapon((currentWeaponIndex + 1) % WeaponCount);
    }

    public void EquipWeapon(int index)
    {
        if (weapons == null || index < 0 || index >= weapons.Length || index == currentWeaponIndex)
            return;

        StopInput();
        currentWeaponIndex = index;
        nextFireTime = Time.time;
        ApplyActiveWeapon();
        WeaponChanged?.Invoke(EquippedWeapon);
        NotifyAmmoChanged();
    }

    public void Reload()
    {
        if (GameSession.Instance != null && !GameSession.Instance.IsPlaying)
            return;

        WeaponSlot slot = GetCurrentSlot();
        WeaponDefinition definition = slot?.definition;
        if (slot == null || definition == null || slot.isReloading || slot.currentAmmo >= definition.MagazineSize)
            return;

        reloadCoroutine = StartCoroutine(ReloadRoutine(slot, definition));
        combatAudio?.PlayReload(definition.PelletCount > 1);
    }

    public bool TryFire()
    {
        if (GameSession.Instance != null && !GameSession.Instance.IsPlaying)
            return false;

        WeaponSlot slot = GetCurrentSlot();
        WeaponDefinition definition = slot?.definition;
        if (slot == null || definition == null || slot.muzzle == null || playerAnimationController == null
            || Time.time < nextFireTime)
        {
            return false;
        }

        if (slot.isReloading)
            return false;
        if (slot.currentAmmo <= 0)
        {
            Reload();
            return false;
        }

        nextFireTime = Time.time + 1f / definition.FireRate;
        slot.currentAmmo--;
        NotifyAmmoChanged();
        playerAnimationController.PlayShoot();

        slot.recoil?.Configure(definition.RecoilDistance, definition.RecoilAngle);
        if (slot.muzzleEffectPrefab != null)
            SpawnMuzzleEffect(slot.muzzleEffectPrefab, slot.muzzle);
        else
            slot.muzzleFlash?.Play(true);
        combatAudio?.PlayWeapon(definition, 0.65f);

        int pelletCount = Mathf.Clamp(definition.PelletCount, 1, PelletEndpoints.Length);
        Vector3 forward = transform.forward;
        autoAim?.SetAimOrigin(slot.muzzle);
        if (autoAim != null)
        {
            autoAim.RefreshTarget(definition.Range);
            autoAim.SnapTowardTarget();
            if (autoAim.TryGetAimDirection(slot.muzzle, out Vector3 assistedDirection))
                forward = assistedDirection;
        }
        Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;
        Vector3 up = Vector3.Cross(forward, right).normalized;
        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 direction = GetPelletDirection(forward, right, up, definition.SpreadAngle);
            Vector3 endpoint = slot.muzzle.position + direction * definition.Range;
            if (Physics.Raycast(slot.muzzle.position, direction, out RaycastHit hit, definition.Range, ~0, QueryTriggerInteraction.Ignore))
            {
                endpoint = hit.point;
                IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
                damageable?.TakeDamage(definition.Damage);
                if (hit.collider.GetComponentInParent<ZombieHealth>() != null)
                    SpawnZombieImpact(hit.point, hit.normal);
            }
            PelletEndpoints[i] = endpoint;
        }

        slot.tracer?.ShowPellets(slot.muzzle.position, PelletEndpoints, pelletCount);

        return true;
    }

    private WeaponSlot GetCurrentSlot()
    {
        int slotIndex = GetLoadoutSlotIndex(currentWeaponIndex);
        return weapons != null && slotIndex >= 0 && slotIndex < weapons.Length
            ? weapons[slotIndex]
            : null;
    }

    private void SetDesktopFireHeld(bool held)
    {
        desktopFireHeld = held;
        UpdateFireHeldState();
    }

    private void UpdateFireHeldState()
    {
        bool nextFireHeld = desktopFireHeld || mobileFireHeld;
        if (fireHeld && !nextFireHeld)
            semiAutomaticShotConsumed = false;
        fireHeld = nextFireHeld;
    }

    private void ApplyActiveWeapon()
    {
        if (weapons == null)
            return;
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i]?.weaponObject != null)
                weapons[i].weaponObject.SetActive(i == GetLoadoutSlotIndex(currentWeaponIndex));
        }
    }

    private int GetLoadoutSlotIndex(int index) => loadoutSlotIndices != null && index >= 0 && index < loadoutSlotIndices.Length ? loadoutSlotIndices[index] : index;

    private void ResolveSavedLoadout()
    {
        if (weapons == null || weapons.Length == 0) return;
        if (loadoutSlotIndices == null || loadoutSlotIndices.Length != 2) loadoutSlotIndices = new[] { 0, Mathf.Min(1, weapons.Length - 1) };
        int first = FindOwnedWeapon(WeaponOwnership.GetSlot(0));
        int second = FindOwnedWeapon(WeaponOwnership.GetSlot(1));
        if (first < 0) first = 0;
        if (second < 0 || second == first) second = weapons.Length > 1 && first != 1 ? 1 : 0;
        loadoutSlotIndices[0] = first; loadoutSlotIndices[1] = second;
    }

    private int FindOwnedWeapon(string id)
    {
        for (int i = 0; i < weapons.Length; i++)
            if (weapons[i]?.definition != null && weapons[i].definition.WeaponId == id && WeaponOwnership.IsOwned(id)) return i;
        return -1;
    }

    private void InitializeAmmo()
    {
        if (weapons == null)
            return;
        foreach (WeaponSlot slot in weapons)
        {
            if (slot?.definition == null)
                continue;
            slot.currentAmmo = slot.definition.MagazineSize;
            slot.isReloading = false;
        }
    }

    private System.Collections.IEnumerator ReloadRoutine(WeaponSlot slot, WeaponDefinition definition)
    {
        slot.isReloading = true;
        NotifyAmmoChanged();
        yield return new WaitForSeconds(definition.ReloadDuration);
        if (slot != null && slot.definition == definition)
        {
            slot.currentAmmo = definition.MagazineSize;
            slot.isReloading = false;
            if (slot == GetCurrentSlot())
                NotifyAmmoChanged();
        }
        reloadCoroutine = null;
    }

    private void CancelReload()
    {
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
        }
        WeaponSlot slot = GetCurrentSlot();
        if (slot != null && slot.isReloading)
        {
            slot.isReloading = false;
            NotifyAmmoChanged();
        }
    }

    private void NotifyAmmoChanged()
    {
        WeaponSlot slot = GetCurrentSlot();
        AmmoChanged?.Invoke(slot?.definition, slot?.currentAmmo ?? 0, slot?.definition?.MagazineSize ?? 0,
            slot?.isReloading ?? false);
    }

    private void SpawnZombieImpact(Vector3 point, Vector3 normal)
    {
        if (zombieBulletImpactPrefab == null)
            return;
        Instantiate(zombieBulletImpactPrefab, point + normal * 0.01f, Quaternion.LookRotation(normal));
    }

    private static void SpawnMuzzleEffect(GameObject prefab, Transform muzzle)
    {
        GameObject effect = Instantiate(prefab, muzzle.position, muzzle.rotation);
        effect.transform.SetParent(muzzle, true);
        ParticleSystem[] particles = effect.GetComponentsInChildren<ParticleSystem>(true);
        float lifetime = 0.5f;
        for (int i = 0; i < particles.Length; i++)
        {
            ParticleSystem.MainModule main = particles[i].main;
            main.loop = false;
            particles[i].Play(true);
            lifetime = Mathf.Max(lifetime, main.duration + main.startLifetime.constantMax + 0.1f);
        }
        Destroy(effect, Mathf.Clamp(lifetime, 0.1f, 2f));
    }

    private void OnDisable()
    {
        CancelReload();
    }

    private static Vector3 GetPelletDirection(Vector3 forward, Vector3 right, Vector3 up, float spreadAngle)
    {
        if (spreadAngle <= 0f)
            return forward;

        Vector2 offset = UnityEngine.Random.insideUnitCircle * Mathf.Tan(spreadAngle * Mathf.Deg2Rad);
        return (forward + right * offset.x + up * offset.y).normalized;
    }
}
