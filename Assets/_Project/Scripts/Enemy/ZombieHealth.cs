using System;
using UnityEngine;

public sealed class ZombieHealth : MonoBehaviour, IDamageable
{
    [SerializeField, Min(1f)] private float maxHealth = 100f;
    [SerializeField, Min(0f)] private float hitFlashDuration = 0.08f;
    [SerializeField] private Renderer[] hitRenderers;

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");
    private MaterialPropertyBlock propertyBlock;
    private Color[] baseColors;
    private float flashUntil;
    private float currentHealth;

    public event Action Died;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead { get; private set; }

    private void Awake()
    {
        currentHealth = maxHealth;
        if (hitRenderers == null || hitRenderers.Length == 0)
            hitRenderers = GetComponentsInChildren<Renderer>(true);

        propertyBlock = new MaterialPropertyBlock();
        baseColors = new Color[hitRenderers.Length];
        for (int i = 0; i < hitRenderers.Length; i++)
        {
            Material material = hitRenderers[i].sharedMaterial;
            baseColors[i] = material != null && material.HasProperty(BaseColorId)
                ? material.GetColor(BaseColorId)
                : material != null && material.HasProperty(ColorId)
                    ? material.GetColor(ColorId)
                    : Color.white;
        }
    }

    private void Update()
    {
        if (flashUntil <= Time.time)
        {
            if (flashUntil > 0f)
            {
                flashUntil = 0f;
                ApplyColor(0f);
            }
            return;
        }

        ApplyColor(1f);
    }

    public void TakeDamage(float amount)
    {
        if (IsDead || amount <= 0f || (GameSession.Instance != null && !GameSession.Instance.IsPlaying))
            return;

        currentHealth = Mathf.Max(0f, currentHealth - amount);
        flashUntil = Time.time + hitFlashDuration;
        ApplyColor(1f);
        if (currentHealth <= 0f)
        {
            IsDead = true;
            Died?.Invoke();
        }
    }

    private void ApplyColor(float flashAmount)
    {
        for (int i = 0; i < hitRenderers.Length; i++)
        {
            Renderer renderer = hitRenderers[i];
            if (renderer == null)
                continue;
            Color color = Color.Lerp(baseColors[i], Color.white, flashAmount);
            renderer.GetPropertyBlock(propertyBlock);
            if (renderer.sharedMaterial != null && renderer.sharedMaterial.HasProperty(BaseColorId))
                propertyBlock.SetColor(BaseColorId, color);
            else
                propertyBlock.SetColor(ColorId, color);
            renderer.SetPropertyBlock(propertyBlock);
        }
    }
}
