// PlayerAttack.cs
using System.Collections;
using UnityEngine;

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

        Vector3 direction = enemy.position - transform.position;
        StartCoroutine(FireBurst(direction));
        _cooldownTimer = PlayerStats.Instance.attackCooldown;
    }

    IEnumerator FireBurst(Vector3 direction)
    {
        for (int i = 0; i < PlayerStats.Instance.projectileCount; i++)
        {
            if (Time.timeScale == 0) yield break;
            ProjectilePool.Instance.Get(transform.position, direction);
            yield return new WaitForSecondsRealtime(PlayerStats.Instance.burstInterval);
        }
    }

    Transform FindClosestEnemy()
    {
        var hits = Physics.OverlapSphere(transform.position, attackRadius, enemyLayer);

        Transform closest = null;
        float closestDist = float.MaxValue;

        foreach (var hit in hits)
        {
            float dist = Vector3.SqrMagnitude(hit.transform.position - transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = hit.transform;
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