// UpgradeCard.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UpgradeCard : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] Button button;

    Upgrade _upgrade;

    void OnEnable()
    {
        LocalizationManager.OnLanguageChanged += UpdateTexts;
    }

    void OnDisable()
    {
        LocalizationManager.OnLanguageChanged -= UpdateTexts;
    }

    public void Setup(Upgrade upgrade, Action<Upgrade> onChosen)
    {
        _upgrade = upgrade;
        icon.sprite = upgrade.icon;
        UpdateTexts();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onChosen(upgrade));
    }

    void UpdateTexts()
    {
        if (_upgrade == null) return;
        nameText.text = LocalizationManager.Get(_upgrade.nameKey);
        descriptionText.text = LocalizationManager.Get(_upgrade.descriptionKey);
    }
}