using UnityEngine;

public sealed class WeaponRecoil : MonoBehaviour
{
    [SerializeField, Min(0.01f)] private float recoverySpeed = 18f;

    private Vector3 defaultLocalPosition;
    private Quaternion defaultLocalRotation;
    private Vector3 recoilOffset;
    private float recoilAngle;

    private void Awake()
    {
        defaultLocalPosition = transform.localPosition;
        defaultLocalRotation = transform.localRotation;
    }

    public void Configure(float distance, float angle)
    {
        recoilOffset += Vector3.back * distance;
        recoilAngle = Mathf.Clamp(recoilAngle + angle, -12f, 12f);
    }

    private void LateUpdate()
    {
        recoilOffset = Vector3.Lerp(recoilOffset, Vector3.zero, 1f - Mathf.Exp(-recoverySpeed * Time.deltaTime));
        recoilAngle = Mathf.Lerp(recoilAngle, 0f, 1f - Mathf.Exp(-recoverySpeed * Time.deltaTime));
        transform.localPosition = defaultLocalPosition + recoilOffset;
        transform.localRotation = defaultLocalRotation * Quaternion.Euler(recoilAngle, 0f, 0f);
    }
}
