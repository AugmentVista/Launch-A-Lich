using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public bool isBackground;

    public bool playerIsDead;

    public bool applyLeftOffsetBias = false;

    [Header("Debug")]
    public bool isCameraToldToFollow;

    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(5, 4f, -10f); // active offset
    [SerializeField] private Vector3 offsetDefault = new Vector3(5, 4f, -10f); // offset value to default back to
    [SerializeField] private Vector3 offsetLeftBias = new Vector3(-3, 4f, -10f); // offset value while player is at launch site

    [Header("Lag Settings")]
    [SerializeField] private float baseFollowSpeed = 51f;
    [SerializeField] private float maxFollowLag = 1.5f;     // Max distance camera can lag behind
    [SerializeField] private float speedThreshold = 40f;
    [SerializeField] private float maxPlayerSpeed = 100f;
    [SerializeField] private float maxCameraLagX = 0.3f;

    [Header("Vertical Lag")]
    [SerializeField] private float verticalLerpSpeed = 2f;
    [SerializeField] private float riseThreshold = 25f;      // if vertical gain < this: move camera down
    [SerializeField] private float fallThreshold = -25f;     // if vertical gain > this: move camera up
    [SerializeField] private float maxUpOffset = 6f;         // camera shows more ground
    [SerializeField] private float maxDownOffset = 2f;       // camera shows more sky
    [SerializeField] private float neutralOffsetY = 4f;      // default

    [Header("Predictive Bias")]
    [SerializeField] private float maxForwardBias = 15f;
    [SerializeField] private float biasEasePower = 1.5f;

    private void OnEnable()
    {
        PlayerStateMachine.OnReadyToLaunch += PlayerAlive;
        PlayerStateMachine.OnFlying += PlayerAlive;
        PlayerStateMachine.OnStopped += PlayerDead;
    }

    private void OnDisable()
    {
        PlayerStateMachine.OnReadyToLaunch -= PlayerAlive;
        PlayerStateMachine.OnFlying -= PlayerAlive;
        PlayerStateMachine.OnStopped -= PlayerDead;
    }

    private void Start()
    {
        if (applyLeftOffsetBias) { transform.position = target.position + offsetLeftBias; }
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

    private Vector3 velocity = Vector3.zero; // initalize as zero

    public void debugCameraPerms()
    {
        if (!playerIsDead && PlayerStateMachine.allowCameraFollow) // player is alive, camera can follow
        {
            isCameraToldToFollow = true;
        }
        else { isCameraToldToFollow = false; }
    }

    void LateUpdate()
    {
        debugCameraPerms();

        if (playerIsDead && !PlayerStateMachine.allowCameraFollow) // player is dead, camera can't follow
        { 
            return; 
        }

        if (target == null) { return; }

        AdjustBackground();

        if (isBackground) { return; }

        if (applyLeftOffsetBias) { offset = offsetLeftBias; } else { offset = offsetDefault; }

        if (PlayerResultsManager.globalPlayerSpeedX > maxPlayerSpeed)
        {
            baseFollowSpeed = PlayerResultsManager.globalPlayerSpeedX / 2;
        }
        else 
        {
            baseFollowSpeed = 51f;
        }

            float playerSpeedX = Mathf.Abs(PlayerResultsManager.globalPlayerSpeedX);
        float playerSpeedY = PlayerResultsManager.globalPlayerSpeedY;

        //  X axis lag 
        float speedFactorX = Mathf.InverseLerp(speedThreshold, maxPlayerSpeed, playerSpeedX);

        // Apply a nonlinear curve using ^1.5 to create a scaling curve to account for unrestrained speeds
        float curvedSpeedFactor = Mathf.Pow(speedFactorX, biasEasePower);

        float biasOffsetX = Mathf.Lerp(offset.x, maxForwardBias, curvedSpeedFactor);

        float lagOffsetX = Mathf.Lerp(0f, maxFollowLag, speedFactorX) * Mathf.Sign(PlayerResultsManager.globalPlayerSpeedX);
        float followSpeedX = Mathf.Lerp(baseFollowSpeed, baseFollowSpeed * maxCameraLagX, speedFactorX);

        //  Y axis lag 
        float targetOffsetY = neutralOffsetY;

        if (playerSpeedY > fallThreshold) // rising fast
            targetOffsetY = Mathf.Lerp(targetOffsetY, maxDownOffset, (playerSpeedY - riseThreshold) / (maxPlayerSpeed - riseThreshold)); 
        else if (playerSpeedY < riseThreshold) // falling fast
            targetOffsetY = Mathf.Lerp(targetOffsetY, maxUpOffset, (playerSpeedY - fallThreshold) / (maxPlayerSpeed - fallThreshold)); 

        offset.y = Mathf.Lerp(offset.y, targetOffsetY, Time.deltaTime * verticalLerpSpeed);
        
        float combinedOffsetX = biasOffsetX - lagOffsetX;
        // Aggregate both offsets into a Vector3, z doesn't matter.
        Vector3 targetOffset = new Vector3(combinedOffsetX, offset.y, offset.z);
        Vector3 targetPos = target.position + targetOffset;

        Vector3 smoothPos = new Vector3(Mathf.SmoothDamp(transform.position.x, targetPos.x, ref velocity.x, 1f / followSpeedX),
            Mathf.SmoothDamp(transform.position.y, targetPos.y, ref velocity.y, 1f / baseFollowSpeed), targetPos.z);

        transform.position = smoothPos;
    }

    private void OnDrawGizmos()
    {
        if (target == null || isBackground || playerIsDead) return;

        // Draw the camera’s current position
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, 0.3f);

        float playerSpeedX = Application.isPlaying ? Mathf.Abs(PlayerResultsManager.globalPlayerSpeedX) : 0f;
        float speedFactorX = Mathf.InverseLerp(speedThreshold, maxPlayerSpeed, playerSpeedX);

        float curvedSpeedFactor = Mathf.Pow(speedFactorX, biasEasePower);
        float lagOffsetX = Mathf.Lerp(0f, maxFollowLag, speedFactorX) * Mathf.Sign(PlayerResultsManager.globalPlayerSpeedX);
        float biasOffsetX = Mathf.Lerp(0f, maxForwardBias, curvedSpeedFactor);

        // Positions for visualization
        Vector3 playerPos = target.position;
        Vector3 lagPos = playerPos - new Vector3(lagOffsetX, 0f, 0f);
        Vector3 biasPos = playerPos + new Vector3(biasOffsetX, 0f, 0f);

        // Red line is drag
        Gizmos.color = Color.red;
        Gizmos.DrawLine(playerPos, lagPos);
        Gizmos.DrawWireSphere(lagPos, 0.15f);

        // Green line is rubberbanding follow line
        Gizmos.color = Color.green;
        Gizmos.DrawLine(playerPos, biasPos);
        Gizmos.DrawWireSphere(biasPos, 0.15f);
    }

}