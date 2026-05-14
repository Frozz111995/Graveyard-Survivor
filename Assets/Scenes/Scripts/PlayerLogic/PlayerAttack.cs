// PlayerAttack.cs
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerAttack : MonoBehaviour
{
    public float attackCooldown = 1f;
    public int projectileCount = 1;
    public float burstInterval = 0.1f;

    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float attackRadius = 10f;

    float _cooldownTimer;

    void Update()
    {
        _cooldownTimer -= Time.deltaTime;
        if (_cooldownTimer > 0) return;

        var enemy = FindClosestEnemy();
        if (enemy == null) return;

        Vector3 origin = transform.position + Vector3.up * 0.5f; // примерно центр игрока
        Vector3 direction = PredictPosition(enemy, origin, 10f) - origin;
        direction += new Vector3(
            Random.Range(-0.1f, 0.1f),
            0f,
            Random.Range(-0.1f, 0.1f)
        );
        StartCoroutine(FireBurst(origin, direction));
        _cooldownTimer = PlayerStats.Instance.attackCooldown;
    }
    
    Vector3 PredictPosition(EnemyAI enemy, Vector3 origin, float projectileSpeed)
    {
        float distance = Vector3.Distance(origin, enemy.transform.position);
        float timeToReach = distance / projectileSpeed;
        return enemy.transform.position + enemy.Velocity * timeToReach;
    }

    IEnumerator FireBurst(Vector3 origin, Vector3 direction)
    {
        for (int i = 0; i < PlayerStats.Instance.projectileCount; i++)
        {
            if (Time.timeScale == 0) yield break;
            ProjectilePool.Instance.Get(origin, direction);
            yield return new WaitForSecondsRealtime(PlayerStats.Instance.burstInterval);
        }
    }

    EnemyAI FindClosestEnemy()
    {
        var hits = Physics.OverlapSphere(transform.position, attackRadius, enemyLayer);

        EnemyAI closest = null;
        float closestDist = float.MaxValue;

        foreach (var hit in hits)
        {
            float dist = Vector3.SqrMagnitude(hit.transform.position - transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = hit.GetComponent<EnemyAI>();
            }
        }

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