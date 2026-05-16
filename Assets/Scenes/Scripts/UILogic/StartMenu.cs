// StartMenu.cs
using UnityEngine;
using System.Collections;

public class StartMenu : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] float animDuration = 0.3f;
    [SerializeField] PropSpawnSystem propSpawnSystem;
    public static bool GameStarted = false;

    void Start()
    {
        if (GameStarted)
        {
            panel.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            panel.SetActive(true);
            panel.transform.localScale = Vector3.one;
            Time.timeScale = 0f;
        }
    }

    public void StartGame()
    {
        GameStarted = true;
        StartCoroutine(ScaleOut());
    }

    IEnumerator ScaleOut()
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
        Time.timeScale = 1f;
    }
}