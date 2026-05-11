using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    public static IInputProvider Input;

    public DesktopInput desktop;
    public MobileInput mobile;
    public WebInput web;

    void Awake()
    {
#if UNITY_ANDROID || UNITY_IOS
        Input = Instantiate(mobile);
#elif UNITY_WEBGL
        Input = Instantiate(web);
#else
        Input = Instantiate(desktop);
#endif
    }
}