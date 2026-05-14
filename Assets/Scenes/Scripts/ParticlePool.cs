using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePool : MonoBehaviour
{
    public static ParticlePool Instance { get; private set; }

    //[SerializeField] int initialSizePerPrefab = 3;

    readonly Dictionary<ParticleSystem, Stack<ParticleSystem>> _pools = new();

    void Awake() => Instance = this;

    public void PlayAt(ParticleSystem prefab, Vector3 position)
    {
        var ps = Get(prefab);
        ps.transform.position = position;
        ps.gameObject.SetActive(true);
        ps.Play();
        StartCoroutine(ReturnWhenDone(prefab, ps));
    }

    ParticleSystem Get(ParticleSystem prefab)
    {
        if (!_pools.ContainsKey(prefab))
            _pools[prefab] = new Stack<ParticleSystem>();

        var pool = _pools[prefab];

        if (pool.Count > 0)
            return pool.Pop();

        var ps = Instantiate(prefab, transform);
        var main = ps.main;
        main.stopAction = ParticleSystemStopAction.None;
        return ps;
    }

    IEnumerator ReturnWhenDone(ParticleSystem prefab, ParticleSystem instance)
    {
        yield return new WaitForSeconds(instance.main.duration);
        instance.Stop();
        instance.gameObject.SetActive(false);
        _pools[prefab].Push(instance);
    }
}