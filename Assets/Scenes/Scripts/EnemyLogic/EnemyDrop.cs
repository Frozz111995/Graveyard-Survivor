// EnemyDrop.cs
using UnityEngine;

public class EnemyDrop : MonoBehaviour
{
    EnemyHealth _health;

    void Awake()
    {
        _health = GetComponent<EnemyHealth>();
        _health.OnDeath += HandleDeath;
    }

    void OnDestroy()
    {
        _health.OnDeath -= HandleDeath;
    }

    void HandleDeath()
    {
        XPOrbPool.Instance.Get(transform.position);
    }
}