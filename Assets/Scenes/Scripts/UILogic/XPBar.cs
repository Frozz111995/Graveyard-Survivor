// XPBar.cs

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class XPBar : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TMPro.TextMeshProUGUI levelText;

    int _currentLevel;

    void Start()
    {
        XPSystem.Instance.OnXPChanged += HandleXPChanged;
        XPSystem.Instance.OnLevelUp += HandleLevelUp;
        LocalizationManager.OnLanguageChanged += UpdateLevelText;
        UpdateLevelText();
    }

    void OnDestroy()
    {
        XPSystem.Instance.OnXPChanged -= HandleXPChanged;
        XPSystem.Instance.OnLevelUp -= HandleLevelUp;
        LocalizationManager.OnLanguageChanged -= UpdateLevelText;
    }

    void HandleXPChanged(float current, float required)
    {
        slider.value = current / required;
    }

    void HandleLevelUp(int level)
    {
        _currentLevel = level;
        UpdateLevelText();
    }

    void UpdateLevelText()
    {
        levelText.text = $"{LocalizationManager.Get("Level")} {_currentLevel}";
        StartCoroutine(RebuildNextFrame(levelText.rectTransform));
    }

    IEnumerator RebuildNextFrame(RectTransform rect)
    {
        yield return null; // ждём конец кадра
        var current = rect;
        while (current != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(current);
            current = current.parent as RectTransform;
        }
    }
}