using UnityEngine;

public class DestroyEnemy : MonoBehaviour
{
    [SerializeField] LevelManager levelManager;
  
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Tell LevelManager that enemy was defeated
            if (levelManager != null)
                levelManager.EnemyDefeated();

            // Destroy enemy immediately
            Destroy(other.gameObject);
        }
    }
}
