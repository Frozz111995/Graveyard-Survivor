using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform player;
    public Vector3 offset;
    public float smoothSpeed = 5f;

    public void SetTarget(Transform target)
    {
        player = target;
    }

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 targetPos = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
    }
}