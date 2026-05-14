using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    float _moveSpeed;
    float _contactDamage;
    float _damageCooldown;

    Transform _player;
    CharacterController _cc;
    float _velocityY;
    float _lastDamageTime = -999f;
    bool _isElite;
    GameObject _eliteOnDeathFx;
    EnemyHealth _health;
    Renderer _renderer;
    Material _originalMaterial;
    public GameObject GetEliteDeathFx() => _eliteOnDeathFx;
    public Vector3 Velocity => (_player.position - transform.position).normalized * _moveSpeed;

    void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _health = GetComponent<EnemyHealth>();
        _renderer = GetComponent<Renderer>();
        _originalMaterial = _renderer.material;
    }

    public void Init(Transform player, EnemyConfig config)
    {
        _player = player;
        _moveSpeed = config.moveSpeed;
        _contactDamage = config.contactDamage;
        _damageCooldown = config.damageCooldown;
        _health.Init(config.maxHP);
    }

    public void ApplyElite(EnemyConfig config)
    {
        _isElite = true;
        _eliteOnDeathFx = config.eliteOnDeathFx;
        _moveSpeed *= config.eliteSpeedMult;
        _health.SetMaxHP(_health.MaxHP * config.eliteHpMult);
        transform.localScale *= config.eliteSizeMult;

        if (config.eliteMaterial != null)
        {
            _renderer.material = config.eliteMaterial;
            GetComponent<EnemyVisuals>().SetBaseColor(config.eliteMaterial.color);
        }
    }

    public void ResetVisuals()
    {
        _isElite = false;
        _eliteOnDeathFx = null;
        _renderer.material = _originalMaterial;
        GetComponent<EnemyVisuals>().SetBaseColor(_originalMaterial.color);
    }

    void Update()
    {
        if (_player == null) return;

        Vector3 diff = _player.position - transform.position;
        diff.y = 0f;
        Vector3 direction = diff.normalized;

        if (_cc.isGrounded) _velocityY = 0f;
        else _velocityY += Physics.gravity.y * Time.deltaTime;

        _cc.Move(direction * _moveSpeed * Time.deltaTime + Vector3.up * _velocityY * Time.deltaTime);

        if (direction != Vector3.zero)
            transform.forward = direction;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!hit.gameObject.CompareTag("Player")) return;
        if (Time.time - _lastDamageTime < _damageCooldown) return;

        _lastDamageTime = Time.time;
        PlayerStats.Instance.TakeDamage(_contactDamage);
    }

    public void Teleport(Vector3 position)
    {
        _cc.enabled = false;
        transform.position = position;
        _cc.enabled = true;
        _velocityY = 0f;
    }
}