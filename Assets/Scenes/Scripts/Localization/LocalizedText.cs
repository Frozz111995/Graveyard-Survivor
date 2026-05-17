using TMPro;
using UnityEngine;

public class LocalizedText : MonoBehaviour
{
    public string key;
    TMP_Text _text;

    void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }

    void Start() => UpdateText();

    public void UpdateText()
    {
        if (_text == null) _text = GetComponent<TMP_Text>();
        _text.text = LocalizationManager.Get(key);
    }
}