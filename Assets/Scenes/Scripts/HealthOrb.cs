using UnityEngine;

public class HealthOrb : MonoBehaviour
{
    [SerializeField] float healAmount = 20f;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        PlayerStats.Instance.Heal(healAmount);
        HealthOrbPool.Instance.Return(this);
    }
}