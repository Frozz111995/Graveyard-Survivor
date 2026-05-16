// StartMenu.cs
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    [SerializeField] GameObject panel;

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
            Time.timeScale = 0f;
        }
    }

    public void StartGame()
    {
        GameStarted = true;
        panel.SetActive(false);
        Time.timeScale = 1f;
    }
}