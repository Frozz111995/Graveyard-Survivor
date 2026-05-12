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
        EnemyPool.Instance.Get(GetSpawnPos(), _player);
    }

    Vector3 GetSpawnPos()
    {
        float y = _player.position.y;
        Vector3[] vp = { new(0,0,0), new(1,0,0), new(0,1,0), new(1,1,0) };
        Vector3[] c = new Vector3[4];

        for (int i = 0; i < 4; i++)
        {
            Ray r = _cam.ViewportPointToRay(vp[i]);
            c[i] = r.origin + r.direction * ((y - r.origin.y) / r.direction.y);
        }

        float t = Random.value;
        int side = Random.Range(0, 4);

        Vector3 pos = side switch
        {
            0 => Vector3.Lerp(c[2], c[3], t),
            1 => Vector3.Lerp(c[0], c[1], t),
            2 => Vector3.Lerp(c[0], c[2], t),
            _ => Vector3.Lerp(c[1], c[3], t),
        };

        Vector3 camCenter = (c[0] + c[1] + c[2] + c[3]) / 4f;
        return pos + (pos - camCenter).normalized * margin;
    }
}