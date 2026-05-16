// LevelUpScreen.cs
using UnityEngine;
using System.Collections;

public class LevelUpScreen : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] UpgradeCard[] cards;
    [SerializeField] Upgrade[] allUpgrades;
    [SerializeField] float animDuration = 0.3f;

    void Start()
    {
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
        panel.transform.localScale = Vector3.zero;
        ShowUpgrades();
        StartCoroutine(ScaleIn());
    }

    IEnumerator ScaleIn()
    {
        float elapsed = 0f;
        while (elapsed < animDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / animDuration);
            panel.transform.localScale = Vector3.one * Mathf.SmoothStep(0f, 1f, t);
            yield return null;
        }
        panel.transform.localScale = Vector3.one;
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
        StartCoroutine(ScaleOut(upgrade));
    }

    IEnumerator ScaleOut(Upgrade upgrade)
    {
        float elapsed = 0f;
        while (elapsed < animDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / animDuration);
            panel.transform.localScale = Vector3.one * Mathf.SmoothStep(1f, 0f, t);
            yield return null;
        }
        panel.transform.localScale = Vector3.one;
        panel.SetActive(false);
        Time.timeScale = 1;
    }
}