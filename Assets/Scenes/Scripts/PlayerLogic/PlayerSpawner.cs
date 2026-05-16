using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;
    public CameraFollow cameraFollow;
    public EnemySpawner enemySpawner;
    [SerializeField] PropSpawnSystem propSpawnSystem;
    void Start()
    {
        var playerObj = Instantiate(playerPrefab);
        propSpawnSystem.Initialize(playerObj.transform);
        var player = playerObj.GetComponentInChildren<PlayerMovement>();
        player.Init(GameBootstrap.Input);

        var playerTransform = playerObj.transform;

        // камера
        cameraFollow.SetTarget(playerTransform);

        // враги теперь знают игрока
        enemySpawner.SetPlayer(playerTransform);
    }
}