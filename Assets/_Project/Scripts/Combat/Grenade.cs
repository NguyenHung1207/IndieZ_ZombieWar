using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public sealed class Grenade : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float fuseDuration = 2f;
    [SerializeField, Min(0.1f)] private float explosionRadius = 5f;
    [SerializeField, Min(0f)] private float centerDamage = 90f;
    [SerializeField, Min(0f)] private float edgeDamage = 45f;
    [SerializeField, Min(0f)] private float knockbackForce = 8f;
    [SerializeField] private ParticleSystem explosionPrefab;
    [SerializeField] private GameObject visual;

    private Rigidbody body;
    private Collider grenadeCollider;
    private bool exploded;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        grenadeCollider = GetComponent<Collider>();
    }

    private void Start()
    {
        Invoke(nameof(Explode), fuseDuration);
    }

    public void Explode()
    {
        if (exploded)
            return;
        exploded = true;
        CancelInvoke(nameof(Explode));

        if (grenadeCollider != null)
            grenadeCollider.enabled = false;
        if (body != null)
        {
            body.isKinematic = true;
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }
        if (visual != null)
            visual.SetActive(false);

        ApplyExplosionDamage();
        CombatAudio.PlayAt(transform.position, CombatSound.GrenadeExplosion, 0.9f);
        if (explosionPrefab != null)
        {
            ParticleSystem effect = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constantMax + 0.1f);
        }

        Destroy(gameObject);
    }

    private void ApplyExplosionDamage()
    {
        HashSet<IDamageable> damagedTargets = new HashSet<IDamageable>();
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            explosionRadius,
            Physics.AllLayers,
            QueryTriggerInteraction.Ignore);

        foreach (Collider hit in hits)
        {
            IDamageable damageable = hit.GetComponentInParent<IDamageable>();
            if (damageable == null || !damagedTargets.Add(damageable))
                continue;

            Vector3 offset = hit.transform.position - transform.position;
            offset.y = 0f;
            float distance = Mathf.Min(offset.magnitude, explosionRadius);
            float falloff = 1f - distance / explosionRadius;
            float damage = Mathf.Lerp(edgeDamage, centerDamage, falloff);
            damageable.TakeDamage(damage);

            ZombieAI zombie = hit.GetComponentInParent<ZombieAI>();
            if (zombie != null)
            {
                Vector3 direction = offset.sqrMagnitude > 0.001f
                    ? offset.normalized
                    : transform.forward;
                zombie.ApplyExplosionKnockback(direction * (knockbackForce * falloff));
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.35f, 0.05f, 0.35f);
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
