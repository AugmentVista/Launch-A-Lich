using UnityEngine;

public class EnemyCheckInOut : MonoBehaviour
{
    public bool increments;
    public EnemyCountTracker countTracker;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;
        Enemy enemy = collision.GetComponent<Enemy>();
        string enemyID = enemy.ID;

        if (increments) 
        {
            Debug.Log($"IN {enemyID}");
            countTracker.enemyCount++; 
        }
        else 
        {
            if (enemy.isDead) { return; }

            Debug.LogWarning($"OUT {enemyID}");
            countTracker.enemyCount--; 
        }
    }
}