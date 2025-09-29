using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;
    public float spawnInterval = 2f;
    public float spawnY = 0f;
    public float spawnOffset = 2f;  // how far past right edge to spawn

    private Camera cam;
    private LevelManager levelManager;
    private bool canSpawn = false;
    private float timer = 0f;

    // Track all spawned enemies
    private List<Enemy> activeEnemies = new List<Enemy>();

    void Awake()
    {
        cam = Camera.main;
        levelManager = GetComponentInParent<LevelManager>();
    }

    void OnEnable()
    {
        PlayerStateMachine.OnRolling += EnableSpawning;
        PlayerStateMachine.OnFlying += EnableSpawning;

        PlayerStateMachine.OnInactive += DisableSpawning;
        PlayerStateMachine.OnStopped += DisableSpawning;

        PlayerStateMachine.OnReadyToLaunch += OnPlayerReadyToLaunch;
    }

    void OnDisable()
    {
        PlayerStateMachine.OnRolling -= EnableSpawning;
        PlayerStateMachine.OnFlying -= EnableSpawning;

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

        // Check for off-screen enemies and despawn
        //for (int i = activeEnemies.Count - 1; i >= 0; i--)
        //{
        //    if (activeEnemies[i] != null && ShouldDespawn(activeEnemies[i].transform.position))
        //    {
        //        Destroy(activeEnemies[i].gameObject);
        //        activeEnemies.RemoveAt(i);
        //    }
        //}
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null) return;

        Vector2 rightEdge = cam.ViewportToWorldPoint(new Vector3(1, 0.5f, 0));
        Vector2 spawnPos = new Vector2(rightEdge.x + spawnOffset, spawnY);

        GameObject enemyObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        Enemy enemy = enemyObj.GetComponent<Enemy>();
        if (enemy != null)
        {
            activeEnemies.Add(enemy);
        }
    }

    public bool ShouldDespawn(Vector2 position)
    {
        Vector2 leftEdge = cam.ViewportToWorldPoint(new Vector3(0, 0.5f, 0));
        return position.x <= leftEdge.x - 0.5f; // just outside camera
    }

    private void EnableSpawning() => canSpawn = true;
    private void DisableSpawning() => canSpawn = false;

    private void OnPlayerReadyToLaunch()
    {
        DisableSpawning();

        // Destroy all active enemies
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            if (activeEnemies[i] != null)
                Destroy(activeEnemies[i].gameObject);
        }

        activeEnemies.Clear();
    }

    // Called by Enemy when it dies
    public void NotifyEnemyDestroyed(Enemy enemy)
    {
        if (activeEnemies.Contains(enemy))
            activeEnemies.Remove(enemy);
    }
}