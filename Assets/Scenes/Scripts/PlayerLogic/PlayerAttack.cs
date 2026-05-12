using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] float attackCooldown = 1f;
    [SerializeField] float attackRadius = 10f;
    [SerializeField] LayerMask enemyLayer;

    float _cooldownTimer;

    void Update()
    {
        _cooldownTimer -= Time.deltaTime;

        if (_cooldownTimer > 0) return;

        var enemy = FindClosestEnemy();
        if (enemy == null) return;

        Vector3 direction = enemy.position - transform.position;
        ProjectilePool.Instance.Get(transform.position, direction);
        _cooldownTimer = attackCooldown;
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