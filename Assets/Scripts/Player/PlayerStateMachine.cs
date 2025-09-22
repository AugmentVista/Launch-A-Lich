using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    public PlayerBase playerBase;
    public GameObject player;
    public Rigidbody2D playerRb;

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
                //Debug.Log("Player is rolling");
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
        ReadVelocity();
    }

    /// <summary>
    /// Reads the forward velocity of the player and sets velocity to 0 if 1 or less
    /// </summary>
    void ReadVelocity()
    {
        if (playerState == PlayerState.ReadyToLaunch)
        {
            if (Mathf.Abs(playerRb.linearVelocityX) > 1) 
            {
                ChangePlayerState(PlayerState.Rolling);
                Debug.Log("Player has begun rolling");
            }
        }


        if (playerState == PlayerState.Flying || playerState == PlayerState.Rolling)
        {
            if (Mathf.Abs(playerRb.linearVelocityX) < 1 && Mathf.Abs(playerRb.linearVelocityX) > 0)
            {
                playerRb.linearVelocity = Vector2.zero;

                // Player has stopped moving. Triggers state change to Stopped
                ChangePlayerState(PlayerState.Stopped);
                Debug.Log("Player has stopped moving");
            }
        }
            
    }

    

}
