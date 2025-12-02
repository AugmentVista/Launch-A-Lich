using UnityEngine;

public class EnemyCountTracker : MonoBehaviour
{
    public int enemyCount = 0;

    public GameObject CheckIn;
    public GameObject CheckOut;

    private void OnEnable()
    {
        PlayerInteractionHandler.OnFlyingEnemyDefeated += LogEnemyKilled;
        PlayerStateMachine.OnStopped += ResetCount;
    }

    private void OnDisable()
    {
        PlayerInteractionHandler.OnFlyingEnemyDefeated -= LogEnemyKilled;
        PlayerStateMachine.OnStopped -= ResetCount;
    }

    public void LogEnemyKilled(int amount)
    {
        enemyCount--;
    }

    public void ResetCount()
    {
        enemyCount = 0;
    }
}
