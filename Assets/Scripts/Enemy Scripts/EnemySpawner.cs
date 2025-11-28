using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    private EnemyPlacement placement;       

    [SerializeField] private GameObject[] flyingEnemies;

    [SerializeField] private SpeedLimit speedLimit;

    [SerializeField] private float minSpawnRate = 2f;
    [SerializeField] private float maxSpawnRate = 5.0f;
    [SerializeField] private int maxEnemyAmount;

    private bool canSpawn = false;
    private float timer = 0f;
    private float spawnInterval;

    void Awake()
    {
        placement = GetComponentInChildren<EnemyPlacement>();
    }

    void OnEnable()
    {
        PlayerStateMachine.OnFlying += EnableSpawning;
        PlayerStateMachine.OnInactive += DisableSpawning;
        PlayerStateMachine.OnStopped += DisableSpawning;
    }

    void OnDisable()
    {
        PlayerStateMachine.OnFlying -= EnableSpawning;
        PlayerStateMachine.OnInactive -= DisableSpawning;
        PlayerStateMachine.OnStopped -= DisableSpawning;
    }

    void Update()
    {
        if (!canSpawn) { return; }

        float playerSpeedX = Mathf.Abs(PlayerResultsManager.globalPlayerSpeedX);
        float playerSpeedY = Mathf.Abs(PlayerResultsManager.globalPlayerSpeedY);
        float playerSpeedFloor = 20f;

        float effectiveSpeed = (playerSpeedX * 2f + playerSpeedY) / 3f;

        float playerSpeedNormalized = Mathf.InverseLerp(playerSpeedFloor, speedLimit.maxSpeedX, effectiveSpeed);

        // get enemies per second
        float spawnRate = Mathf.Lerp(minSpawnRate, maxSpawnRate, playerSpeedNormalized);

        // Round to whole numbers
        spawnRate = Mathf.Round(spawnRate);

        // Prevent invalid spawn rates
        if (spawnRate < 1f) { spawnRate = 1f; }

        // convert enemies/sec -> seconds/enemy
        spawnInterval = 1f / spawnRate;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    private void SpawnEnemy()
    {
        if (EnemyCountTracker.EnemyCount >= maxEnemyAmount) { return; }

        DetermineFlyingPrefab();
        if (enemyPrefab == null) return;

        Transform pod = placement.GetNextPod();
        if (pod == null) return;

        // Spawn as a child of that pod 
        GameObject instance = Instantiate(enemyPrefab, pod.position, Quaternion.identity, pod);
    }

    private void DetermineFlyingPrefab()
    {
        float dist = PlayerResultsManager.currentDistance;

        if (dist < 1000f) enemyPrefab = flyingEnemies[0];
        else if (dist < 3000f) enemyPrefab = flyingEnemies[1];
        else { enemyPrefab = flyingEnemies[2]; }
    }

    // Called from EnemyPlacement when it retries after pod cooldown
    public void SpawnUsingPod(Transform pod)
    {
        if (enemyPrefab == null) return;
        Instantiate(enemyPrefab, pod.position, Quaternion.identity, pod);
    }

    private void EnableSpawning() => canSpawn = true;
    private void DisableSpawning() => canSpawn = false;
}
