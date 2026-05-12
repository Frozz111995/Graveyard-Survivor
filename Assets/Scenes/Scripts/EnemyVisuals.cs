// EnemyVisuals.cs
using System.Collections;
using UnityEngine;

public class EnemyVisuals : MonoBehaviour
{
    [SerializeField] float flashDuration = 0.1f;
    [SerializeField] Color flashColor = Color.white;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem hitParticles;
    [SerializeField] AudioClip[] deathSounds;
    [SerializeField] AudioClip[] hitSounds;

    static readonly int ColorProp = Shader.PropertyToID("_BaseColor");

    Renderer _renderer;
    MaterialPropertyBlock _block;
    Color _originalColor;
    EnemyHealth _health;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _block = new MaterialPropertyBlock();
        _originalColor = _renderer.sharedMaterial.color;

        _health = GetComponent<EnemyHealth>();
        _health.OnDamaged += HandleDamaged;
        _health.OnDeath += HandleDeath;
    }

    void OnDestroy()
    {
        _health.OnDamaged -= HandleDamaged;
        _health.OnDeath -= HandleDeath;
    }

    void OnEnable()
    {
        ResetColor();
    }

    void HandleDamaged(float current, float max)
    {
        AudioPool.Instance.Play(GetRandom(hitSounds), transform.position);

        if (hitParticles != null)
            ParticlePool.Instance.PlayAt(hitParticles, transform.position);

        StopAllCoroutines();
        StartCoroutine(Flash());
    }

    void HandleDeath()
    {
        AudioPool.Instance.Play(GetRandom(deathSounds), transform.position);

        if (deathParticles != null)
            ParticlePool.Instance.PlayAt(deathParticles, transform.position);
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