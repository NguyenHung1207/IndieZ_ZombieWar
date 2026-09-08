using System;
using UnityEngine;

public sealed class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField, Min(1f)] private float maxHealth = 100f;

    private float currentHealth;

    public event Action Died;
    public event Action<float, float, float> Damaged;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead { get; private set; }

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (IsDead || amount <= 0f)
            return;

        currentHealth = Mathf.Max(0f, currentHealth - amount);
        Damaged?.Invoke(currentHealth, maxHealth, amount);
        if (currentHealth <= 0f)
        {
            IsDead = true;
            Died?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        if (IsDead || amount <= 0f) return;
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
    }
}
