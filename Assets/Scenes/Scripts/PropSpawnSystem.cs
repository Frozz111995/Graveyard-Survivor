// PropSpawnSystem.cs
using System.Collections.Generic;
using UnityEngine;

public class PropSpawnSystem : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public GameObject[] propPrefabs;

    [Header("Tuning")]
    public float chunkSize      = 20f;
    public int   viewDistance   = 3;
    public int   propsPerChunk  = 5;
    public int   poolSize       = 300;
    public float updateInterval = 0.4f;
    public float groundY        = -1f;
    public float scaleMultiplier = 1f; 
    public float safeZoneRadius = 5f;
    
    readonly Stack<GameObject>                        _pool      = new();
    readonly Dictionary<Vector2Int, List<GameObject>> _active    = new();
    readonly Dictionary<Vector2Int, List<PropData>>   _chunkData = new();
    Vector2Int _lastPlayerChunk = new(int.MaxValue, 0);
    float _timer;

    void Start()
    {
        WarmPool();
    }

    void WarmPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            var go = Instantiate(propPrefabs[Random.Range(0, propPrefabs.Length)]);
            go.AddComponent<SphereCollider>();
            go.SetActive(false);
            _pool.Push(go);
        }
    }

    void Update()
    {
        if (player == null) return;

        _timer += Time.deltaTime;
        if (_timer < updateInterval) return;
        _timer = 0f;

        var playerChunk = WorldToChunk(player.position);
        if (playerChunk == _lastPlayerChunk) return;
        _lastPlayerChunk = playerChunk;

        UpdateChunks(playerChunk);
    }

    public void Initialize(Transform playerTransform)
    {
        player = playerTransform;
    }

    void UpdateChunks(Vector2Int center)
    {
        var needed = new HashSet<Vector2Int>();
        for (int x = -viewDistance; x <= viewDistance; x++)
        for (int z = -viewDistance; z <= viewDistance; z++)
            needed.Add(center + new Vector2Int(x, z));

        var toRemove = new List<Vector2Int>();
        foreach (var kv in _active)
        {
            if (!needed.Contains(kv.Key))
            {
                foreach (var go in kv.Value)
                {
                    go.SetActive(false);
                    _pool.Push(go);
                }
                toRemove.Add(kv.Key);
            }
        }
        foreach (var key in toRemove) _active.Remove(key);

        foreach (var chunk in needed)
        {
            if (_active.ContainsKey(chunk)) continue;
            SpawnChunk(chunk);
        }
    }

    void SpawnChunk(Vector2Int chunk)
    {
        var positions = GetChunkData(chunk);
        var spawned   = new List<GameObject>(positions.Count);

        foreach (var data in positions)
        {
            if (_pool.Count == 0) break;

            var go = _pool.Pop();
            go.transform.position   = data.position;
            go.transform.rotation   = data.rotation;
            go.transform.localScale = data.scale * scaleMultiplier;
            go.SetActive(true);
            spawned.Add(go);
        }

        _active[chunk] = spawned;
    }

    List<PropData> GetChunkData(Vector2Int chunk)
    {
        if (_chunkData.TryGetValue(chunk, out var cached)) return cached;

        var rng    = new System.Random(chunk.x * 73856093 ^ chunk.y * 19349663);
        var list   = new List<PropData>(propsPerChunk);
        var origin = new Vector3(chunk.x * chunkSize, groundY, chunk.y * chunkSize);

        for (int i = 0; i < propsPerChunk; i++)
        {
            var pos = origin + new Vector3((float)(rng.NextDouble() * chunkSize), 0f, (float)(rng.NextDouble() * chunkSize));
    
            // пропускаем если слишком близко к центру мира (точка старта игрока)
            if (pos.magnitude < safeZoneRadius) continue;
            
            list.Add(new PropData
            {
                position = origin + new Vector3((float)(rng.NextDouble() * chunkSize), 0f, (float)(rng.NextDouble() * chunkSize)),
                rotation = Quaternion.Euler(0f, (float)(rng.NextDouble() * 360f), 0f),
                scale    = Vector3.one * (0.8f + (float)(rng.NextDouble() * 0.5f)),
            });
        }

        _chunkData[chunk] = list;
        return list;
    }

    Vector2Int WorldToChunk(Vector3 pos)
        => new(Mathf.FloorToInt(pos.x / chunkSize),
               Mathf.FloorToInt(pos.z / chunkSize));

    struct PropData
    {
        public Vector3    position;
        public Quaternion rotation;
        public Vector3    scale;
    }
}