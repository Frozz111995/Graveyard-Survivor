// EnemyConfig.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Game/EnemyConfig")]
public class EnemyConfig : ScriptableObject
{
    [Header("Stats")]
    public float maxHP = 100f;
    public float moveSpeed = 3f;
    public float contactDamage = 10f;
    public float damageCooldown = 1f;

    [Header("Progression")]
    [Tooltip("Через сколько секунд этот тип начинает спавниться")]
    public float unlockAfterSeconds = 0f;
    
    [Header("Spawn Weight")]
    [Tooltip("Чем больше — тем чаще спавнится")]
    public float spawnWeight = 1f;

    [Header("Prefab")]
    public EnemyAI prefab;
    public int initialPoolSize = 20;
    
    [Header("Elite")]
    public float eliteUnlockDelay = 300f; // 5 минут после unlockAfterSeconds
    public bool canSpawnElite = true;
    public float eliteChanceBase = 0f;     // шанс до элитки (0 = недоступна)
    public float eliteChancePerMinute = 0.05f; // +5% каждую минуту после анлока
    public float eliteChanceMax = 0.4f;    // максимум 40%

    [Header("Elite Overrides")]
    public float eliteHpMult = 3f;
    public float eliteSpeedMult = 1.5f;
    public float eliteSizeMult = 1.4f;
    public Material eliteMaterial;         // фиолетовый/чёрный материал
    public GameObject eliteOnDeathFx;      // взрыв при смерти
}