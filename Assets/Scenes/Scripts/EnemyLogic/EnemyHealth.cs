using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float maxHp = 100f;

    float _currentHp;

    public event Action OnDeath;
    public event Action<float, float> OnDamaged; // current, max — для UI хп бара
    EnemyAI _ai;

    void Awake()
    {
        _ai = GetComponent<EnemyAI>();
    }
    void OnEnable()
    {
        _currentHp = maxHp;
    }

    public void TakeDamage(float amount)
    {
        if (_currentHp <= 0) return; // уже мёртв, игнорируем

        _currentHp = Mathf.Max(0, _currentHp - amount);
        OnDamaged?.Invoke(_currentHp, maxHp);

        if (_currentHp <= 0)
            Die();
    }

    void Die()
    {
        OnDeath?.Invoke();
        EnemyPool.Instance.Return(_ai);
    }
}