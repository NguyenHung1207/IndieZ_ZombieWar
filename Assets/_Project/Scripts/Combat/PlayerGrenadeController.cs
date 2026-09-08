using UnityEngine;

public sealed class PlayerGrenadeController : MonoBehaviour
{
    [SerializeField] private Grenade grenadePrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField, Min(0f)] private float throwForce = 8f;
    [SerializeField, Min(0f)] private float upwardForce = 3f;
    [SerializeField, Min(0.1f)] private float throwCooldown = 1f;

    private float nextThrowTime;

    private void Update()
    {
        if (GameSession.Instance != null && !GameSession.Instance.IsPlaying)
            return;
        if (Input.GetKeyDown(KeyCode.G))
            ThrowGrenade();
    }

    public void ThrowGrenade()
    {
        if (GameSession.Instance != null && !GameSession.Instance.IsPlaying)
            return;

        if (grenadePrefab == null || throwPoint == null || Time.time < nextThrowTime)
            return;

        nextThrowTime = Time.time + throwCooldown;
        Grenade grenade = Instantiate(grenadePrefab, throwPoint.position, throwPoint.rotation);
        Rigidbody body = grenade.GetComponent<Rigidbody>();
        if (body == null)
            return;

        foreach (Collider playerCollider in GetComponentsInChildren<Collider>())
        {
            Collider grenadeCollider = grenade.GetComponent<Collider>();
            if (grenadeCollider != null)
                Physics.IgnoreCollision(playerCollider, grenadeCollider);
        }

        body.linearVelocity = transform.forward * throwForce + Vector3.up * upwardForce;
    }
}
