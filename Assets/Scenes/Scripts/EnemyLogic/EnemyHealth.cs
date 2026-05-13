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

    void Die()
    {
        _isDead = true;
        OnDeath?.Invoke();
        EnemyPool.Instance.Return(_ai);
    }
}