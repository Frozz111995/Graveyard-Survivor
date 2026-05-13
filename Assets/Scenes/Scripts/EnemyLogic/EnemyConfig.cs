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

    [Header("Spawn Weight")]
    [Tooltip("Чем больше — тем чаще спавнится")]
    public float spawnWeight = 1f;

    [Header("Prefab")]
    public EnemyAI prefab;
    public int initialPoolSize = 20; 
}