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
    public WeaponTracer tracer;
}

[RequireComponent(typeof(PlayerAnimationController))]
public sealed class PlayerWeaponController : MonoBehaviour
{
    [SerializeField] private WeaponSlot[] weapons;
    [SerializeField, Min(0)] private int startingWeaponIndex;
    [SerializeField] private CombatAudio combatAudio;

    private static readonly Vector3[] PelletEndpoints = new Vector3[8];
    private PlayerAnimationController playerAnimationController;
    private bool fireHeld;
    private bool desktopFireHeld;
    private bool mobileFireHeld;
    private bool semiAutomaticShotConsumed;
    private float nextFireTime;
    private int currentWeaponIndex;

    public int CurrentWeaponIndex => currentWeaponIndex;
    public int WeaponCount => weapons != null ? weapons.Length : 0;
    public WeaponDefinition EquippedWeapon => GetCurrentSlot()?.definition;
    public event Action<WeaponDefinition> WeaponChanged;

    private void Awake()
    {
        playerAnimationController = GetComponent<PlayerAnimationController>();
        if (combatAudio == null)
            combatAudio = GetComponent<CombatAudio>();
        currentWeaponIndex = Mathf.Clamp(startingWeaponIndex, 0, Mathf.Max(0, WeaponCount - 1));
        ApplyActiveWeapon();
        WeaponChanged?.Invoke(EquippedWeapon);
    }

    private void Update()
    {
        if (GameSession.Instance != null && !GameSession.Instance.IsPlaying)
            return;
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwitchWeapon();
            return;
        }

        SetDesktopFireHeld(Input.GetMouseButton(0));
        WeaponDefinition definition = EquippedWeapon;
        if (!fireHeld || definition == null)
            return;

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

        nextFireTime = Time.time + 1f / definition.FireRate;
        playerAnimationController.PlayShoot();

        slot.recoil?.Configure(definition.RecoilDistance, definition.RecoilAngle);
        slot.muzzleFlash?.Play(true);
        combatAudio?.Play(definition.PelletCount > 1 ? CombatSound.ShotgunShot : CombatSound.RifleShot, 0.65f);

        int pelletCount = Mathf.Clamp(definition.PelletCount, 1, PelletEndpoints.Length);
        Vector3 forward = transform.forward;
        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 direction = GetPelletDirection(forward, definition.SpreadAngle);
            Vector3 endpoint = slot.muzzle.position + direction * definition.Range;
            if (Physics.Raycast(slot.muzzle.position, direction, out RaycastHit hit, definition.Range, ~0, QueryTriggerInteraction.Ignore))
            {
                endpoint = hit.point;
                IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
                damageable?.TakeDamage(definition.Damage);
            }
            PelletEndpoints[i] = endpoint;
        }

        slot.tracer?.ShowPellets(slot.muzzle.position, PelletEndpoints, pelletCount);

        return true;
    }

    private WeaponSlot GetCurrentSlot()
    {
        return weapons != null && currentWeaponIndex >= 0 && currentWeaponIndex < weapons.Length
            ? weapons[currentWeaponIndex]
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
                weapons[i].weaponObject.SetActive(i == currentWeaponIndex);
        }
    }

    private static Vector3 GetPelletDirection(Vector3 forward, float spreadAngle)
    {
        if (spreadAngle <= 0f)
            return forward;

        Vector2 offset = UnityEngine.Random.insideUnitCircle * Mathf.Tan(spreadAngle * Mathf.Deg2Rad);
        return (forward + Vector3.right * offset.x + Vector3.up * offset.y).normalized;
    }
}
