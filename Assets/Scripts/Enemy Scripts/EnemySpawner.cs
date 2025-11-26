using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    [SerializeField] GameObject[] groundEnemies;
    [SerializeField] GameObject[] flyingEnemies;

    [SerializeField] SpeedLimit speedLimit;
    [SerializeField] GameObject ceiling;

    private float speedLimitX;
    private float spawnInterval = 0.25f;
    private float spawnY;

    [SerializeField] float minSpawnRate = 0.5f;
    [SerializeField] float maxSpawnRate = 0.15f;

    private Camera cam;
    private bool canSpawn = false;
    private float timer = 0f;

    void Awake() { cam = Camera.main; }

    void OnEnable()
    {
        PlayerStateMachine.OnGrounded += EnableSpawning;
        PlayerStateMachine.OnFlying += EnableSpawning;
        PlayerStateMachine.OnGrounded += SpawnGroundedEnemies;
        PlayerStateMachine.OnFlying += SpawnFlyingEnemies;

        PlayerStateMachine.OnInactive += DisableSpawning;
        PlayerStateMachine.OnStopped += DisableSpawning;
    }

    void OnDisable()
    {
        PlayerStateMachine.OnGrounded -= EnableSpawning;
        PlayerStateMachine.OnFlying -= EnableSpawning;
        PlayerStateMachine.OnGrounded -= SpawnGroundedEnemies;
        PlayerStateMachine.OnFlying -= SpawnFlyingEnemies;

        PlayerStateMachine.OnInactive -= DisableSpawning;
        PlayerStateMachine.OnStopped -= DisableSpawning;
    }

    void Update()
    {
        if (!canSpawn) return;

        speedLimitX = speedLimit.maxSpeedX;

        float x = Mathf.Abs(PlayerResultsManager.globalPlayerSpeedX);
        float y = Mathf.Abs(PlayerResultsManager.globalPlayerSpeedY);

        float effectiveSpeed = (x * 2f + y * 1f) / 3f;

        float speedDeterminedSpawnRange = Mathf.InverseLerp(20f, speedLimitX, effectiveSpeed);
        float scaledInterval = Mathf.Lerp(0.1f, 1.0f, speedDeterminedSpawnRange);

        spawnInterval = Mathf.Clamp(Mathf.Round(scaledInterval * 10f) / 10f, maxSpawnRate, minSpawnRate);

        if (PlayerResultsManager.globalPlayerSpeedY > 10 || PlayerResultsManager.globalPlayerSpeedY < -10f)
        {
            SpawnFlyingEnemies();
        }
        else
        {
            SpawnGroundedEnemies();
        }


        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    void SpawnFlyingEnemies()
    {
        float dist = PlayerResultsManager.currentDistance;

        if (dist < 1000f) enemyPrefab = flyingEnemies[0];
        else if (dist < 3000f) enemyPrefab = flyingEnemies[1];
        else enemyPrefab = flyingEnemies[2];

        Vector3 bottom = cam.ViewportToWorldPoint(new Vector3(0.5f, 0f, 0));
        Vector3 top = cam.ViewportToWorldPoint(new Vector3(0.5f, 1f, 0));

        float randomY = Random.Range(bottom.y + 5f, top.y - 5f);
        spawnY = randomY;
    }

    void SpawnGroundedEnemies()
    {
        float dist = PlayerResultsManager.currentDistance;

        if (dist < 1500f) enemyPrefab = groundEnemies[0];
        else if (dist < 3000f) enemyPrefab = groundEnemies[1];
        else enemyPrefab = groundEnemies[2];

        spawnY = 0f;
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null) return;

        CapsuleCollider2D prefabCollider = enemyPrefab.GetComponent<CapsuleCollider2D>();
        if (prefabCollider == null) return;

        Vector2 overlapSize = prefabCollider.size * 4f;
        Vector2 rightEdge = cam.ViewportToWorldPoint(new Vector3(1, 0.5f, 0));

        const int maxAttempts = 10;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            float playerX = PlayerResultsManager.currentDistance;
            float roundedPlayerSpeed = Mathf.RoundToInt(PlayerResultsManager.globalPlayerSpeedX /10f);

            float minLead = 5f;
            float maxLead = Mathf.Max(10f, roundedPlayerSpeed);

            float lead = Random.Range(minLead, maxLead);

            Vector2 spawnPos = new Vector2(playerX + lead, spawnY);

            Collider2D hit = Physics2D.OverlapBox(spawnPos, overlapSize, 0f, LayerMask.GetMask("Enemies", "Item"));

            if (hit == null)
            {
                GameObject instance = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

                for (int i = 0; i < groundEnemies.Length; i++)
                {
                    if (enemyPrefab == groundEnemies[i])
                    {
                        Vector3 scale = instance.transform.localScale;
                        scale.x *= -1;
                        instance.transform.localScale = scale;
                    }
                }
                return;
            }
        }
    }

    private void EnableSpawning() => canSpawn = true;
    private void DisableSpawning() => canSpawn = false;
}
