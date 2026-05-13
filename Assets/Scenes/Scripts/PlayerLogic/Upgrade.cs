// Upgrade.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Upgrade")]
public class Upgrade : ScriptableObject
{
    public string upgradeName => type switch
    {
        UpgradeType.AttackSpeed => "Attack Speed",
        UpgradeType.MaxHP => "Max HP",
        UpgradeType.ProjectileCount => "Projectile Count",
        _ => type.ToString()
    };
    [TextArea] public string description;
    public Sprite icon;
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