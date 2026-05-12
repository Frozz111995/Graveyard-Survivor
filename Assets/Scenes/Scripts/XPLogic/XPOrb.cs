// XPOrb.cs
using UnityEngine;

public class XPOrb : MonoBehaviour
{
    [SerializeField] float xpAmount = 10f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            Collect();
    }

    public void Collect()
    {
        XPSystem.Instance.AddXP(xpAmount);
        XPOrbPool.Instance.Return(this);
    }
}