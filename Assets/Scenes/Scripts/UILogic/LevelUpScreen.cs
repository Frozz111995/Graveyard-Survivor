// LevelUpScreen.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpScreen : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] UpgradeCard[] cards; // 3 карточки
    [SerializeField] Upgrade[] allUpgrades;

    GameObject _player;

    void Start()
    {
        _player = GameObject.FindWithTag("Player");
        XPSystem.Instance.OnLevelUp += HandleLevelUp;
        panel.SetActive(false);
    }

    void OnDestroy()
    {
        XPSystem.Instance.OnLevelUp -= HandleLevelUp;
    }

    void HandleLevelUp(int level)
    {
        Time.timeScale = 0;
        panel.SetActive(true);
        ShowUpgrades();
    }

    void ShowUpgrades()
    {
        var shuffled = GetRandom3();
        for (int i = 0; i < cards.Length; i++)
            cards[i].Setup(shuffled[i], OnUpgradeChosen);
    }

    Upgrade[] GetRandom3()
    {
        var list = new System.Collections.Generic.List<Upgrade>(allUpgrades);
        var result = new Upgrade[3];

        for (int i = 0; i < 3; i++)
        {
            int idx = Random.Range(0, list.Count);
            result[i] = list[idx];
            list.RemoveAt(idx);
        }

        return result;
    }

    void OnUpgradeChosen(Upgrade upgrade)
    {
        upgrade.Apply();
        panel.SetActive(false);
        Time.timeScale = 1;
    }
}