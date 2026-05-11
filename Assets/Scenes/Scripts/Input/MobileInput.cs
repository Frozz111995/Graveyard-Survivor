using UnityEngine;

public class MobileInput : MonoBehaviour, IInputProvider
{
    public Vector2 GetMove()
    {
        // сюда позже подключишь джойстик
        return Vector2.zero;
    }
}