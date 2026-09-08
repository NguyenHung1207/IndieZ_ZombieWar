using UnityEngine;

public enum WeaponFireMode
{
    Automatic,
    SemiAutomatic
}

[CreateAssetMenu(menuName = "Project/Weapons/Weapon Definition")]
public sealed class WeaponDefinition : ScriptableObject
{
    [SerializeField] private string weaponId = "assault_rifle";
    [SerializeField] private string displayName = "Assault Rifle";
    [SerializeField, Min(0.01f)] private float fireRate = 8f;
    [SerializeField, Min(0.1f)] private float range = 30f;
    [SerializeField, Min(0f)] private float recoilDistance = 0.04f;
    [SerializeField, Min(0f)] private float recoilAngle = 2.5f;
    [SerializeField, Min(0f)] private float damage = 20f;
    [SerializeField] private WeaponFireMode fireMode = WeaponFireMode.Automatic;
    [SerializeField, Range(1, 8)] private int pelletCount = 1;
    [SerializeField, Range(0f, 30f)] private float spreadAngle;
    [SerializeField, Min(1)] private int magazineSize = 30;
    [SerializeField, Min(0.1f)] private float reloadDuration = 1.6f;

    public string WeaponId => weaponId;
    public string DisplayName => displayName;
    public float FireRate => fireRate;
    public float Range => range;
    public float RecoilDistance => recoilDistance;
    public float RecoilAngle => recoilAngle;
    public float Damage => damage;
    public WeaponFireMode FireMode => fireMode;
    public int PelletCount => pelletCount;
    public float SpreadAngle => spreadAngle;
    public int MagazineSize => magazineSize;
    public float ReloadDuration => reloadDuration;
}
