// PlayerPositionCache.cs
using UnityEngine;

public class PlayerPositionCache : MonoBehaviour
{
    public static Vector3 Position { get; private set; }
    public static bool Exists { get; private set; }

    void Awake()
    {
        Exists = true;
    }

    void OnDestroy()
    {
        Exists = false;
    }

    void Update()
    {
        Position = transform.position;
    }
    
}