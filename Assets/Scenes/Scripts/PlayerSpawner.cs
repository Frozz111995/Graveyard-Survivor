using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;
    public CameraFollow cameraFollow;

    void Start()
    {
        var playerObj = Instantiate(playerPrefab);

        var player = playerObj.GetComponentInChildren<PlayerMovement>();
        player.Init(GameBootstrap.Input);

        cameraFollow.SetTarget(playerObj.transform);
    }
}