// EnemyHealth.cs
using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    float _maxHp;
    float _currentHp;
    bool _isDead;

    public event Action OnDeath;
    public event Action<float, float> OnDamaged; // current, max

    EnemyAI _ai;

    public float MaxHP => _maxHp;
    public float CurrentHP => _currentHp;

    void Awake()
    {
        _ai = GetComponent<EnemyAI>();
    }

    public void Init(float maxHp)
    {
        _maxHp = maxHp;
        _currentHp = maxHp;
        _isDead = false;
    }

    public void TakeDamage(float amount)
    {
        if (_isDead) return;

        _currentHp = Mathf.Max(0, _currentHp - amount);
        OnDamaged?.Invoke(_currentHp, _maxHp);

        if (_currentHp <= 0)
            Die();
    }

    public void SetMaxHP(float newMaxHP)
    {
        if (newMaxHP <= 0f)
            newMaxHP = 1f;

        float hpPercent = _maxHp > 0f ? _currentHp / _maxHp : 1f;

        _maxHp = newMaxHP;
        _currentHp = _maxHp * hpPercent;

        if (_currentHp > _maxHp)
            _currentHp = _maxHp;

        if (_currentHp < 0f)
            _currentHp = 0f;

        OnDamaged?.Invoke(_currentHp, _maxHp);
    }

    void Die()
    {
        _isDead = true;
        OnDeath?.Invoke();
        EnemyPool.Instance.Return(_ai);
    }
}