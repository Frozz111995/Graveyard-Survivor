using System.Collections;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    [SerializeField] float flashDuration = 0.1f;
    [SerializeField] Color flashColor = Color.red;
    [SerializeField] AudioClip[] hitSounds;
    [SerializeField] AudioClip[] deathSounds;
    [SerializeField] float attackRotationLock = 1f; // подбери под длину анимации
    static readonly int ColorProp  = Shader.PropertyToID("_BaseColor");
    static readonly int AnimRun    = Animator.StringToHash("Run");
    static readonly int AnimAttack = Animator.StringToHash("Attack");
    static readonly int AnimDeath  = Animator.StringToHash("Death");

    Renderer _renderer;
    MaterialPropertyBlock _block;
    Color _originalColor;
    PlayerStats _stats;
    Animator _animator;

    public bool IsAttacking { get; private set; }

    void Awake()
    {
        _renderer = GetComponentInChildren<Renderer>();
        _animator = GetComponentInChildren<Animator>();
        _block = new MaterialPropertyBlock();
        _originalColor = _renderer.sharedMaterial.color;

        _stats = PlayerStats.Instance;
        _stats.OnDamaged += HandleDamaged;
        _stats.OnDeath += HandleDeath;
    }

    void OnDestroy()
    {
        if (PlayerStats.Instance == null) return;
        _stats.OnDamaged -= HandleDamaged;
        _stats.OnDeath -= HandleDeath;
    }

    public void SetRun(bool isRunning) => _animator?.SetBool(AnimRun, isRunning);

    public void SetAttack()
    {
        IsAttacking = true;
        _animator?.SetTrigger(AnimAttack);
        StartCoroutine(ResetAttacking());
    }

    IEnumerator ResetAttacking()
    {
        yield return new WaitForSeconds(attackRotationLock);
        IsAttacking = false;
    }

    void HandleDamaged(float current, float max)
    {
        AudioPool.Instance.Play(GetRandom(hitSounds), transform.position);
        CameraFollow.Instance.Shake(0.2f, 5f);

        StopAllCoroutines();
        StartCoroutine(Flash());
    }

    void HandleDeath()
    {
        AudioPool.Instance.Play(GetRandom(deathSounds), transform.position);
        _animator?.SetBool(AnimRun, false);
        _animator?.SetBool(AnimDeath, true);
    }

    IEnumerator Flash()
    {
        SetColor(flashColor);
        yield return new WaitForSeconds(flashDuration);
        ResetColor();
    }

    void SetColor(Color color)
    {
        _block.SetColor(ColorProp, color);
        _renderer.SetPropertyBlock(_block);
    }

    void ResetColor()
    {
        _block.SetColor(ColorProp, _originalColor);
        _renderer.SetPropertyBlock(_block);
    }

    AudioClip GetRandom(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return null;
        return clips[Random.Range(0, clips.Length)];
    }
}