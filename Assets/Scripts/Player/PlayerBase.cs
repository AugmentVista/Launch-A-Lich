using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    public Rigidbody2D playerRb;

    public float minBounceVelocity = 4f;
    public float enemySlowFactor = 0.95f;
    public float floorSlowFactor = 0.7f;

    private PlayerStateMachine stateMachine;

    private GameObject lastCollidedObject = null;
    private float lastCollisionTime = 0f;
    private float collisionIgnoreTime = 0.1f;

    private void Awake()
    {
        stateMachine = GetComponent<PlayerStateMachine>();
        if (stateMachine == null)
            Debug.LogError("PlayerBase requires a PlayerStateMachine component.");
    }

    private void Start()
    {
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
        if (stateMachine.playerState == PlayerStateMachine.PlayerState.Stopped ||
            stateMachine.playerState == PlayerStateMachine.PlayerState.ReadyToLaunch)
            return;

        // Prevent multiple triggers from same object
        if (collision.gameObject == lastCollidedObject && Time.time - lastCollisionTime < collisionIgnoreTime)
            return;

        lastCollidedObject = collision.gameObject;
        lastCollisionTime = Time.time;

        Vector2 vel = playerRb.linearVelocity;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            var enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
                enemy.isDead = true;

            vel.x = Mathf.Max(vel.x * enemySlowFactor, 0f);
            vel.y = Mathf.Max(vel.y, minBounceVelocity);
        }

        if (collision.collider.CompareTag("Floor"))
        {
            vel.x = Mathf.Max(vel.x * floorSlowFactor, 0f); // Clamp to prevent going left
        }

        playerRb.linearVelocity = vel;
    }

    private void CounterBackwardsMovement()
    {
        Vector2 vel = playerRb.linearVelocity;
        if (vel.x < 0f)
            vel.x = 0f;

        playerRb.linearVelocity = vel;
    }
    void Inactive() { }
    void Rolling() { }
    void Flying() { }
    void Stopped() { }
    void ReadyToLaunch() { }
}
