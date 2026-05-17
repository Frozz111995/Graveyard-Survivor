// PropSpawnSystem.cs
using System.Collections.Generic;
using UnityEngine;

// Маркер-компонент для хранения индекса префаба
public class PropIndex : MonoBehaviour
{
    public int value;
}

public class PropSpawnSystem : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public GameObject[] propPrefabs;
    public Transform propRoot; // ← корень для всех спавнящихся пропсов

    [Header("Tuning")]
    public float chunkSize       = 20f;
    public int   viewDistance    = 3;
    public int   propsPerChunk   = 5;
    public int   poolSize        = 300;
    public float updateInterval  = 0.4f;
    public float groundY         = -1f;
    public float scaleMultiplier = 1f;
    public float safeZoneRadius  = 5f;

    [SerializeField] PhysicMaterial slipperyMaterial;
    [SerializeField] float minPropDistance = 3f;

    // Отдельный пул для каждого префаба
    readonly Dictionary<int, Stack<GameObject>>        _pools     = new();
    readonly Dictionary<Vector2Int, List<GameObject>>  _active    = new();
    readonly Dictionary<Vector2Int, List<PropData>>    _chunkData = new();

    Vector2Int _lastPlayerChunk = new(int.MaxValue, 0);
    float _timer;

    // ─────────────────────────────────────────────
    //  Инициализация
    // ─────────────────────────────────────────────

    public void Initialize(Transform playerTransform)
    {
        player = playerTransform;
        EnsurePropRoot();
        WarmPool();
        UpdateChunks(WorldToChunk(player.position));
    }

    void Start() { }

    // ─────────────────────────────────────────────
    //  Гарантируем наличие корня
    // ─────────────────────────────────────────────

    void EnsurePropRoot()
    {
        if (propRoot != null) return;

        // Если не задан в инспекторе — создаём автоматически
        var go = new GameObject("PropRoot");
        propRoot = go.transform;
    }

    // ─────────────────────────────────────────────
    //  Прогрев пула
    // ─────────────────────────────────────────────

    void WarmPool()
    {
        int countPerPrefab = Mathf.Max(1, poolSize / propPrefabs.Length);

        for (int i = 0; i < propPrefabs.Length; i++)
        {
            _pools[i] = new Stack<GameObject>();

            for (int j = 0; j < countPerPrefab; j++)
            {
                var go = CreateProp(i);
                go.SetActive(false);
                _pools[i].Push(go);
            }
        }
    }

    // ─────────────────────────────────────────────
    //  Создание одного пропа с правильным коллайдером
    // ─────────────────────────────────────────────

    GameObject CreateProp(int prefabIndex)
    {
        // Инстанциируем сразу в propRoot
        var go = Instantiate(propPrefabs[prefabIndex], propRoot);
        go.name = $"Prop_{prefabIndex}";

        // Запоминаем индекс через компонент-маркер
        go.AddComponent<PropIndex>().value = prefabIndex;

        // Удаляем все существующие коллайдеры (могли быть на префабе)
        foreach (var col in go.GetComponentsInChildren<Collider>())
            Destroy(col);

        // Ищем меш (может быть на дочернем объекте)
        var meshFilter = go.GetComponentInChildren<MeshFilter>();
        var box = go.AddComponent<BoxCollider>();

        if (meshFilter != null)
        {
            Bounds b = meshFilter.sharedMesh.bounds;

            box.center = go.transform.InverseTransformPoint(
                meshFilter.transform.TransformPoint(b.center)
            );

            Vector3 worldSize = meshFilter.transform.TransformVector(b.size);
            box.size = new Vector3(
                Mathf.Abs(worldSize.x) / go.transform.lossyScale.x,
                Mathf.Abs(worldSize.y) / go.transform.lossyScale.y,
                Mathf.Abs(worldSize.z) / go.transform.lossyScale.z
            );
        }

        if (slipperyMaterial != null)
            box.material = slipperyMaterial;

        return go;
    }

    // ─────────────────────────────────────────────
    //  Update
    // ─────────────────────────────────────────────

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

    // ─────────────────────────────────────────────
    //  Управление чанками
    // ─────────────────────────────────────────────

    void UpdateChunks(Vector2Int center)
    {
        var needed = new HashSet<Vector2Int>();
        for (int x = -viewDistance; x <= viewDistance; x++)
        for (int z = -viewDistance; z <= viewDistance; z++)
            needed.Add(center + new Vector2Int(x, z));

        // Деактивируем чанки вне зоны видимости
        var toRemove = new List<Vector2Int>();
        foreach (var kv in _active)
        {
            if (!needed.Contains(kv.Key))
            {
                foreach (var go in kv.Value)
                    ReturnToPool(go);

                toRemove.Add(kv.Key);
            }
        }
        foreach (var key in toRemove) _active.Remove(key);

        // Спавним новые чанки
        foreach (var chunk in needed)
        {
            if (!_active.ContainsKey(chunk))
                SpawnChunk(chunk);
        }
    }

    void SpawnChunk(Vector2Int chunk)
    {
        var positions = GetChunkData(chunk);
        var spawned   = new List<GameObject>(positions.Count);

        foreach (var data in positions)
        {
            var go = GetFromPool(data.prefabIndex);
            if (go == null) continue;

            go.transform.SetParent(propRoot, worldPositionStays: false); // ← гарантируем правильный родитель
            go.transform.position   = data.position;
            go.transform.rotation   = data.rotation;
            go.transform.localScale = data.scale * scaleMultiplier;
            go.SetActive(true);
            spawned.Add(go);
        }

        _active[chunk] = spawned;
    }

    // ─────────────────────────────────────────────
    //  Работа с пулом
    // ─────────────────────────────────────────────

    GameObject GetFromPool(int prefabIndex)
    {
        if (_pools.TryGetValue(prefabIndex, out var pool) && pool.Count > 0)
            return pool.Pop();

        // Пул пустой — создаём новый объект на лету
        return CreateProp(prefabIndex);
    }

    void ReturnToPool(GameObject go)
    {
        go.SetActive(false);

        var marker = go.GetComponent<PropIndex>();
        if (marker != null && _pools.TryGetValue(marker.value, out var pool))
            pool.Push(go);
        else
            Destroy(go);
    }

    // ─────────────────────────────────────────────
    //  Генерация данных чанка
    // ─────────────────────────────────────────────

    List<PropData> GetChunkData(Vector2Int chunk)
    {
        if (_chunkData.TryGetValue(chunk, out var cached)) return cached;

        var rng    = new System.Random(chunk.x * 73856093 ^ chunk.y * 19349663);
        var list   = new List<PropData>(propsPerChunk);
        var origin = new Vector3(chunk.x * chunkSize, groundY, chunk.y * chunkSize);

        int attempts = 0;
        while (list.Count < propsPerChunk && attempts < propsPerChunk * 10)
        {
            attempts++;

            var pos = origin + new Vector3(
                (float)(rng.NextDouble() * chunkSize),
                0f,
                (float)(rng.NextDouble() * chunkSize)
            );

            if (Vector3.Distance(pos, Vector3.zero) < safeZoneRadius) continue;

            bool tooClose = false;
            foreach (var existing in list)
            {
                if (Vector3.Distance(pos, existing.position) < minPropDistance)
                {
                    tooClose = true;
                    break;
                }
            }
            if (tooClose) continue;

            list.Add(new PropData
            {
                position    = pos,
                rotation    = Quaternion.Euler(0f, (float)(rng.NextDouble() * 360f), 0f),
                scale       = Vector3.one * (0.8f + (float)(rng.NextDouble() * 0.5f)),
                prefabIndex = rng.Next(0, propPrefabs.Length),
            });
        }

        _chunkData[chunk] = list;
        return list;
    }

    // ─────────────────────────────────────────────
    //  Утилиты
    // ─────────────────────────────────────────────

    Vector2Int WorldToChunk(Vector3 pos)
        => new(Mathf.FloorToInt(pos.x / chunkSize),
               Mathf.FloorToInt(pos.z / chunkSize));

    struct PropData
    {
        public Vector3    position;
        public Quaternion rotation;
        public Vector3    scale;
        public int        prefabIndex;
    }
}