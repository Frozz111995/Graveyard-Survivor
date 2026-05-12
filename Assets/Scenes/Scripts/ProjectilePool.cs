using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    public static ProjectilePool Instance { get; private set; }
    [SerializeField] Transform projectileRoot;
    [SerializeField] Projectile prefab;
    [SerializeField] int initialSize = 20;

    readonly Stack<Projectile> _inactive = new();

    void Awake()
    {
        Instance = this;

        for (int i = 0; i < initialSize; i++)
        {
            var p = Instantiate(prefab, projectileRoot);
            p.gameObject.SetActive(false);
            _inactive.Push(p);
        }
    }

    public Projectile Get(Vector3 position, Vector3 direction)
    {
        var p = _inactive.Count > 0
            ? _inactive.Pop()
            : Instantiate(prefab); // пул кончился — создаём новый

        p.transform.position = position;
        p.gameObject.SetActive(true);
        p.Init(direction);
        return p;
    }

    public void Return(Projectile p)
    {
        p.gameObject.SetActive(false);
        _inactive.Push(p);
    }
}