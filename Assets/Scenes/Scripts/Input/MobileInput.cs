// MobileInput.cs
using UnityEngine;

public class MobileInput : MonoBehaviour, IInputProvider
{
    DragInput _drag;

    void Awake()
    {
        _drag = DragInput.Create();
    }

    public Vector2 GetMove() => _drag.GetMove();
}