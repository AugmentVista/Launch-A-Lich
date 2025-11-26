using UnityEngine;
using System.Collections;

public class PlayerStateMachine : MonoBehaviour
{
    public PlayerInteractionHandler playerInteract;
    public GameObject player;
    public GameObject ground;
    public Rigidbody2D playerRb;

    private bool groundWasSearched = false;
    private float speedToStopAt = 4f;
    private float flyingHeightThreshold = 15f;
    public float playerLinearX;

    public static bool allowCameraFollow = true;

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
        if (ground == null)
        {
            ground = GameObject.FindGameObjectWithTag("Ground");
            groundWasSearched = true;
        }
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
                break;
            case PlayerState.Grounded:
                OnGrounded?.Invoke();
                break;
            case PlayerState.Flying:
                OnFlying?.Invoke();
                break;
            case PlayerState.Stopped:
                OnStopped?.Invoke();
                break;
            case PlayerState.ReadyToLaunch:
                OnReadyToLaunch?.Invoke();
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
            if (Mathf.Abs(playerRb.linearVelocityX) <= speedToStopAt && Mathf.Abs(playerRb.linearVelocityX) >= -0.001f && player.transform.position.y < flyingHeightThreshold)
            {
                FreezePlayerMovement();

                ChangePlayerState(PlayerState.Stopped);
            }
            else if (playerInteract.stopCalled && player.transform.position.y < flyingHeightThreshold && playerState!= PlayerState.Stopped)
            {
                playerRb.linearVelocity = Vector2.zero;
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
        // temporarily force the camera to keep following
        PlayerStateMachine.allowCameraFollow = true;

        // HARD STOP THE PLAYER
        playerRb.bodyType = RigidbodyType2D.Kinematic;
        playerRb.linearVelocity = Vector2.zero;
        playerRb.angularVelocity = 0f;
        player.transform.rotation = Quaternion.identity;

        // tiny delay to ensure physics settles
        yield return new WaitForSeconds(0.05f);

        // move to ground for death animation
        Vector3 pos = player.transform.position;

        if (groundWasSearched) { pos.y = 2.5f; } else { pos.y = 0f; }

        player.transform.position = pos;

        // now the camera may stop
        yield return new WaitForSeconds(0.25f); // small buffer for animation
        PlayerStateMachine.allowCameraFollow = false;
    }
}