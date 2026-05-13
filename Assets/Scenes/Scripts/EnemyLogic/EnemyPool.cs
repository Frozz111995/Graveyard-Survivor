// EnemyPool.cs
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance { get; private set; }

    [SerializeField] EnemyConfig[] configs;
    [SerializeField] Transform enemiesRoot;

    // отдельный стек на каждый конфиг
    Dictionary<EnemyConfig, Stack<EnemyAI>> _pools = new();

    void Awake()
    {
        Instance = this;

        foreach (var config in configs)
        {
            var stack = new Stack<EnemyAI>();

            for (int i = 0; i < config.initialPoolSize; i++)
            {
                var e = CreateEnemy(config);
                stack.Push(e);
            }

            _pools[config] = stack;
        }
    }

    public EnemyAI Get(Vector3 position, Transform player, EnemyConfig config)
    {
        var stack = _pools[config];

        var e = stack.Count > 0
            ? stack.Pop()
            : CreateEnemy(config);

        e.gameObject.SetActive(true);
        e.Init(player, config);
        e.Teleport(position);
        return e;
    }

    public void Return(EnemyAI enemy)
    {
        enemy.transform.position = Vector3.down * 1000f;
        enemy.gameObject.SetActive(false);

        // находим конфиг по префабу
        foreach (var config in configs)
        {
            if (enemy.gameObject.name.StartsWith(config.prefab.name))
            {
                _pools[config].Push(enemy);
                return;
            }
        }
    }

    EnemyAI CreateEnemy(EnemyConfig config)
    {
        var e = Instantiate(config.prefab, Vector3.down * 1000f, Quaternion.identity, enemiesRoot);
        e.gameObject.SetActive(false);
        return e;
    }

    // удобный метод для спавнера — вернуть случайный конфиг по весам
    public EnemyConfig GetRandomConfig()
    {
        float total = 0f;
        foreach (var c in configs) total += c.spawnWeight;

        float roll = Random.Range(0f, total);
        float cumulative = 0f;

        foreach (var c in configs)
        {
            cumulative += c.spawnWeight;
            if (roll <= cumulative) return c;
        }

        return configs[0];
    }
}