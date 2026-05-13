// EnemySpawner.cs
using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] float margin = 3f;
    [SerializeField] float spawnInterval = 1f;

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
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemy()
    {
        var config = EnemyPool.Instance.GetRandomConfig();
        EnemyPool.Instance.Get(GetSpawnPos(), _player, config);
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
}