using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    EnemyAI _currentTarget;

    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float attackRadius = 10f;

    float _cooldownTimer;
    PlayerVisuals _visuals;
    PlayerMovement _movement;
    Transform _root;

    void Start()
    {
        _visuals = transform.root.GetComponentInChildren<PlayerVisuals>();
        _movement = transform.root.GetComponentInChildren<PlayerMovement>();
        _root = _visuals.transform;
    }

    void Update()
    {
        _cooldownTimer -= Time.deltaTime;
        if (_cooldownTimer > 0) return;

        var enemy = FindClosestEnemy();
        if (enemy == null) return;

        StartCoroutine(FireBurst());
        _cooldownTimer = PlayerStats.Instance.attackCooldown;
    }

    Vector3 PredictPosition(EnemyAI enemy, Vector3 origin, float projectileSpeed)
    {
        Vector3 enemyCenter = enemy.GetComponent<Collider>().bounds.center;
        float distance = Vector3.Distance(origin, enemyCenter);
        float timeToReach = Mathf.Min(distance / projectileSpeed, 0.3f); // клампим время
        return enemyCenter + enemy.Velocity * timeToReach;
    }

    IEnumerator FireBurst()
    {
        _visuals?.SetAttack();
        _movement?.LockRotation(PlayerStats.Instance.attackCooldown);

        StartCoroutine(TrackTarget());

        for (int i = 0; i < PlayerStats.Instance.projectileCount; i++)
        {
            if (Time.timeScale == 0) yield break;
            if (_currentTarget == null) yield break;

            Vector3 diff = _currentTarget.transform.position - transform.position;
            diff.y = 0f;
            bool isOnTop = diff.magnitude < 0.2f;
            bool isClose = diff.magnitude < 1.5f;

            Vector3 origin = (isOnTop || isClose)
                ? _currentTarget.GetComponent<Collider>().bounds.center
                : transform.position + Vector3.up * 0.5f;

            Vector3 direction = isOnTop
                ? Vector3.down
                : isClose
                    ? Vector3.down
                    : PredictPosition(_currentTarget, origin, 10f) - origin;

            if (!isOnTop)
            {
                direction += new Vector3(
                    Random.Range(-0.1f, 0.1f),
                    0.1f,
                    Random.Range(-0.1f, 0.1f)
                );
            }

            ProjectilePool.Instance.Get(origin, direction);
            yield return new WaitForSecondsRealtime(PlayerStats.Instance.burstInterval);
        }
    }

    IEnumerator TrackTarget()
    {
        float timer = PlayerStats.Instance.attackCooldown;
        while (timer > 0f)
        {
            if (_currentTarget != null)
            {
                Vector3 lookDir = _currentTarget.transform.position - _root.position;
                lookDir.y = 0f;
                if (lookDir.sqrMagnitude > 0.01f)
                    _root.rotation = Quaternion.Slerp(_root.rotation, Quaternion.LookRotation(lookDir.normalized), 15f * Time.deltaTime);
            }
            timer -= Time.deltaTime;
            yield return null;
        }
    }

    EnemyAI FindClosestEnemy()
    {
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