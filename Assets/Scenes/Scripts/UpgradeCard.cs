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

    public void Setup(Upgrade upgrade, Action<Upgrade> onChosen)
    {
        icon.sprite = upgrade.icon;
        nameText.text = upgrade.upgradeName;
        descriptionText.text = upgrade.description;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onChosen(upgrade));
    }
}