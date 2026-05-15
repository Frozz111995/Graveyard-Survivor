using UnityEngine;

public class XPOrb : MonoBehaviour
{
    float _xpAmount;

    static readonly Color ColorSmall  = Color.yellow;
    static readonly Color ColorMedium = Color.cyan;
    static readonly Color ColorLarge  = new Color(1f, 0.5f, 0f); // оранжевый
    static readonly Color ColorElite  = new Color(0.8f, 0f, 1f); // фиолетовый

    Renderer _renderer;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    public void Init(float xpAmount)
    {
        _xpAmount = xpAmount;
        ApplyVisual(xpAmount);
    }

    void ApplyVisual(float xp)
    {
        // размер и цвет по значению xp
        (float scale, Color color) = xp switch
        {
            <= 10f  => (0.6f, ColorSmall),
            <= 20f  => (0.9f, ColorMedium),
            <= 30f  => (1.2f, ColorLarge),
            _       => (1.6f, ColorElite),
        };

        transform.localScale = Vector3.one * scale;
        _renderer.material.color = color;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            Collect();
    }

    public void Collect()
    {
        XPSystem.Instance.AddXP(_xpAmount);
        XPOrbPool.Instance.Return(this);
    }
}