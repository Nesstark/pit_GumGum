using System;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("Stats")]
    [Tooltip("Current health")] [SerializeField]
    private int health = 100;

    [Tooltip("Maximum health")] [SerializeField]
    private int maxHealth = 100;

    [Tooltip("Base damage")] [SerializeField]
    private int damage = 10;

    public event Action<int> OnHealthChanged;

    public event Action OnDied;

    public int Health => health;
    public int MaxHealth => maxHealth;
    public int Damage => damage;

    protected virtual void Awake()
    {
        maxHealth = Mathf.Max(1, maxHealth);
        health = Mathf.Clamp(health, 0, maxHealth);
    }

    public virtual int TakeDamage(int amount)
    {
        if (amount <= 0 || IsDead()) return 0;

        int prev = health;
        health = Mathf.Clamp(health - amount, 0, maxHealth);
        int applied = prev - health;

        OnHealthChanged?.Invoke(health);

        if (health <= 0)
            Die();
        
        return applied;
    }

    public virtual int Heal(int amount)
    {
        if (amount <= 0 || IsDead()) return 0;

        int prev = health;
        health = Mathf.Clamp(health + amount, 0, maxHealth);
        int healed = health - prev;

        if (healed > 0)
            OnHealthChanged?.Invoke(health);

        return healed;
    }

    protected virtual void Die()
    {
        OnDied?.Invoke();
        gameObject.SetActive(false); // TODO; trigger death animation, drops, etc.
    }

    public bool IsDead() => health <= 0;

    public virtual void RestoreToFullHealth()
    {
        health = maxHealth;
        OnHealthChanged?.Invoke(health);
    }

    public virtual int Attack(Unit target)
    {
        if (target == null || IsDead()) return 0;
        return target.TakeDamage(damage);
    }
}
