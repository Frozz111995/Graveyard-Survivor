// Upgrade.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Upgrade")]
public class Upgrade : ScriptableObject
{
    [Header("Display")]
    public Sprite icon;

    [Header("Localization Keys")]
    public string nameKey;        // например "upgrade_damage_name"
    public string descriptionKey; // например "upgrade_damage_desc"

    [Header("Stats")]
    public UpgradeType type;
    public float value;

    public void Apply()
    {
        switch (type)
        {
            case UpgradeType.Damage:
                PlayerStats.Instance.damage += value;
                break;
            case UpgradeType.AttackSpeed:
                PlayerStats.Instance.attackCooldown = Mathf.Max(0.1f, PlayerStats.Instance.attackCooldown - value);
                break;
            case UpgradeType.MoveSpeed:
                PlayerStats.Instance.moveSpeed += value;
                break;
            case UpgradeType.MaxHP:
                PlayerStats.Instance.UpgradeMaxHP(value);
                break;
            case UpgradeType.ProjectileCount:
                PlayerStats.Instance.projectileCount += Mathf.RoundToInt(value);
                break;
        }
    }
}

public enum UpgradeType
{
    Damage,
    AttackSpeed,
    MoveSpeed,
    MaxHP,
    ProjectileCount
}