// LocalizationData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizationData", menuName = "Localization/Data")]
public class LocalizationData : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        public string key;
        [TextArea] public string english;
        [TextArea] public string russian;
    }

    public Entry[] entries;
}