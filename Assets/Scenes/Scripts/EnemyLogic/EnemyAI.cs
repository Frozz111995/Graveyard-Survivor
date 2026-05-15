using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    float _moveSpeed;
    float _contactDamage;
    float _damageCooldown;
    float _velocityY;
    float _lastDamageTime = -999f;
    float _eliteXpMult = 1f;
    bool _isElite;
    bool _isDead;
    GameObject _eliteOnDeathFx;

    Transform _player;
    CharacterController _cc;
    EnemyHealth _health;
    EnemyVisuals _visuals;
    Animator _animator;

    static readonly int AnimRun    = Animator.StringToHash("Run");
    static readonly int AnimAttack = Animator.StringToHash("Attack");
    static readonly int AnimDeath  = Animator.StringToHash("Death");

    public EnemyConfig Config { get; private set; }
    public bool IsElite => _isElite;
    public float XpMult => _eliteXpMult;
    public Vector3 Velocity => (_player.position - transform.position).normalized * _moveSpeed;
    public GameObject GetEliteDeathFx() => _eliteOnDeathFx;

    void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _health = GetComponent<EnemyHealth>();
        _visuals = GetComponent<EnemyVisuals>();
        _animator = GetComponentInChildren<Animator>();

        _health.OnDeath += HandleDeath;
    }

    void OnDestroy()
    {
        _health.OnDeath -= HandleDeath;
    }

    public void Init(Transform player, EnemyConfig config)
    {
        Config = config;
        _player = player;
        _moveSpeed = config.moveSpeed;
        _contactDamage = config.contactDamage;
        _damageCooldown = config.damageCooldown;
        _isDead = false;
        _health.Init(config.maxHP);

        _animator?.SetBool(AnimAttack, false);
        _animator?.SetBool(AnimDeath, false);
    }

    void HandleDeath()
    {
        _isDead = true;
        StopAllCoroutines();
        _animator?.SetBool(AnimRun, false);
        _animator?.SetBool(AnimAttack, false);
        _animator?.SetBool(AnimDeath, true);
    }

    public void ApplyElite(EnemyConfig config)
    {
        _isElite = true;
        _eliteOnDeathFx = config.eliteOnDeathFx;
        _eliteXpMult = config.eliteXpMult;
        _moveSpeed *= config.eliteSpeedMult;
        _health.SetMaxHP(_health.MaxHP * config.eliteHpMult);
        transform.localScale *= config.eliteSizeMult;

        if (config.eliteMaterial != null)
            _visuals.SetBaseColor(config.eliteMaterial.color);
    }

    public void ResetVisuals()
    {
        _isElite = false;
        _eliteOnDeathFx = null;
        _eliteXpMult = 1f;
        _visuals.ResetOriginalColors();
    }

    void Update()
    {
        if (_player == null || _isDead) return;

        Vector3 diff = _player.position - transform.position;
        diff.y = 0f;
        Vector3 direction = diff.normalized;
        float distanceToPlayer = diff.magnitude;
        bool isMoving = distanceToPlayer > _cc.radius + 0.1f;

        _animator?.SetBool(AnimRun, isMoving);

        if (_cc.isGrounded) _velocityY = 0f;
        else _velocityY += Physics.gravity.y * Time.deltaTime;

        _cc.Move(direction * _moveSpeed * Time.deltaTime + Vector3.up * _velocityY * Time.deltaTime);

        if (direction != Vector3.zero)
            transform.forward = direction;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (_isDead) return;
        if (!hit.gameObject.CompareTag("Player")) return;
        if (Time.time - _lastDamageTime < _damageCooldown) return;

        _lastDamageTime = Time.time;
        _animator?.SetBool(AnimAttack, true);
        PlayerStats.Instance.TakeDamage(_contactDamage);

        StartCoroutine(ResetAttack());
    }

    IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(_damageCooldown);
        _animator?.SetBool(AnimAttack, false);
    }

    public void Teleport(Vector3 position)
    {
        _cc.enabled = false;
        transform.position = position;
        _cc.enabled = true;
        _velocityY = 0f;
        _isDead = false;
    }
}