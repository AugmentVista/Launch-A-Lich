using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    private float spawnInterval = 2f;
    public float spawnY = 0f;
    public float spawnOffset = 2f;  // how far past right edge to spawn

    private Camera cam;
    private LevelManager levelManager;
    private bool canSpawn = false;
    private float timer = 0f;

    void Awake()
    {
        cam = Camera.main;
        levelManager = GetComponentInParent<LevelManager>();
    }

    void OnEnable()
    {
        PlayerStateMachine.OnGrounded += EnableSpawning;
        PlayerStateMachine.OnFlying += EnableSpawning;

        PlayerStateMachine.OnFlying += SpawnFlyingEnemies;

        PlayerStateMachine.OnInactive += DisableSpawning;
        PlayerStateMachine.OnStopped += DisableSpawning;

        PlayerStateMachine.OnReadyToLaunch += OnPlayerReadyToLaunch;
    }

    void OnDisable()
    {
        PlayerStateMachine.OnGrounded -= EnableSpawning;
        PlayerStateMachine.OnFlying -= EnableSpawning;

        PlayerStateMachine.OnFlying -= SpawnFlyingEnemies;

        PlayerStateMachine.OnInactive -= DisableSpawning;
        PlayerStateMachine.OnStopped -= DisableSpawning;

        PlayerStateMachine.OnReadyToLaunch -= OnPlayerReadyToLaunch;
    }

    void Update()
    {
        if (canSpawn)
        {
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                SpawnEnemy();
                timer = 0f;
            }
        }
    }

    void SpawnFlyingEnemies() { }

    void SpawnEnemy()
    {
        if (enemyPrefab == null) return;

        // Get the size of the BoxCollider2D on the prefab
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

            if (hit == null || !hit.CompareTag("Enemy"))
            {
                Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
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