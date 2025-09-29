using UnityEngine;
using System.Collections;

public class PlayerStateMachine : MonoBehaviour
{
    public PlayerBase playerBase;
    public GameObject player;
    public Rigidbody2D playerRb;

    public float angularDamping = 1f;
    public float speedToStopAt = 3f;
    public float flyingHeightThreshold = 15f;

    float timeSpentStopped = 2f;


    public enum PlayerState
    {
        Inactive, Rolling, Flying, Stopped, ReadyToLaunch
    }
    public PlayerState playerState;

    /// <summary>
    /// A delegate event that other classes can subcribe to
    /// </summary>
    public delegate void PlayerStateChange();

    public static event PlayerStateChange OnInactive;
    public static event PlayerStateChange OnRolling;
    public static event PlayerStateChange OnFlying;
    public static event PlayerStateChange OnStopped;
    public static event PlayerStateChange OnReadyToLaunch;


    private void Start()
    {
        playerState = PlayerState.ReadyToLaunch;
    }
    public void ChangePlayerState(PlayerState state)
    {
        if (playerState == state)
            return;

        playerState = state;

        switch (state)
        {
            case PlayerState.Inactive:
                OnInactive?.Invoke();
                Debug.Log("Player is Inactive");
                break;
            case PlayerState.Rolling:
                OnRolling?.Invoke();
                Debug.Log("Player is rolling");
                break;
            case PlayerState.Flying:
                OnFlying?.Invoke();
                Debug.Log("Player is flying");
                break;
            case PlayerState.Stopped:
                OnStopped?.Invoke();
                Debug.Log("Player has stopped");
                break;
            case PlayerState.ReadyToLaunch:
                OnReadyToLaunch?.Invoke();
                Debug.Log("Player is Ready To Launch");
                break;
        }
    }
    private void Update()
    {
        DetermineState();
    }

    /// <summary>
    /// Changes the playerState according to the player's velocity and height.
    /// </summary>
    private void DetermineState()
    {
        LaunchToMoving();

        MovingToStopped();
    }

    private void LaunchToMoving()
    {
        if (playerState == PlayerState.ReadyToLaunch || playerState == PlayerState.Rolling || playerState == PlayerState.Flying)
        {
            if (Mathf.Abs(playerRb.linearVelocityY) > 1 && player.gameObject.transform.position.y > flyingHeightThreshold)
            {
                if (playerState != PlayerState.Flying) { Debug.Log("Player has begun Flying"); }
                ChangePlayerState(PlayerState.Flying);

            }
            else if (Mathf.Abs(playerRb.linearVelocityX) > 1 && player.transform.position.y <= flyingHeightThreshold)
            {
                if (playerState != PlayerState.Rolling) { Debug.Log("Player has begun "); }
                ChangePlayerState(PlayerState.Rolling);
            }
        }
    }

    private void MovingToStopped()
    {
        if (playerState == PlayerState.Flying || playerState == PlayerState.Rolling)
        {
            if (Mathf.Abs(playerRb.linearVelocityX) <= speedToStopAt && Mathf.Abs(playerRb.linearVelocityX) > 0)
            {
                //Debug.Log($"Speed before stop was {playerRb.linearVelocityX}");

                // Player has stopped moving, trigger state change to Stopped
                ChangePlayerState(PlayerState.Stopped);

                FreezePlayerMovement();
                
            }
        }
    }

    public void StoppedToLaunchReady()
    {
        if (playerState == PlayerState.Stopped)
        { 
            ChangePlayerState(PlayerState.ReadyToLaunch);
        }
    }


    public void FreezePlayerMovement()
    {
       StartCoroutine(FreezeRoutine());
    }

    private IEnumerator FreezeRoutine()
    {
        // Make the player kinematic to stop movement
        playerRb.bodyType = RigidbodyType2D.Kinematic;
        playerRb.linearVelocity = Vector2.zero;
        playerRb.angularVelocity = 0f;
        player.transform.rotation = Quaternion.identity;

        // Wait for 0.25 seconds
        yield return new WaitForSeconds(0.25f);

        // Restore dynamic behavior
        playerRb.bodyType = RigidbodyType2D.Dynamic;
    }
}