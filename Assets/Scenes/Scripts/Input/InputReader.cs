using UnityEngine;

public class InputReader : MonoBehaviour
{
    public Vector2 GetMove()
    {
#if UNITY_ANDROID || UNITY_IOS
        return GetMobile();
#else
        return GetDesktop();
#endif
    }

    Vector2 GetDesktop()
    {
        return new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );
    }

    Vector2 GetMobile()
    {
        return Vector2.zero; // позже джойстик
    }
}