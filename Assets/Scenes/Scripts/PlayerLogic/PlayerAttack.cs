// PlayerAttack.cs
using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackCooldown = 1f;
    public int projectileCount = 1;
    public float burstInterval = 0.1f;

    EnemyAI _currentTarget;

    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float attackRadius = 10f;

    float _cooldownTimer;

    void Update()
    {
        _cooldownTimer -= Time.deltaTime;
        if (_cooldownTimer > 0) return;

        var enemy = FindClosestEnemy();
        if (enemy == null) return;

        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 direction = PredictPosition(enemy, origin, 10f) - origin;
        direction += new Vector3(
            Random.Range(-0.1f, 0.1f),
            0f,
            Random.Range(-0.1f, 0.1f)
        );
        StartCoroutine(FireBurst(direction));
        _cooldownTimer = PlayerStats.Instance.attackCooldown;
    }

    Vector3 PredictPosition(EnemyAI enemy, Vector3 origin, float projectileSpeed)
    {
        Vector3 enemyCenter = enemy.transform.position;
        float distance = Vector3.Distance(origin, enemyCenter);

        if (distance < 3f)
            return enemyCenter;

        float timeToReach = distance / projectileSpeed;
        return enemyCenter + enemy.Velocity * timeToReach;
    }

    IEnumerator FireBurst(Vector3 direction)
    {
        for (int i = 0; i < PlayerStats.Instance.projectileCount; i++)
        {
            if (Time.timeScale == 0) yield break;
            if (_currentTarget == null) yield break;

            Vector3 origin = transform.position + Vector3.up * 0.5f;
            ProjectilePool.Instance.Get(origin, direction);
            yield return new WaitForSecondsRealtime(PlayerStats.Instance.burstInterval);
        }
    }

    EnemyAI FindClosestEnemy()
    {
        if (_currentTarget != null)
        {
            float dist = Vector3.Distance(transform.position, _currentTarget.transform.position);
            if (dist <= attackRadius)
                return _currentTarget;
        }

        var hits = Physics.OverlapSphere(transform.position, attackRadius, enemyLayer);

        EnemyAI closest = null;
        float closestDist = float.MaxValue;

        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<EnemyAI>();
            if (enemy == null) continue;

            Vector3 diff = hit.transform.position - transform.position;
            diff.y = 0f;
            float dist = diff.sqrMagnitude;

            if (dist < closestDist)
            {
                closestDist = dist;
                closest = enemy;
            }
        }

        _currentTarget = closest;
        return closest;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
#endif
}