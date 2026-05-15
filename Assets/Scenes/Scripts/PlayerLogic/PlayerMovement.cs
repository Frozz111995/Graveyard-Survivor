using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    private IInputProvider input;
    private float _velocityY = 0f;
    private PlayerVisuals _visuals;
    private Transform _root;
    private float _rotationLockTimer = 0f;

    public void Init(IInputProvider inputProvider)
    {
        input = inputProvider;
        _visuals = transform.root.GetComponentInChildren<PlayerVisuals>();
        _root = _visuals.transform;
        PlayerStats.Instance.OnDeath += HandleDeath;
    }

    void OnDestroy()
    {
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.OnDeath -= HandleDeath;
    }

    void HandleDeath()
    {
        enabled = false;
    }

    public void LockRotation(float duration)
    {
        _rotationLockTimer = duration;
    }

    void Update()
    {
        _rotationLockTimer -= Time.deltaTime;

        Vector2 move = input.GetMove();

        if (controller.isGrounded)
            _velocityY = 0f;
        else
            _velocityY += Physics.gravity.y * Time.deltaTime;

        Vector3 dir = new Vector3(move.x, 0, move.y);

        _visuals?.SetRun(dir.magnitude > 0.1f);

        if (dir.sqrMagnitude > 0.01f && _rotationLockTimer <= 0f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir.normalized);
            _root.rotation = Quaternion.Slerp(_root.rotation, targetRot, 20f * Time.deltaTime);
        }

        if (dir.magnitude > 1f)
            dir.Normalize();

        dir *= PlayerStats.Instance.moveSpeed * Time.deltaTime;
        dir.y = _velocityY * Time.deltaTime;
        controller.Move(dir);
    }
}