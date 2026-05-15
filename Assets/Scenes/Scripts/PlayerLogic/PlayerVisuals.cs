using System.Collections;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    [SerializeField] float flashDuration = 0.1f;
    [SerializeField] Color flashColor = Color.red;
    [SerializeField] AudioClip[] hitSounds;
    [SerializeField] AudioClip[] deathSounds;
    [SerializeField] float attackRotationLock = 1f;

    static readonly int ColorProp  = Shader.PropertyToID("_BaseColor");
    static readonly int AnimRun    = Animator.StringToHash("Run");
    static readonly int AnimAttack = Animator.StringToHash("Attack");
    static readonly int AnimDeath  = Animator.StringToHash("Death");

    Renderer[] _renderers;
    Color[] _originalColors;
    MaterialPropertyBlock _block;
    PlayerStats _stats;
    Animator _animator;

    public bool IsAttacking { get; private set; }

    void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>();
        _originalColors = new Color[_renderers.Length];
        for (int i = 0; i < _renderers.Length; i++)
            _originalColors[i] = _renderers[i].sharedMaterial.color;

        _animator = GetComponentInChildren<Animator>();
        _block = new MaterialPropertyBlock();

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
        foreach (var r in _renderers)
        {
            _block.SetColor(ColorProp, color);
            r.SetPropertyBlock(_block);
        }
    }

    void ResetColor()
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            _block.SetColor(ColorProp, _originalColors[i]);
            _renderers[i].SetPropertyBlock(_block);
        }
    }

    AudioClip GetRandom(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return null;
        return clips[Random.Range(0, clips.Length)];
    }
}