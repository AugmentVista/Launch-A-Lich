using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public bool isBackground;

    public bool playerIsDead = false;

    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(5, 4f, -10f);


    [Header("Lag Settings")]
    [SerializeField] private float baseFollowSpeed = 5f;
    [SerializeField] private float maxFollowLag = 1.5f; // Max distance camera can lag behind
    [SerializeField] private float speedThreshold = 40f;
    [SerializeField] private float maxPlayerSpeed = 100f;
    [SerializeField] private float maxCameraLagX = 0.3f;

    [Header("Vertical Lag")]
    [SerializeField] private float verticalLerpSpeed = 2f;
    [SerializeField] private float riseThreshold = 25f;      // rising faster than this → move camera down
    [SerializeField] private float fallThreshold = -25f;     // falling faster than this → move camera up
    [SerializeField] private float maxUpOffset = 6f;         // camera shows more ground
    [SerializeField] private float maxDownOffset = 2f;       // camera shows more sky
    [SerializeField] private float neutralOffsetY = 4f;      // default mid offset


    private void OnEnable()
    {
        PlayerStateMachine.OnReadyToLaunch += PlayerAlive;
        PlayerStateMachine.OnStopped += PlayerDead;
    }

    private void OnDisable()
    {
        PlayerStateMachine.OnReadyToLaunch -= PlayerAlive;
        PlayerStateMachine.OnStopped -= PlayerDead;
    }

    void PlayerDead()
    {
        playerIsDead = true;
    }

    void PlayerAlive()
    {
        playerIsDead = false;
    }

    private void AdjustBackground()
    {
        if (isBackground)
        {
            transform.position = target.position + offset;
            if (playerIsDead)
            {
                offset = new Vector3(0, 7f, -10f);
            }
            else 
            {
                offset = new Vector3(0, 2f, -10f);
            }
        }
    }

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;
        AdjustBackground();

        if (isBackground) { return; }
        
        float playerSpeedX = Mathf.Abs(PlayerResultsManager.globalPlayerSpeedX);
        float playerSpeedY = PlayerResultsManager.globalPlayerSpeedY;

        //  X axis lag 
        float speedFactorX = Mathf.InverseLerp(speedThreshold, maxPlayerSpeed, playerSpeedX);
        float lagOffsetX = Mathf.Lerp(0f, maxFollowLag, speedFactorX) * Mathf.Sign(PlayerResultsManager.globalPlayerSpeedX);
        float followSpeedX = Mathf.Lerp(baseFollowSpeed, baseFollowSpeed * maxCameraLagX, speedFactorX);

        //  Vertical lag (simplified) 
        float targetOffsetY = neutralOffsetY;

        if (playerSpeedY > fallThreshold) // rising fast
            targetOffsetY = Mathf.Lerp(targetOffsetY, maxDownOffset, (playerSpeedY - riseThreshold) / (maxPlayerSpeed - riseThreshold)); 
        else if (playerSpeedY < riseThreshold) // falling fast
            targetOffsetY = Mathf.Lerp(targetOffsetY, maxUpOffset, (playerSpeedY - fallThreshold) / (maxPlayerSpeed - fallThreshold)); 

        // Smoothly return toward target vertical offset
        offset.y = Mathf.Lerp(offset.y, targetOffsetY, Time.deltaTime * verticalLerpSpeed);

        // Aggregate both offsets into a Vector3, z doesn't matter.
        Vector3 targetOffset = new Vector3(-lagOffsetX, offset.y, offset.z);
        Vector3 targetPos = target.position + targetOffset;

        Vector3 smoothPos = new Vector3(
            Mathf.SmoothDamp(transform.position.x, targetPos.x, ref velocity.x, 1f / followSpeedX),
            Mathf.SmoothDamp(transform.position.y, targetPos.y, ref velocity.y, 1f / baseFollowSpeed), targetPos.z);

        transform.position = smoothPos;
    }
}