// EnemyAI.cs
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

    EnemyHealth _health;

    public Vector3 Velocity => (_player.position - transform.position).normalized * _moveSpeed;
    void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _health = GetComponent<EnemyHealth>();
    }

    public void Init(Transform player, EnemyConfig config)
    {
        _player = player;
        _moveSpeed = config.moveSpeed;
        _contactDamage = config.contactDamage;
        _damageCooldown = config.damageCooldown;

        _health.Init(config.maxHP);
    }

    void Update()
    {
        if (_player == null) return;

        Vector3 direction = (_player.position - transform.position).normalized;
        direction.y = 0f;

        if (_cc.isGrounded)
            _velocityY = 0f;
        else
            _velocityY += Physics.gravity.y * Time.deltaTime;

        Vector3 move = direction * _moveSpeed * Time.deltaTime;
        move.y = _velocityY * Time.deltaTime;

        _cc.Move(move);

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
    
    public void ApplyElite(EnemyConfig config)
    {
        _moveSpeed *= config.eliteSpeedMult;
        _health.SetMaxHP(_health.MaxHP * config.eliteHpMult);
        transform.localScale *= config.eliteSizeMult;

        if (config.eliteMaterial != null)
            GetComponentInChildren<Renderer>().material = config.eliteMaterial;
    }
}