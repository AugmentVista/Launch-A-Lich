using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] PlayerResultsManager results;

    public GameObject itemPrefab;
    [SerializeField] GameObject[] flyingItems;

    [SerializeField] SpeedLimit speedLimit;
    [SerializeField] GameObject ceiling;

    private float speedLimitX;
    private float spawnInterval = 0.25f;
    public float spawnY;

    [SerializeField] private float minSpawnRate = 1.5f;
    [SerializeField] private float maxSpawnRate = 0.5f;

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
        PlayerStateMachine.OnFlying += SpawnFlyingItems;

        PlayerStateMachine.OnInactive += DisableSpawning;
        PlayerStateMachine.OnStopped += DisableSpawning;
    }

    void OnDisable()
    {
        PlayerStateMachine.OnGrounded -= EnableSpawning;
        PlayerStateMachine.OnFlying -= EnableSpawning;
        PlayerStateMachine.OnFlying -= SpawnFlyingItems;

        PlayerStateMachine.OnInactive -= DisableSpawning;
        PlayerStateMachine.OnStopped -= DisableSpawning;
    }

    void Update()
    {
        if (!canSpawn) { return; }

        speedLimitX = speedLimit.maxSpeedX;

        float x = Mathf.Abs(PlayerResultsManager.globalPlayerSpeedX);
        float y = Mathf.Abs(PlayerResultsManager.globalPlayerSpeedY);

        // Weighted 1:2 ratio Y:X
        float effectiveSpeed = (x * 2f + y * 1f) / 3f;

        // Scale the spawn interval effectiveSpeed
        float speedDeterminedSpawnRange = Mathf.InverseLerp(20f, speedLimitX, effectiveSpeed);

        float scaledInterval = Mathf.Lerp(0.1f, 1.0f, speedDeterminedSpawnRange);

        spawnInterval = Mathf.Clamp(Mathf.Round(scaledInterval * 10f) / 10f,maxSpawnRate, minSpawnRate);

        if (PlayerResultsManager.currentHeight >= 10)
        {
            SpawnFlyingItems();
        }

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnItem();
            timer = 0f;
        }
    }

    void SpawnFlyingItems()
    {
        float dist = PlayerResultsManager.currentDistance;

        switch (dist)
        {
            case float distance when (distance < 500f):
                itemPrefab = flyingItems[0];
                break;
            case float distance when (distance < 1500f):
                itemPrefab = flyingItems[1];
                break;
            case float distance when (distance <= 3000f):
                itemPrefab = flyingItems[2];
                break;
            case float distance when (distance <= 4000f):
                itemPrefab = flyingItems[3];
                break;
            case float distance when (distance <= 5000f):
                itemPrefab = flyingItems[4];
                break;
            case float distance when (distance <= 6000f):
                itemPrefab = flyingItems[5];
                break;
            case float distance when (distance <= 7000f):
                itemPrefab = flyingItems[6];
                break;
            case float distance when (distance <= 8000f):
                itemPrefab = flyingItems[7];
                break;
            case float distance when (distance <= 9000f):
                itemPrefab = flyingItems[8];
                break;
            default:
                int randomTreat = Random.Range(0, 9);
                itemPrefab = flyingItems[randomTreat];
            break;
        }
        Vector3 bottomEdge = cam.ViewportToWorldPoint(new Vector3(0.5f, 0f, cam.nearClipPlane));
        Vector3 topEdge = cam.ViewportToWorldPoint(new Vector3(0.5f, 1f, cam.nearClipPlane));

        float randomY = Random.Range(bottomEdge.y + 10f, topEdge.y - 1f);

        spawnY = Mathf.Clamp(randomY, 10f, ceiling.transform.position.y - 8);
    }


    private void SpawnItem()
    {
        if (itemPrefab == null) return;

        CapsuleCollider2D prefabCollider = itemPrefab.GetComponent<CapsuleCollider2D>();
        if (prefabCollider == null)
        {
            return;
        }

        Vector2 colliderSize = prefabCollider.size;
        Vector2 overlapBoxSize = colliderSize * 4f; // Double the size for spacing

        Vector2 rightEdge = cam.ViewportToWorldPoint(new Vector3(1, 0.5f, 0));

        const int maxAttempts = 10;
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            float randomSpawnOffset = Random.Range(4f, 15f);
            Vector2 spawnPos = new Vector2(rightEdge.x + randomSpawnOffset, spawnY);

            Collider2D hit = Physics2D.OverlapBox(spawnPos, overlapBoxSize, 0f, LayerMask.GetMask("Enemies", "Item"));

            if (hit == null || !hit.CompareTag("Enemy") && !hit.CompareTag("Item") && !hit.CompareTag("Ground") && !hit.CompareTag("Ceiling"))
            {
                GameObject instance = Instantiate(itemPrefab, spawnPos, Quaternion.identity);

                return;
            }
        }
    }

    private void EnableSpawning() => canSpawn = true;
    private void DisableSpawning() => canSpawn = false;

}