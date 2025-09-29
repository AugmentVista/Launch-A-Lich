using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D playerRb;

    [Header("Bounce Settings")]
    public float minBounceVelocity = 5f; // Minimum Y velocity when bouncing on enemies
    public float enemySlowFactor = 0.95f; // Horizontal slowdown when hitting enemy
    public float floorSlowFactor = 0.7f; // Horizontal slowdown when hitting floor

    private PlayerStateMachine stateMachine;

    private void Awake()
    {
        stateMachine = GetComponent<PlayerStateMachine>();
        if (stateMachine == null)
            Debug.LogError("PlayerBase requires a PlayerStateMachine component on the same GameObject.");
    }

    private void Start()
    {
        // Subscribe to player state events (optional if needed elsewhere)
        PlayerStateMachine.OnInactive += Inactive;
        PlayerStateMachine.OnRolling += Rolling;
        PlayerStateMachine.OnFlying += Flying;
        PlayerStateMachine.OnStopped += Stopped;
        PlayerStateMachine.OnReadyToLaunch += ReadyToLaunch;
    }

    private void OnDestroy()
    {
        PlayerStateMachine.OnInactive -= Inactive;
        PlayerStateMachine.OnRolling -= Rolling;
        PlayerStateMachine.OnFlying -= Flying;
        PlayerStateMachine.OnStopped -= Stopped;
        PlayerStateMachine.OnReadyToLaunch -= ReadyToLaunch;
    }

    private void FixedUpdate()
    {
        CounterBackwardsMovement();
        StopPlayerIfNecessary();
    }

    private void StopPlayerIfNecessary()
    {
        if (stateMachine.playerState == PlayerStateMachine.PlayerState.Stopped ||
            stateMachine.playerState == PlayerStateMachine.PlayerState.ReadyToLaunch)
        {
            playerRb.linearVelocity = Vector2.zero;
            playerRb.angularVelocity = 0f;
            playerRb.rotation = 0f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Only apply bounce/velocity changes if player is moving
        if (stateMachine.playerState == PlayerStateMachine.PlayerState.Stopped ||
            stateMachine.playerState == PlayerStateMachine.PlayerState.ReadyToLaunch)
            return;

        Vector2 vel = playerRb.linearVelocity;

        Debug.Log($"Player collided with {collision.gameObject.name}");

        if (collision.gameObject.CompareTag("Enemy"))
        {
            GameObject lastEnemy = collision.gameObject;
            var enemyComponent = lastEnemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.isDead = true;
            }

            vel.x = Mathf.Min(vel.x - enemySlowFactor, 3.1f);
            vel.y = Mathf.Max(vel.y, minBounceVelocity);
        }

        if (collision.collider.CompareTag("Floor"))
        {
            vel.x *= floorSlowFactor;
        }

        playerRb.linearVelocity = vel;
    }

    /// <summary>
    /// Prevent the player from sliding slightly backwards
    /// </summary>
    private void CounterBackwardsMovement()
    {
        Vector2 vel = playerRb.linearVelocity;

        // If velocity is negative (moving left), clamp to zero
        if (vel.x < 0f)
        {
            vel.x = 0f;
        }

        playerRb.linearVelocity = vel;
    }

    // --- Event handlers for PlayerStateMachine ---
    void Inactive() { }
    void Rolling() { }
    void Flying() { }
    void Stopped() { }
    void ReadyToLaunch() { }
}
