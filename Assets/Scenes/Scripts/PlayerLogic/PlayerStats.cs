// PlayerStats.cs
using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [Header("Attack")]
    public float damage = 10f;
    public float attackCooldown = 1f;
    public float burstInterval = 0.1f;
    public int projectileCount = 1;

    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Health")]
    public float maxHP = 100f;
    public float CurrentHP { get; private set; }
    public bool IsAlive => !_isDead;
    public float HealthPercent => CurrentHP / maxHP;

    [Header("Invulnerability")]
    [SerializeField] float invulnerabilityDuration = 0.4f;
    public bool IsInvulnerable { get; private set; }

    public event Action OnDeath;
    public event Action<float, float> OnHealthChanged; // current, max — для UI хп бара

    bool _isDead;
    float _lastHitTime = -999f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        CurrentHP = maxHP;
    }

    void Update()
    {
        if (IsInvulnerable && Time.time - _lastHitTime >= invulnerabilityDuration)
            IsInvulnerable = false;
    }

    public void TakeDamage(float amount)
    {
        if (_isDead || IsInvulnerable) return;

        IsInvulnerable = true;
        _lastHitTime = Time.time;

        CurrentHP = Mathf.Max(0, CurrentHP - amount);
        OnHealthChanged?.Invoke(CurrentHP, maxHP);

        if (CurrentHP <= 0)
            Die();
    }

    public void Heal(float amount)
    {
        if (_isDead) return;

        CurrentHP = Mathf.Min(CurrentHP + amount, maxHP);
        OnHealthChanged?.Invoke(CurrentHP, maxHP);
    }

    public void UpgradeMaxHP(float amount)
    {
        maxHP += amount;
        CurrentHP += amount;
        OnHealthChanged?.Invoke(CurrentHP, maxHP);
    }

    void Die()
    {
        _isDead = true;
        OnDeath?.Invoke();
    }
}