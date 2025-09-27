using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Reward Settings")]
    public int moneyPerEnemy = 5;

    private int enemiesDefeated = 0;

    /// <summary>
    /// Called by enemies when they are destroyed by the player.
    /// </summary>
    public void EnemyDefeated()
    {
        enemiesDefeated++;
        Debug.Log($"Enemy defeated! Total defeated this run: {enemiesDefeated}");
    }

    /// <summary>
    /// Returns the amount of money earned this run.
    /// </summary>
    public int GetRunReward()
    {
        return enemiesDefeated * moneyPerEnemy;
    }

    /// <summary>
    /// Reset counters at the start of a new run.
    /// </summary>
    public void ResetRun()
    {
        enemiesDefeated = 0;
        Debug.Log("LevelManager reset for new run.");
    }

    /// <summary>
    /// Optionally expose enemy count for UI or other systems.
    /// </summary>
    public int GetDefeatedCount()
    {
        return enemiesDefeated;
    }
}
