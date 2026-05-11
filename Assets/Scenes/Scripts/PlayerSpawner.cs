using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;

    void Start()
    {
        var playerObj = Instantiate(playerPrefab);

        var player = playerObj.GetComponentInChildren<PlayerMovement>();
        player.Init(GameBootstrap.Input);
    }
}