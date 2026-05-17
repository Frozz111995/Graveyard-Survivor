// GameOverScreen.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] GameObject panel;

    void Start()
    {
        panel.SetActive(false);
        PlayerStats.Instance.OnDeath += HandleDeath;
    }

    void OnDestroy()
    {
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.OnDeath -= HandleDeath;
    }

    void HandleDeath()
    {
        YG2.InterstitialAdvShow();
        panel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
}