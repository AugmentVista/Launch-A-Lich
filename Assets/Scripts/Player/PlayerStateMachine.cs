using Unity.VisualScripting;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    public PlayerBase playerBase;
    public GameObject player;
    public Rigidbody2D playerRb;

    public float angularDamping = 1f;

    public float speedToStopAt = 3f;

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
        ReadVelocity();
    }

    /// <summary>
    /// Reads the forward velocity of the player and sets velocity to 0 if 1 or less
    /// </summary>
    void ReadVelocity()
    {
        if (playerState == PlayerState.ReadyToLaunch || playerState == PlayerState.Rolling || playerState == PlayerState.Flying)
        {
            if (Mathf.Abs(playerRb.linearVelocityY) > 1 && player.gameObject.transform.position.y > 15f)
            {
                if (playerState != PlayerState.Flying) { Debug.Log("Player has begun Flying"); }
                ChangePlayerState(PlayerState.Flying);
               
            }
            else if (Mathf.Abs(playerRb.linearVelocityX) > 1 && player.transform.position.y <= 15f) 
            {
                if (playerState != PlayerState.Rolling) { Debug.Log("Player has begun "); }
                ChangePlayerState(PlayerState.Rolling);
            }
        }


        if (playerState == PlayerState.Flying || playerState == PlayerState.Rolling)
        {
            if (Mathf.Abs(playerRb.linearVelocityX) <= speedToStopAt && Mathf.Abs(playerRb.linearVelocityX) > 0)
            {
                Debug.Log($"Speed before stop was {playerRb.linearVelocityX}");
                // Player has stopped moving. Triggers state change to Stopped
                ChangePlayerState(PlayerState.Stopped);

                playerRb.bodyType = RigidbodyType2D.Kinematic; // Stops all motion
                playerRb.linearVelocity = Vector2.zero;
                playerRb.angularVelocity = 0f;

                playerRb.bodyType = RigidbodyType2D.Dynamic;
                Debug.Log("Player has stopped moving");
            }
        }
            
    }

    

}
