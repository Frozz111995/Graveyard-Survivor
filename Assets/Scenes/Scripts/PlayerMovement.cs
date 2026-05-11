using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    private IInputProvider input;

    public void Init(IInputProvider inputProvider)
    {
        input = inputProvider;
    }

    void Update()
    {
        Vector2 move = input.GetMove();

        Vector3 dir = new Vector3(move.x, 0, move.y);
        controller.Move(dir * 5f * Time.deltaTime);
    }
}