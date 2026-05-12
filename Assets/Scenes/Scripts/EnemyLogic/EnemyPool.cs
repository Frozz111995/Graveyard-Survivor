using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance { get; private set; }

    [SerializeField] EnemyAI prefab;
    [SerializeField] Transform enemiesRoot;
    [SerializeField] int initialSize = 20;

    readonly Stack<EnemyAI> _inactive = new();

    void Awake()
    {
        Instance = this;

        for (int i = 0; i < initialSize; i++)
        {
            var e = Instantiate(prefab, Vector3.down * 1000f, Quaternion.identity, enemiesRoot);
            e.gameObject.SetActive(false);
            _inactive.Push(e);
        }
    }

    public EnemyAI Get(Vector3 position, Transform player)
    {
        var e = _inactive.Count > 0
            ? _inactive.Pop()
            : Instantiate(prefab, enemiesRoot);

        e.gameObject.SetActive(true);
        e.Init(player);
        e.Teleport(position);
        return e;
    }

    public void Return(EnemyAI enemy)
    {
        enemy.transform.position = Vector3.down * 1000f;
        enemy.gameObject.SetActive(false);
        _inactive.Push(enemy);
    }
}