using UnityEngine;

[RequireComponent(typeof(PlayerAnimationController))]
public sealed class PlayerWeaponController : MonoBehaviour
{
    [SerializeField] private WeaponDefinition weapon;
    [SerializeField] private Transform muzzle;
    [SerializeField] private WeaponRecoil recoil;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private WeaponTracer tracer;

    private PlayerAnimationController playerAnimationController;
    private bool fireHeld;
    private float nextFireTime;

    public WeaponDefinition EquippedWeapon => weapon;

    private void Awake()
    {
        playerAnimationController = GetComponent<PlayerAnimationController>();
    }

    private void Update()
    {
        SetFireHeld(Input.GetMouseButton(0));
        if (fireHeld)
        {
            TryFire();
        }
    }

    public void SetFireHeld(bool held)
    {
        fireHeld = held;
    }

    public bool TryFire()
    {
        if (weapon == null || muzzle == null || playerAnimationController == null || Time.time < nextFireTime)
        {
            return false;
        }

        nextFireTime = Time.time + 1f / weapon.FireRate;
        playerAnimationController.PlayShoot();

        if (recoil != null)
        {
            recoil.Configure(weapon.RecoilDistance, weapon.RecoilAngle);
        }

        if (muzzleFlash != null)
        {
            muzzleFlash.Play(true);
        }

        Vector3 direction = transform.forward;
        Vector3 endpoint = muzzle.position + direction * weapon.Range;
        if (Physics.Raycast(muzzle.position, direction, out RaycastHit hit, weapon.Range, ~0, QueryTriggerInteraction.Ignore))
        {
            endpoint = hit.point;
        }

        if (tracer != null)
        {
            tracer.Show(muzzle.position, endpoint);
        }

        return true;
    }
}
