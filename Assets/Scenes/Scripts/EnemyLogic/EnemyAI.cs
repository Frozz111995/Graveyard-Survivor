using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 3f;

    private Transform player;
    private CharacterController _cc;
    private float _velocityY = 0f;

    public void Init(Transform playerTransform)
    {
        player = playerTransform;
        _cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f;

        if (_cc.isGrounded)
            _velocityY = 0f;
        else
            _velocityY += Physics.gravity.y * Time.deltaTime;

        Vector3 move = direction * moveSpeed * Time.deltaTime;
        move.y = _velocityY * Time.deltaTime;

        _cc.Move(move);

        if (direction != Vector3.zero)
            transform.forward = direction;
    }

    public void Teleport(Vector3 position)
    {
        _cc.enabled = false;
        transform.position = position;
        _cc.enabled = true;
        _velocityY = 0f;
    }
}