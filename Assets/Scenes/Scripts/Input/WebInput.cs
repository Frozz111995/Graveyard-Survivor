// WebInput.cs
using UnityEngine;

public class WebInput : MonoBehaviour, IInputProvider
{
    DragInput _drag;

    void Awake()
    {
        _drag = DragInput.Create();
    }

    public Vector2 GetMove()
    {
        Vector2 keyboard = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        if (keyboard.sqrMagnitude > 0.01f)
            return keyboard.normalized;

        return _drag.GetMove();
    }
}