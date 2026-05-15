using UnityEngine;

public class EnemyDrop : MonoBehaviour
{
    [SerializeField] float healthOrbChance = 0.1f;
    EnemyHealth _health;
    EnemyAI _ai;

    void Awake()
    {
        _health = GetComponent<EnemyHealth>();
        _ai = GetComponent<EnemyAI>();
        _health.OnDeath += HandleDeath;
    }

    void OnDestroy()
    {
        _health.OnDeath -= HandleDeath;
    }

    
    void HandleDeath()
    {
        Vector3 deathPos = transform.position;

        float xp = _ai.Config.xpPerOrb * _ai.XpMult;
        int count = _ai.IsElite ? 1 : Mathf.RoundToInt(_ai.Config.xpOrbCount);

        for (int i = 0; i < count; i++)
        {
            Vector3 offset = Random.insideUnitSphere * 0.5f;
            offset.y = 0f;
            XPOrbPool.Instance.Get(deathPos + offset, xp);
        }

        if (Random.value < healthOrbChance)
        {
            Vector3 offset = Random.insideUnitSphere * 0.5f;
            offset.y = 0f;
            HealthOrbPool.Instance.Get(deathPos + offset);
        }
    }
}