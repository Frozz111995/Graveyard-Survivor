using System;
using UnityEngine;
using YG;

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
        }
        else Destroy(gameObject);
    }

    void OnEnable()
    {
        YG2.onSwitchLang += OnLangChanged;
        YG2.onCorrectLang += OnLangChanged;
    }

    void OnDisable()
    {
        YG2.onSwitchLang -= OnLangChanged;
        YG2.onCorrectLang -= OnLangChanged;
    }

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            var next = Current == Language.Russian ? "en" : "ru";
            YG2.SwitchLanguage(next);
            Debug.Log($"[Localization] Switched to: {next}");
        }
    }
#endif

    void OnLangChanged(string lang)
    {
        Current = lang == "ru" ? Language.Russian : Language.English;
        Debug.Log($"[Localization] Language set: {lang} → {Current}");
        SetLanguage(Current);
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