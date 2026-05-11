using UnityEngine;

public class WebInput : MonoBehaviour, IInputProvider
{
    public Vector2 GetMove()
    {
        return new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );
    }
}