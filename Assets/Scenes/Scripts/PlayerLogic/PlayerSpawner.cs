using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;
    public CameraFollow cameraFollow;
    public EnemySpawner enemySpawner;
    [SerializeField] PropSpawnSystem propSpawnSystem;
    public Transform spawnPoint;
    void Start()
    {
        var playerObj = Instantiate(playerPrefab, spawnPoint.position, Quaternion.Euler(0f, 180f, 0f));
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