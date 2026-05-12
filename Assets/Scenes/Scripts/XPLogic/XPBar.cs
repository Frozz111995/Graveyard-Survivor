// XPBar.cs
using UnityEngine;
using UnityEngine.UI;

public class XPBar : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TMPro.TextMeshProUGUI levelText;

    void Start()
    {
        XPSystem.Instance.OnXPChanged += HandleXPChanged;
        XPSystem.Instance.OnLevelUp += HandleLevelUp;
    }

    void OnDestroy()
    {
        XPSystem.Instance.OnXPChanged -= HandleXPChanged;
        XPSystem.Instance.OnLevelUp -= HandleLevelUp;
    }

    void HandleXPChanged(float current, float required)
    {
        slider.value = current / required;
    }

    void HandleLevelUp(int level)
    {
        levelText.text = $"Level {level}";
    }
}