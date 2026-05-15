using System.Collections.Generic;
using UnityEngine;

public class XPOrbPool : MonoBehaviour
{
    public static XPOrbPool Instance { get; private set; }

    [SerializeField] XPOrb prefab;
    [SerializeField] Transform orbsRoot;
    [SerializeField] int initialSize = 30;

    readonly Stack<XPOrb> _inactive = new();

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

    public XPOrb Get(Vector3 position, float xpAmount)
    {
        var orb = _inactive.Count > 0
            ? _inactive.Pop()
            : Instantiate(prefab, position, Quaternion.identity, orbsRoot);

        orb.gameObject.SetActive(false);
        orb.transform.position = position;
        orb.gameObject.SetActive(true);
        orb.Init(xpAmount);
        return orb;
    }

    public void Return(XPOrb orb)
    {
        orb.gameObject.SetActive(false);
        _inactive.Push(orb);
    }
}