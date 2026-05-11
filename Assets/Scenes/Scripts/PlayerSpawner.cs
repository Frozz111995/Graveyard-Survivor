using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;
    public CameraFollow cameraFollow;
    public EnemySpawner enemySpawner;

    void Start()
    {
        var playerObj = Instantiate(playerPrefab);

        var player = playerObj.GetComponentInChildren<PlayerMovement>();
        player.Init(GameBootstrap.Input);

        var playerTransform = playerObj.transform;

        // камера
        cameraFollow.SetTarget(playerTransform);

        // враги теперь знают игрока
        enemySpawner.SetPlayer(playerTransform);
    }
}