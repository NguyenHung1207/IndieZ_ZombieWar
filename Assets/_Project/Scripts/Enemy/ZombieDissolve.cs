using UnityEngine;

public sealed class ZombieDissolve : MonoBehaviour
{
    [SerializeField, Min(0f)] private float startDelay = 0.7f;
    [SerializeField, Min(0.05f)] private float duration = 1f;
    [SerializeField] private Renderer[] renderers;

    private static readonly int DissolveAmountId = Shader.PropertyToID("_DissolveAmount");
    private MaterialPropertyBlock propertyBlock;
    private bool started;
    private float startTime;

    private void Awake()
    {
        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<Renderer>(true);
        propertyBlock = new MaterialPropertyBlock();
        SetAmount(0f);
    }

    public void Begin()
    {
        if (started)
            return;
        started = true;
        startTime = Time.time + startDelay;
    }

    private void Update()
    {
        if (!started || Time.time < startTime)
            return;
        float amount = Mathf.Clamp01((Time.time - startTime) / duration);
        SetAmount(amount);
        if (amount >= 1f)
            Destroy(gameObject);
    }

    private void SetAmount(float amount)
    {
        if (renderers == null)
            return;
        foreach (Renderer renderer in renderers)
        {
            if (renderer == null)
                continue;
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat(DissolveAmountId, amount);
            renderer.SetPropertyBlock(propertyBlock);
        }
    }
}
