// PlayerVisuals.cs
using System.Collections;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    [SerializeField] float flashDuration = 0.1f;
    [SerializeField] Color flashColor = Color.red;
    [SerializeField] AudioClip[] hitSounds;
    [SerializeField] AudioClip[] deathSounds;

    static readonly int ColorProp = Shader.PropertyToID("_BaseColor");

    Renderer _renderer;
    MaterialPropertyBlock _block;
    Color _originalColor;
    PlayerStats _stats;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
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