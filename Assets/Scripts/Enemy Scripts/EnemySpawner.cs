using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    [SerializeField] GameObject GroundEnemy;
    [SerializeField] GameObject FlyingEnemy;

    [SerializeField] SpeedLimit speedLimit;
    private float speedLimitX;
    private float spawnInterval = 0.25f;
    public float spawnY;
    public float spawnOffset = 2f;  // how far past right edge to spawn

    [SerializeField] private float minSpawnRate = 0.5f;
    [SerializeField] private float maxSpawnRate = 0.15f;

    private Camera cam;
    private bool canSpawn = false;
    private float timer = 0f;

    void Awake()
    {
        cam = Camera.main;
    }

    void OnEnable()
    {
        PlayerStateMachine.OnGrounded += EnableSpawning;
        PlayerStateMachine.OnFlying += EnableSpawning;
        PlayerStateMachine.OnGrounded += SpawnGroundedEnemies;
        PlayerStateMachine.OnFlying += SpawnFlyingEnemies;


        PlayerStateMachine.OnInactive += DisableSpawning;
        PlayerStateMachine.OnStopped += DisableSpawning;

        PlayerStateMachine.OnReadyToLaunch += OnPlayerReadyToLaunch;
    }

    void OnDisable()
    {
        PlayerStateMachine.OnGrounded -= EnableSpawning;
        PlayerStateMachine.OnFlying -= EnableSpawning;
        PlayerStateMachine.OnGrounded -= SpawnGroundedEnemies;
        PlayerStateMachine.OnFlying -= SpawnFlyingEnemies;

        PlayerStateMachine.OnInactive -= DisableSpawning;
        PlayerStateMachine.OnStopped -= DisableSpawning;

        PlayerStateMachine.OnReadyToLaunch -= OnPlayerReadyToLaunch;
    }

    void Update()
    {
        if (canSpawn)
        {
            speedLimitX = speedLimit.maxSpeedX;
            float speed = Mathf.Abs(PlayerResultsManager.globalPlayerSpeedX);

            float speedDeterminedSpawnRange = Mathf.InverseLerp(20f, speedLimitX, speed);
            float scaledInterval = Mathf.Lerp(0.1f, 1.0f, speedDeterminedSpawnRange);

            spawnInterval = Mathf.Clamp((float)Mathf.Round(scaledInterval * 10f) / 10f, maxSpawnRate, minSpawnRate);

            if (PlayerResultsManager.globalPlayerSpeedY > 10 || PlayerResultsManager.globalPlayerSpeedY < -10f)
            {
                SpawnFlyingEnemies();
            }
            else { SpawnGroundedEnemies(); }

            timer += Time.deltaTime;

            if (timer >= spawnInterval)
            {
                SpawnEnemy();
                timer = 0f;
            }
        }
    }

    void SpawnFlyingEnemies()
    {
        enemyPrefab = FlyingEnemy;

        Vector3 bottomEdge = cam.ViewportToWorldPoint(new Vector3(0.5f, 0f, cam.nearClipPlane));
        Vector3 topEdge = cam.ViewportToWorldPoint(new Vector3(0.5f, 1f, cam.nearClipPlane));

        float randomY = Random.Range(bottomEdge.y + 10f, topEdge.y - 1f);

        spawnY = Mathf.Clamp(randomY, 10f, float.MaxValue);
    }

    void SpawnGroundedEnemies()
    {
        enemyPrefab = GroundEnemy;

        spawnY = 0f;
    }


    private void SpawnEnemy()
    {
        if (enemyPrefab == null) return;

        CapsuleCollider2D prefabCollider = enemyPrefab.GetComponent<CapsuleCollider2D>();
        if (prefabCollider == null)
        {
            Debug.LogError("Enemy prefab does not have a BoxCollider2D attached.");
            return;
        }

        Vector2 colliderSize = prefabCollider.size;
        Vector2 overlapBoxSize = colliderSize * 2f; // Double the size for spacing

        Vector2 rightEdge = cam.ViewportToWorldPoint(new Vector3(1, 0.5f, 0));

        const int maxAttempts = 10;
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            float randomSpawnOffset = Random.Range(0.25f, 0.75f);
            Vector2 spawnPos = new Vector2(rightEdge.x + randomSpawnOffset, spawnY);

            Collider2D hit = Physics2D.OverlapBox(spawnPos, overlapBoxSize, 0f, LayerMask.GetMask("Enemies"));

            if (hit == null || !hit.CompareTag("Enemy") && !hit.CompareTag("Item"))
            {
                GameObject instance = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

                if (enemyPrefab == GroundEnemy)
                {
                    Vector3 scale = instance.transform.localScale;
                    scale.x *= -1;
                    instance.transform.localScale = scale;
                }
                return;
            }
        }

        Debug.LogWarning("EnemySpawner: Could not find valid spawn position after multiple attempts.");
    }



    private void EnableSpawning() => canSpawn = true;
    private void DisableSpawning() => canSpawn = false;

    private void OnPlayerReadyToLaunch()
    {
        DisableSpawning();
    }

}