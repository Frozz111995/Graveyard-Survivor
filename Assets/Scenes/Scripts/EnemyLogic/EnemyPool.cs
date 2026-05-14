using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance { get; private set; }
    public EnemyConfig[] Configs => configs;
    [SerializeField] EnemyConfig[] configs;
    [SerializeField] Transform enemiesRoot;

    Dictionary<EnemyConfig, Stack<EnemyAI>> _pools = new();

    public int ActiveCount { get; private set; }

    void Awake()
    {
        Instance = this;

        foreach (var config in configs)
        {
            var stack = new Stack<EnemyAI>();
            for (int i = 0; i < config.initialPoolSize; i++)
                stack.Push(CreateEnemy(config));
            _pools[config] = stack;
        }
    }

    public EnemyAI Get(Vector3 position, Transform player, EnemyConfig config, bool isElite = false)
    {
        var stack = _pools[config];
        var e = stack.Count > 0 ? stack.Pop() : CreateEnemy(config);

        e.ResetVisuals();
        e.transform.localScale = config.prefab.transform.localScale;
        e.gameObject.SetActive(true);
        e.Init(player, config);
        e.Teleport(position);

        if (isElite) e.ApplyElite(config);

        ActiveCount++;
        return e;
    }

    public void Return(EnemyAI enemy)
    {
        enemy.transform.position = Vector3.down * 1000f;
        enemy.gameObject.SetActive(false);
        ActiveCount = Mathf.Max(0, ActiveCount - 1);

        foreach (var config in configs)
        {
            if (enemy.gameObject.name.StartsWith(config.prefab.name))
            {
                _pools[config].Push(enemy);
                return;
            }
        }
    }

    public (EnemyConfig config, bool isElite) GetRandomConfig()
    {
        float elapsed = Time.timeSinceLevelLoad;
        float minutes = elapsed / 60f;

        float total = 0f;
        foreach (var c in configs)
            if (elapsed >= c.unlockAfterSeconds) total += c.spawnWeight;

        float roll = Random.Range(0f, total);
        float cumulative = 0f;
        EnemyConfig picked = configs[0];

        foreach (var c in configs)
        {
            if (elapsed < c.unlockAfterSeconds) continue;
            cumulative += c.spawnWeight;
            if (roll <= cumulative) { picked = c; break; }
        }

        bool isElite = false;
        if (picked.canSpawnElite)
        {
            float eliteUnlockTime = picked.unlockAfterSeconds + picked.eliteUnlockDelay;
            if (elapsed >= eliteUnlockTime)
            {
                float minutesSinceEliteUnlock = (elapsed - eliteUnlockTime) / 60f;
                float chance = Mathf.Clamp(
                    picked.eliteChanceBase + picked.eliteChancePerMinute * minutesSinceEliteUnlock,
                    0f, picked.eliteChanceMax
                );
                isElite = Random.value < chance;
            }
        }

        return (picked, isElite);
    }

    EnemyAI CreateEnemy(EnemyConfig config)
    {
        var e = Instantiate(config.prefab, Vector3.down * 1000f, Quaternion.identity, enemiesRoot);
        e.gameObject.SetActive(false);
        return e;
    }
}