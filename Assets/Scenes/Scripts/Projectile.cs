using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] float margin = 1f;

    Vector3 _direction;
    float _planeY;
    Camera _cam;

    void Awake()
    {
        _cam = Camera.main;
    }

    void OnEnable()
    {
        _planeY = transform.position.y;
    }

    public void Init(Vector3 direction)
    {
        _direction = direction.normalized;
    }

    void Update()
    {
        transform.position += _direction * (speed * Time.deltaTime);

        if (IsOutOfBounds(transform.position))
            ReturnToPool();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) return; // игнорируем игрока
    
        if (other.TryGetComponent<EnemyHealth>(out var health))
        {
            health.TakeDamage(PlayerStats.Instance.damage);
            ReturnToPool();
        }
    }

    bool IsOutOfBounds(Vector3 worldPos)
    {
        Vector3[] vp = { new(0,0,0), new(1,0,0), new(0,1,0), new(1,1,0) };
        Vector3[] c = new Vector3[4];

        for (int i = 0; i < 4; i++)
        {
            Ray r = _cam.ViewportPointToRay(vp[i]);
            c[i] = r.origin + r.direction * ((_planeY - r.origin.y) / r.direction.y);
        }

        float minX = Mathf.Min(c[0].x, c[2].x);
        float maxX = Mathf.Max(c[1].x, c[3].x);
        float minZ = Mathf.Min(c[0].z, c[1].z);
        float maxZ = Mathf.Max(c[2].z, c[3].z);

        return worldPos.x < minX - margin ||
               worldPos.x > maxX + margin ||
               worldPos.z < minZ - margin ||
               worldPos.z > maxZ + margin;
    }

    void ReturnToPool()
    {
        ProjectilePool.Instance.Return(this);
    }
}