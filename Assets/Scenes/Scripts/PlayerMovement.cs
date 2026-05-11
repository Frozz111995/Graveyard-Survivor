using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    private IInputProvider input;
    private float _velocityY = 0f;

    public void Init(IInputProvider inputProvider)
    {
        input = inputProvider;
    }

    void Update()
    {
        Vector2 move = input.GetMove();

        if (controller.isGrounded)
            _velocityY = 0f;
        else
            _velocityY += Physics.gravity.y * Time.deltaTime;

        Vector3 dir = new Vector3(move.x, 0, move.y) * 5f * Time.deltaTime;
        dir.y = _velocityY * Time.deltaTime;
        controller.Move(dir);
    }
}
