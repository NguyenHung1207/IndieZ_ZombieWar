using UnityEngine;

public sealed class PlayerAutoAim : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float aimRange = 14f;
    [SerializeField, Range(1f, 180f)] private float coneAngle = 70f;
    [SerializeField, Min(0.01f)] private float refreshInterval = 0.08f;
    [SerializeField, Range(1f, 180f)] private float retentionConeAngle = 45f;
    [SerializeField, Min(1f)] private float rotationSpeed = 900f;
    [SerializeField] private LayerMask targetLayers = ~0;

    private readonly Collider[] candidateBuffer = new Collider[64];
    private ZombieAI currentTarget;
    private Transform aimOrigin;
    private float nextRefreshTime;

    public ZombieAI CurrentTarget
    {
        get { return IsTargetValid(currentTarget, retentionConeAngle) ? currentTarget : null; }
    }
    public float AimRange => aimRange;
    public float ConeAngle => coneAngle;

    public void SetAimOrigin(Transform origin)
    {
        aimOrigin = origin;
    }

    public void RefreshTarget()
    {
        if (GameSession.Instance != null && !GameSession.Instance.IsPlaying)
        {
            ClearTarget();
            return;
        }

        if (Time.time < nextRefreshTime && IsTargetValid(currentTarget, retentionConeAngle))
            return;

        nextRefreshTime = Time.time + refreshInterval;
        if (!IsTargetValid(currentTarget, retentionConeAngle))
            currentTarget = FindBestTarget();
    }

    public bool TryGetAimDirection(Transform origin, out Vector3 direction)
    {
        SetAimOrigin(origin);
        ZombieAI target = CurrentTarget;
        if (target == null)
        {
            direction = transform.forward;
            return false;
        }

        Vector3 delta = GetAimPoint(target) - origin.position;
        if (delta.sqrMagnitude < 0.0001f)
        {
            direction = transform.forward;
            return false;
        }

        direction = delta.normalized;
        return true;
    }

    public void RotateTowardTarget(bool shouldAim)
    {
        if (!shouldAim)
            return;

        ZombieAI target = CurrentTarget;
        if (target == null)
            return;

        Vector3 direction = GetAimPoint(target) - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.0001f)
            return;

        Quaternion desiredRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            desiredRotation,
            rotationSpeed * Time.deltaTime);
    }

    public void ClearTarget()
    {
        currentTarget = null;
        nextRefreshTime = 0f;
    }

    private ZombieAI FindBestTarget()
    {
        Vector3 origin = transform.position;
        int count = Physics.OverlapSphereNonAlloc(origin, aimRange, candidateBuffer, targetLayers,
            QueryTriggerInteraction.Ignore);
        ZombieAI best = null;
        float bestScore = float.MaxValue;
        float halfCone = coneAngle * 0.5f;

        for (int i = 0; i < count; i++)
        {
            Collider candidateCollider = candidateBuffer[i];
            ZombieAI candidate = candidateCollider != null
                ? candidateCollider.GetComponentInParent<ZombieAI>()
                : null;
            if (candidate == null || !IsTargetValid(candidate, coneAngle))
                continue;

            Vector3 toTarget = GetAimPoint(candidate) - origin;
            toTarget.y = 0f;
            float distance = toTarget.magnitude;
            float angle = Vector3.Angle(Flat(transform.forward), toTarget / Mathf.Max(distance, 0.0001f));
            float score = angle / Mathf.Max(halfCone, 0.0001f) * 0.65f
                        + distance / Mathf.Max(aimRange, 0.0001f) * 0.35f;
            if (score < bestScore && HasLineOfSight(candidate))
            {
                best = candidate;
                bestScore = score;
            }
        }

        return best;
    }

    private bool IsTargetValid(ZombieAI target, float allowedCone)
    {
        if (target == null || !target.isActiveAndEnabled || target.IsDead)
            return false;

        Vector3 toTarget = GetAimPoint(target) - transform.position;
        toTarget.y = 0f;
        float distance = toTarget.magnitude;
        if (distance > aimRange || distance < 0.01f)
            return false;

        float angle = Vector3.Angle(Flat(transform.forward), toTarget / distance);
        return angle <= allowedCone * 0.5f && HasLineOfSight(target);
    }

    private bool HasLineOfSight(ZombieAI target)
    {
        Vector3 origin = aimOrigin != null ? aimOrigin.position : transform.position + Vector3.up;
        Vector3 point = GetAimPoint(target);
        Vector3 delta = point - origin;
        float distance = delta.magnitude;
        if (distance < 0.01f)
            return true;

        if (!Physics.Raycast(origin, delta / distance, out RaycastHit hit, distance,
            ~0, QueryTriggerInteraction.Ignore))
            return true;

        return hit.collider.GetComponentInParent<ZombieAI>() == target;
    }

    private static Vector3 GetAimPoint(ZombieAI target)
    {
        Collider collider = target.GetComponentInChildren<Collider>();
        return collider != null ? collider.bounds.center : target.transform.position + Vector3.up;
    }

    private static Vector3 Flat(Vector3 value)
    {
        value.y = 0f;
        return value.sqrMagnitude > 0.0001f ? value.normalized : Vector3.forward;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position + Vector3.up * 0.05f;
        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.7f);
        Gizmos.DrawWireSphere(origin, aimRange);
        Vector3 forward = Flat(transform.forward) * aimRange;
        Vector3 left = Quaternion.Euler(0f, -coneAngle * 0.5f, 0f) * forward;
        Vector3 right = Quaternion.Euler(0f, coneAngle * 0.5f, 0f) * forward;
        Gizmos.DrawLine(origin, origin + left);
        Gizmos.DrawLine(origin, origin + right);
        if (CurrentTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin, GetAimPoint(CurrentTarget));
        }
    }
}
