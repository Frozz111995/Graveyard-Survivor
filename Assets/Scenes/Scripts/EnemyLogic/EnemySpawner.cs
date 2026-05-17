// EnemySpawner.cs
using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Ramp Up")]
    [SerializeField] float startInterval = 3f;
    [SerializeField] float rampDuration = 180f; // за 3 минуты выйти на spawnInterval
    
    [SerializeField] float margin = 3f;
    [SerializeField] float spawnInterval = 1f;

    // ── Dynamic Soft Cap ──────────────────────────────────────────────────────
    [System.Serializable]
    public struct CapPhase
    {
        [Tooltip("До скольких минут действует эта фаза")]
        public float untilMinutes;
        [Tooltip("Soft cap: выше него спавн начинает замедляться")]
        public int softCap;
        [Tooltip("Hard cap: выше него спавн полностью останавливается")]
        public int hardCap;
    }

    [Header("Soft Cap Phases")]
    [SerializeField] CapPhase[] capPhases = new CapPhase[]
    {
        new CapPhase { untilMinutes = 5f,  softCap = 60,  hardCap = 80  },
        new CapPhase { untilMinutes = 10f, softCap = 90,  hardCap = 120 },
        new CapPhase { untilMinutes = 999f, softCap = 130, hardCap = 160 },
    };

    [Tooltip("Максимальное замедление интервала спавна (множитель) при подходе к hard cap")]
    [SerializeField] float maxSlowdownMultiplier = 5f;
    // ─────────────────────────────────────────────────────────────────────────

    Transform _player;
    Camera _cam;
    bool _started;

    public void SetPlayer(Transform playerTransform)
    {
        _player = playerTransform;
        _cam = Camera.main;

        if (!_started)
        {
            _started = true;
            StartCoroutine(SpawnLoop());
        }
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            float interval = GetCurrentInterval();

            // Hard cap достигнут — ждём не спавня
            if (interval < 0f)
            {
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            SpawnEnemy();
            yield return new WaitForSeconds(interval);
        }
    }

    // Возвращает текущий интервал спавна с учётом soft/hard cap.
    // Возвращает -1 если hard cap достигнут (спавн заблокирован).
    float GetCurrentInterval()
    {
        int active = EnemyPool.Instance.ActiveCount;
        CapPhase phase = GetCurrentPhase();

        if (active >= phase.hardCap)
            return -1f;

        // плавное нарастание от startInterval до spawnInterval
        float t = Mathf.Clamp01(Time.timeSinceLevelLoad / rampDuration);
        float interval = Mathf.Lerp(startInterval, spawnInterval, t);

        // soft cap замедляет сверху
        if (active > phase.softCap)
        {
            float slowT = (float)(active - phase.softCap) / (phase.hardCap - phase.softCap);
            interval *= Mathf.Lerp(1f, maxSlowdownMultiplier, slowT);
        }

        return interval;
    }

    CapPhase GetCurrentPhase()
    {
        float minutes = Time.time / 60f;

        foreach (var phase in capPhases)
        {
            if (minutes < phase.untilMinutes)
                return phase;
        }

        return capPhases[capPhases.Length - 1];
    }

    void SpawnEnemy()
    {
        var (config, isElite) = EnemyPool.Instance.GetRandomConfig();
        EnemyPool.Instance.Get(GetSpawnPos(), _player, config, isElite);
    }

    Vector3 GetSpawnPos()
    {
        Vector3[] vp = { new(0,0,0), new(1,0,0), new(0,1,0), new(1,1,0) };
        Vector3[] c = new Vector3[4];
        Vector3 camPos = _cam.transform.position;
        float camHeight = camPos.y - _player.position.y;

        for (int i = 0; i < 4; i++)
        {
            Ray r = _cam.ViewportPointToRay(vp[i]);
            float t = camHeight / -r.direction.y;
            c[i] = _player.position + (r.direction * t + (camPos - _player.position));
        }

        float tv = Random.value;
        Vector3 pos = Random.Range(0, 4) switch
        {
            0 => Vector3.Lerp(c[2], c[3], tv),
            1 => Vector3.Lerp(c[0], c[1], tv),
            2 => Vector3.Lerp(c[0], c[2], tv),
            _ => Vector3.Lerp(c[1], c[3], tv),
        };

        Vector3 camCenter = (c[0] + c[1] + c[2] + c[3]) / 4f;
        return pos + (pos - camCenter).normalized * margin;
    }
    
#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) DebugSpawn(0, false);
        if (Input.GetKeyDown(KeyCode.Alpha2)) DebugSpawn(1, false);
        if (Input.GetKeyDown(KeyCode.Alpha3)) DebugSpawn(2, false);
        if (Input.GetKeyDown(KeyCode.Alpha4)) DebugSpawn(0, true);
        if (Input.GetKeyDown(KeyCode.Alpha5)) DebugSpawn(1, true);
        if (Input.GetKeyDown(KeyCode.Alpha6)) DebugSpawn(2, true);
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            CapPhase phase = capPhases[capPhases.Length - 1];
            int toSpawn = phase.hardCap - EnemyPool.Instance.ActiveCount;
            for (int i = 0; i < toSpawn; i++)
                SpawnEnemy();
            Debug.Log($"[EnemySpawner] spawned {toSpawn} enemies | total: {EnemyPool.Instance.ActiveCount}");
        }
    }

    void DebugSpawn(int index, bool isElite)
    {
        var configs = EnemyPool.Instance.Configs;
        if (index >= configs.Length) return;
        EnemyPool.Instance.Get(GetSpawnPos(), _player, configs[index], isElite);
    }
#endif
}