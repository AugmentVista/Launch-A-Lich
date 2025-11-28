using UnityEngine;

public class EnemyCountTracker : MonoBehaviour
{
    public static int EnemyCount = 0;

    [SerializeField] private bool increments = true;

    private void OnEnable()
    {
        PlayerStateMachine.OnStopped += ResetCount;
    }

    private void OnDisable()
    {
        PlayerStateMachine.OnStopped -= ResetCount;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Enemy")) return;

        if (increments)
            EnemyCount++;
        else
            EnemyCount--;
    }

    public void ResetCount()
    {
        EnemyCount = 0;
    }
}
