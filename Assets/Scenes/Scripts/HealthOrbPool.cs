using System.Collections.Generic;
using UnityEngine;

public class HealthOrbPool : MonoBehaviour
{
    public static HealthOrbPool Instance { get; private set; }

    [SerializeField] HealthOrb prefab;
    [SerializeField] Transform orbsRoot;
    [SerializeField] int initialSize = 10;

    readonly Stack<HealthOrb> _inactive = new();

    void Awake()
    {
        Instance = this;

        for (int i = 0; i < initialSize; i++)
        {
            var orb = Instantiate(prefab, orbsRoot);
            orb.gameObject.SetActive(false);
            _inactive.Push(orb);
        }
    }

    public HealthOrb Get(Vector3 position)
    {
        var orb = _inactive.Count > 0
            ? _inactive.Pop()
            : Instantiate(prefab, position, Quaternion.identity, orbsRoot);

        orb.gameObject.SetActive(false);
        orb.transform.position = position;
        orb.gameObject.SetActive(true);
        return orb;
    }

    public void Return(HealthOrb orb)
    {
        orb.gameObject.SetActive(false);
        _inactive.Push(orb);
    }
}