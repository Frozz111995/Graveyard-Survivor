using UnityEngine;

public class HealthOrb : MonoBehaviour
{
    [SerializeField] float healAmount = 20f;
    [SerializeField] Color orbColor = new Color(0f, 1f, 0.3f);
    [SerializeField] AudioClip collectSound;
    Renderer _renderer;
    MaterialPropertyBlock _propBlock;
    OrbFloat _orbFloat;

    void Awake()
    {
        _renderer = GetComponentInChildren<Renderer>();
        _propBlock = new MaterialPropertyBlock();
        _orbFloat = GetComponent<OrbFloat>();

        _renderer.GetPropertyBlock(_propBlock);
        _propBlock.SetColor("_BaseColor", orbColor);
        _propBlock.SetColor("_EmissionColor", orbColor * 2f);
        _renderer.SetPropertyBlock(_propBlock);

        _orbFloat.SetEmissionColor(orbColor * 2f);
    } 

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        AudioPool.Instance.Play(collectSound, transform.position, volume: 0.3f, pitch: Random.Range(0.5f, 1.2f));
        PlayerStats.Instance.Heal(healAmount);
        HealthOrbPool.Instance.Return(this);
    }
}