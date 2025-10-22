using UnityEngine;
using System.Collections;

public class PlayerStateMachine : MonoBehaviour
{
    public PlayerBase playerBase;
    public GameObject player;
    public GameObject ground;
    public Rigidbody2D playerRb;

    private float speedToStopAt = 4f;
    private float flyingHeightThreshold = 20f;
    public float playerLinearX;

    public enum PlayerState
    {
        Inactive, Grounded, Flying, Stopped, ReadyToLaunch
    }
    public PlayerState playerState;

    /// <summary>
    /// A delegate event that other classes can subcribe to
    /// </summary>
    public delegate void PlayerStateChange();

    public static event PlayerStateChange OnInactive;
    public static event PlayerStateChange OnGrounded;
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
            case PlayerState.Grounded:
                OnGrounded?.Invoke();
                Debug.Log("Player is Grounded");
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
        playerLinearX = playerRb.linearVelocityX;
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
        if (playerState == PlayerState.ReadyToLaunch || playerState == PlayerState.Grounded || playerState == PlayerState.Flying)
        {
            if (Mathf.Abs(playerRb.linearVelocityY) > 0 && player.gameObject.transform.position.y > flyingHeightThreshold)
            {
                if (playerState != PlayerState.Flying)
                ChangePlayerState(PlayerState.Flying);

            }
            else if (Mathf.Abs(playerRb.linearVelocityX) > 1 && player.transform.position.y <= flyingHeightThreshold)
            {
                if (playerState != PlayerState.Grounded)
                ChangePlayerState(PlayerState.Grounded);
            }
        }
    }

    private void MovingToStopped()
    {
        if (playerState == PlayerState.Flying || playerState == PlayerState.Grounded)
        {
            // check that the player is moving at less than the speed limit and more than 0 to set it to 0 and that the player is on the ground.
            if (Mathf.Abs(playerRb.linearVelocityX) <= speedToStopAt && Mathf.Abs(playerRb.linearVelocityX) >= 0 && player.transform.position.y < flyingHeightThreshold) 
            {
                FreezePlayerMovement();

                ChangePlayerState(PlayerState.Stopped);
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
        // HARD STOP THE PLAYER
        playerRb.bodyType = RigidbodyType2D.Kinematic;
        playerRb.linearVelocity = Vector2.zero;
        playerRb.angularVelocity = 0f;
        player.transform.rotation = Quaternion.identity;

        yield return new WaitForSeconds(0.05f);
        Debug.LogWarning("FREEZE ROUTINE CALLED");
        Vector3 pos = player.transform.position;
        pos.y = ground.transform.position.y - ground.transform.position.y;
        player.transform.position = pos;
    }
}