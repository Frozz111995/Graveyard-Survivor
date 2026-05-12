// PlayerStats.cs
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [Header("Attack")]
    public float damage = 10f;
    public float attackCooldown = 1f;
    public float burstInterval = 0.1f;
    public int projectileCount = 1;

    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Health")]
    public float maxHP = 100f;

    void Awake()
    {
        Instance = this;
    }
}