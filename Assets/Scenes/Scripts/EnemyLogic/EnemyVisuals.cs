using System.Collections;
using UnityEngine;

public class EnemyVisuals : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float flashDuration = 0.1f;
    // Сделайте цвет ярче (например, HDR белый), чтобы он "перебивал" текстуру
    [SerializeField, ColorUsage(true, true)] Color flashColor = Color.white;
    
    [Header("Effects")]
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem hitParticles;
    [SerializeField] AudioClip[] deathSounds;
    [SerializeField] AudioClip[] hitSounds;

    // Свойство для URP — _BaseColor, для Standard — _Color
    static readonly int ColorProp = Shader.PropertyToID("_BaseColor");

    Renderer[] _renderers;
    Color[] _originalColors;
    MaterialPropertyBlock[] _blocks;
    EnemyHealth _health;
    Coroutine _flashCoroutine; // Ссылка на корутину, чтобы не останавливать всё остальное

    void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>();
        _originalColors = new Color[_renderers.Length];
        _blocks = new MaterialPropertyBlock[_renderers.Length];
    
        for (int i = 0; i < _renderers.Length; i++)
        {
            _blocks[i] = new MaterialPropertyBlock();
            
            // Прямое получение цвета через имя свойства надежнее в URP
            if (_renderers[i].sharedMaterial.HasProperty(ColorProp))
            {
                _originalColors[i] = _renderers[i].sharedMaterial.GetColor(ColorProp);
            }
            else
            {
                _originalColors[i] = Color.white;
            }
        }

        _health = GetComponent<EnemyHealth>();
        if (_health != null)
        {
            _health.OnDamaged += HandleDamaged;
            _health.OnDeath += HandleDeath;
        }
    }

    void OnDestroy()
    {
        if (_health != null)
        {
            _health.OnDamaged -= HandleDamaged;
            _health.OnDeath -= HandleDeath;
        }
    }

    void OnEnable()
    {
        // Сбрасываем цвет при активации (важно для пуллинга объектов)
        if (_renderers != null)
            ResetColor();
    }

    void HandleDamaged(float current, float max)
    {
        if (hitSounds.Length > 0)
            AudioPool.Instance.Play(GetRandom(hitSounds), transform.position);

        if (hitParticles != null)
            ParticlePool.Instance.PlayAt(hitParticles, transform.position);

        if (_flashCoroutine != null) StopCoroutine(_flashCoroutine);
        _flashCoroutine = StartCoroutine(Flash());
    }

    void HandleDeath()
    {
        if (deathSounds.Length > 0)
            AudioPool.Instance.Play(GetRandom(deathSounds), transform.position);

        if (deathParticles != null)
            ParticlePool.Instance.PlayAt(deathParticles, transform.position);
    }

    IEnumerator Flash()
    {
        SetColor(flashColor);
        yield return new WaitForSeconds(flashDuration);
        ResetColor();
        _flashCoroutine = null;
    }

    void SetColor(Color color)
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            // Получаем текущие настройки блока, чтобы не затереть другие свойства (например, текстуры)
            _renderers[i].GetPropertyBlock(_blocks[i]); 
            _blocks[i].SetColor(ColorProp, color);
            _renderers[i].SetPropertyBlock(_blocks[i]);
        }
    }

    public void SetBaseColor(Color color)
    {
        for (int i = 0; i < _originalColors.Length; i++)
            _originalColors[i] = color;
        ResetColor();
    }

    void ResetColor()
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].GetPropertyBlock(_blocks[i]);
            _blocks[i].SetColor(ColorProp, _originalColors[i]);
            _renderers[i].SetPropertyBlock(_blocks[i]);
        }
    }
    
    public void ResetOriginalColors()
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            if (_renderers[i].sharedMaterial.HasProperty(ColorProp))
                _originalColors[i] = _renderers[i].sharedMaterial.GetColor(ColorProp);
        }
        ResetColor();
    }

    AudioClip GetRandom(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return null;
        return clips[Random.Range(0, clips.Length)];
    }
}