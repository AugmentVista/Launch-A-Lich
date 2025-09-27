using UnityEngine;

public class EnemyClear : MonoBehaviour
{
    private BoxCollider2D enemyClearCollider;

    private void Awake()
    {
        enemyClearCollider = GetComponent<BoxCollider2D>();
        enemyClearCollider.enabled = false;
    }

    private void OnEnable()
    {
        PlayerStateMachine.OnReadyToLaunch += ClearEnemies;
        PlayerStateMachine.OnRolling += DisableCollider;
        PlayerStateMachine.OnFlying += DisableCollider;
        PlayerStateMachine.OnInactive += DisableCollider;
        PlayerStateMachine.OnStopped += DisableCollider;
    }

    private void OnDisable()
    {
        PlayerStateMachine.OnReadyToLaunch -= ClearEnemies;
        PlayerStateMachine.OnRolling -= DisableCollider;
        PlayerStateMachine.OnFlying -= DisableCollider;
        PlayerStateMachine.OnInactive -= DisableCollider;
        PlayerStateMachine.OnStopped -= DisableCollider;
    }

    private void ClearEnemies()
    {
        enemyClearCollider.enabled = true;

        // Get all colliders overlapping this BoxCollider
        Collider2D[] hits = new Collider2D[50]; // max 50 enemies at once, adjust if needed
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Enemy"));
        filter.useTriggers = true;

        int count = enemyClearCollider.Overlap(filter, hits);

        // Destroy every enemy found
        for (int i = 0; i < count; i++)
        {
            if (hits[i] != null && hits[i].CompareTag("Enemy"))
            {
                Destroy(hits[i].gameObject);
                Debug.Log("Enemy cleared on respawn");
            }
        }

        enemyClearCollider.enabled = false; // turn off after clearing
    }

    private void DisableCollider()
    {
        enemyClearCollider.enabled = false;
    }
}
