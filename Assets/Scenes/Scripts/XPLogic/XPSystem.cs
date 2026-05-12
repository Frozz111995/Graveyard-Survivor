// XPSystem.cs
using System;
using UnityEngine;

public class XPSystem : MonoBehaviour
{
    public static XPSystem Instance { get; private set; }

    [SerializeField] float baseXP = 100f;
    [SerializeField] float scaling = 1.5f;

    float _currentXP;
    int _currentLevel;

    public event Action<float, float> OnXPChanged;  // current, required
    public event Action<int> OnLevelUp;             // new level

    public int CurrentLevel => _currentLevel;
    public float CurrentXP => _currentXP;
    public float RequiredXP => baseXP * Mathf.Pow(scaling, _currentLevel);

    void Awake()
    {
        Instance = this;
    }

    public void AddXP(float amount)
    {
        _currentXP += amount;
        OnXPChanged?.Invoke(_currentXP, RequiredXP);
        CheckLevelUp();
    }

    void CheckLevelUp()
    {
        if (_currentXP < RequiredXP) return;

        _currentXP -= RequiredXP;
        _currentLevel++;
        OnLevelUp?.Invoke(_currentLevel);
        OnXPChanged?.Invoke(_currentXP, RequiredXP); // добавить
        CheckLevelUp();
    }
}