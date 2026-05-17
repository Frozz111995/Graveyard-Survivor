using System;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static event Action OnLanguageChanged;
    public static LocalizationManager Instance;
    public LocalizationData data;

    public enum Language { English, Russian }
    public static Language Current = Language.English;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DetectLanguage();
        }
        else Destroy(gameObject);
    }

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            var next = Current == Language.Russian ? Language.English : Language.Russian;
            SetLanguage(next);
            Debug.Log($"[Localization] Switched to: {next}");
        }
    }
#endif

    void DetectLanguage()
    {
        var lang = Application.systemLanguage;
        Current = lang == SystemLanguage.Russian ? Language.Russian : Language.English;
    }

    public static string Get(string key)
    {
        foreach (var entry in Instance.data.entries)
        {
            if (entry.key == key)
                return Current == Language.Russian ? entry.russian : entry.english;
        }
        return key;
    }

    public static void SetLanguage(Language lang)
    {
        Current = lang;
        foreach (var lt in FindObjectsOfType<LocalizedText>())
            lt.UpdateText();
        OnLanguageChanged?.Invoke();
    }
}