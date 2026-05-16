using UnityEngine;

public class XPOrb : MonoBehaviour
{
    float _xpAmount;

    static readonly Color ColorSmall  = Color.yellow;
    static readonly Color ColorMedium = Color.cyan;
    static readonly Color ColorLarge  = new Color(1f, 0.5f, 0f);
    static readonly Color ColorElite  = new Color(0.8f, 0f, 1f);
    [SerializeField] AudioClip collectSound;
    Renderer _renderer;
    MaterialPropertyBlock _propBlock;
    OrbFloat _orbFloat;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _propBlock = new MaterialPropertyBlock();
        _orbFloat = GetComponent<OrbFloat>();
    }

    public void Init(float xpAmount)
    {
        _xpAmount = xpAmount;
        ApplyVisual(xpAmount);
    }

    void ApplyVisual(float xp)
    {
        (float scale, Color color) = xp switch
        {
            <= 10f => (0.6f, ColorSmall),
            <= 20f => (0.9f, ColorMedium),
            <= 30f => (1.2f, ColorLarge),
            _      => (1.6f, ColorElite),
        };

        transform.localScale = Vector3.one * scale;

        _renderer.GetPropertyBlock(_propBlock);
        _propBlock.SetColor("_BaseColor", color);
        _propBlock.SetColor("_EmissionColor", color * 2f);
        _renderer.SetPropertyBlock(_propBlock);

        _orbFloat.SetEmissionColor(color * 2f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            Collect();
    }

    public void Collect()
    {
        AudioPool.Instance.Play(collectSound, transform.position, volume: 0.3f, pitch: Random.Range(0.5f, 1.2f));
        XPSystem.Instance.AddXP(_xpAmount);
        XPOrbPool.Instance.Return(this);
    }
}