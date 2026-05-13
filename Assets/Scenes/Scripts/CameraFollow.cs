// CameraFollow.cs
using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance { get; private set; }

    private Transform player;
    public Vector3 offset;
    public float smoothSpeed = 5f;

    Vector3 _shakeOffset;

    void Awake()
    {
        Instance = this;
    }

    public void SetTarget(Transform target)
    {
        player = target;
    }

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 targetPos = player.position + offset + _shakeOffset;
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
    }

    public void Shake(float duration, float magnitude)
    {
        StopAllCoroutines();
        StartCoroutine(DoShake(duration, magnitude));
    }

    IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float strength = Mathf.Lerp(magnitude, 0f, elapsed / duration); // затухает
            _shakeOffset = Random.insideUnitSphere * strength;
            _shakeOffset.z = 0f;

            elapsed += Time.deltaTime;
            yield return null;
        }

        _shakeOffset = Vector3.zero;
    }
}