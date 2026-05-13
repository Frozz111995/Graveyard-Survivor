// PlayerHealthBar.cs
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Image fill; // опционально — менять цвет при низком HP

    void Start()
    {
        var stats = PlayerStats.Instance;
        UpdateBar(stats.CurrentHP, stats.maxHP);
        stats.OnHealthChanged += UpdateBar;
    }

    void OnDestroy()
    {
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.OnHealthChanged -= UpdateBar;
    }

    void UpdateBar(float current, float max)
    {
        slider.value = current / max;

        if (fill != null)
            fill.color = Color.Lerp(Color.red, Color.green, slider.value);
    }
}