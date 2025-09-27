using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D playerRb;

    [Header("Bounce Settings")]
    public float minBounceVelocity = 5f; // Minimum Y velocity when bouncing on enemies
    public float enemySlowFactor = 0.95f; // Horizontal slowdown when hitting enemy
    public float floorSlowFactor = 0.7f; // Horizontal slowdown when hitting floor
    public float backwardsThreshold = -0.05f; // Prevent moving slightly backwards

    private void Start()
    {
        // Subscribe to player state events
        PlayerStateMachine.OnInactive += Inactive;
        PlayerStateMachine.OnRolling += Rolling;
        PlayerStateMachine.OnFlying += Flying;
        PlayerStateMachine.OnStopped += Stopped;
        PlayerStateMachine.OnReadyToLaunch += ReadyToLaunch;
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        PlayerStateMachine.OnInactive -= Inactive;
        PlayerStateMachine.OnRolling -= Rolling;
        PlayerStateMachine.OnFlying -= Flying;
        PlayerStateMachine.OnStopped -= Stopped;
        PlayerStateMachine.OnReadyToLaunch -= ReadyToLaunch;
    }

    private void FixedUpdate()
    {
        CounterBackwardsMovement();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 vel = playerRb.linearVelocity; // Use the current velocity

         if (collision.gameObject.CompareTag("Enemy"))
        {
            GameObject lastEnemy = collision.gameObject;
            var enemyComponent = lastEnemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.isDead = true;
            }
            // Apply bounce / momentum changes to player
            vel.x *= 0.9f; // reduce horizontal speed slightly
            vel.y = Mathf.Max(vel.y, minBounceVelocity); // apply vertical bounce
            playerRb.linearVelocity = vel;
        }
        if (collision.collider.CompareTag("Floor"))
        {
            // Reduce horizontal speed more when hitting the floor
            vel.x *= floorSlowFactor;
        }

        playerRb.linearVelocity = vel; // Apply the modified velocity
    }

    /// <summary>
    /// Prevent the player from sliding slightly backwards
    /// </summary>
    public void CounterBackwardsMovement()
    {
        if (playerRb.linearVelocity.x < backwardsThreshold)
        {
            Vector2 vel = playerRb.linearVelocity;
            vel.x = 0f;
            playerRb.linearVelocity = vel;
        }
    }

    // --- Event handlers for PlayerStateMachine ---

    void Inactive() { }

    void Rolling() { }

    void Flying() { }

    void Stopped() { }

    void ReadyToLaunch() { }
}