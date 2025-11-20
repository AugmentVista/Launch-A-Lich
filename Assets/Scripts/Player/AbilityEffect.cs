using UnityEngine;

public class AbilityEffect : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] private Transform visualTransform;

    private bool armed = true;

    private Rigidbody2D playerRb;

    public float abilityStrength;

    float relativeAbilityStrength;

    [SerializeField] bool downForce;

    public bool followPlayer = true;

    private Transform playerTransform;

    private void Start()
    {
        float clipLength = animator.GetCurrentAnimatorStateInfo(0).length;
        Destroy(gameObject, clipLength);
    }

    public void SetPlayerTransform(Transform transform)
    {
        playerTransform = transform;
    }

    public void SetPlayerRb(Rigidbody2D rb)
    {
        playerRb = rb;
    }

    private void Update()
    {
        if (PlayerResultsManager.globalPlayerSpeedY < 0 && !downForce)
        {
            relativeAbilityStrength = abilityStrength + PlayerResultsManager.globalPlayerSpeedY * -1;
        }
        else 
        { 
            relativeAbilityStrength = abilityStrength; 
        }
        
    }

    private void LateUpdate()
    {
        if (followPlayer && playerTransform != null)
        {
            visualTransform.position = playerTransform.position;
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!armed) return; 

        if (collision.gameObject.CompareTag("Player"))
        {
            if (playerRb != null)
            {
                if (downForce)
                {
                    NegativeLogarithmicBounce(2f, relativeAbilityStrength);
                }
                else 
                {
                    LogarithmicBounce(2f, relativeAbilityStrength); 
                }
            }
        }
    }

    /// <summary>
    /// if inputX = 2, Angle Above X-Axis = 73.8°
    /// if inputX = 3, Angle Above X-Axis = 69.2°
    /// if inputX = 4, Angle Above X-Axis = 66.1°
    /// if inputX = 5, Angle Above X-Axis = 62.7°
    /// if inputX = 6, Angle Above X-Axis = 60.1°
    /// if inputX = 7, Angle Above X-Axis = 57.6°
    /// if inputX = 8, Angle Above X-Axis = 55.0°
    /// if inputX = 9, Angle Above X-Axis = 53.1°
    /// if inputX = 10, Angle Above X-Axis = 51.4°
    /// if inputX = 11, Angle Above X-Axis = 49.8°
    /// if inputX = 12, Angle Above X-Axis = 48.3°
    /// if inputX = 13, Angle Above X-Axis = 47.0°
    /// if inputX = 14, Angle Above X-Axis = 45.8°
    /// if inputX = 15, Angle Above X-Axis = 44.7°
    /// if inputX = 16, Angle Above X-Axis = 42.1°
    /// if inputX = 17, Angle Above X-Axis = 40.1°
    /// if inputX = 18, Angle Above X-Axis = 39.8°
    /// if inputX = 19, Angle Above X-Axis = 38.7°
    /// if inputX = 20, Angle Above X-Axis = 37.8°
    /// if inputX = 21, Angle Above X-Axis = 36.7°
    /// if inputX = 22, Angle Above X-Axis = 35.8°
    /// if inputX = 23, Angle Above X-Axis = 35.0°
    /// if inputX = 24, Angle Above X-Axis = 34.2°
    /// if inputX = 25, Angle Above X-Axis = 33.4°
    /// if inputX = 26, Angle Above X-Axis = 32.6°
    /// if inputX = 27, Angle Above X-Axis = 32.0°
    /// if inputX = 28, Angle Above X-Axis = 31.3°
    /// if inputX = 29, Angle Above X-Axis = 30.6°
    /// if inputX = 30, Angle Above X-Axis = 30.0°
    /// </summary>
    /// <param name="inputX"></param>
    /// <param name="magnitude"></param>
    public void LogarithmicBounce(float inputX, float magnitude)
    {
        if (inputX <= -2f)
        {
            Debug.LogWarning("Input too small for logarithmic force, must be > -2.");
            return;
        }

        float y = 5f * Mathf.Log(inputX + 2f);
        Vector2 direction = new Vector2(inputX, y).normalized;

        Vector2 force = direction * magnitude;
        playerRb.AddForce(force, ForceMode2D.Impulse);
    }


    /// <summary>
    /// Negative Logarithmic Bounce (Right/Down direction)
    /// 
    /// Uses the same logarithmic relationship as LogarithmicBounce, but inverted vertically.
    /// Produces a downward force instead of upward.
    /// 
    /// if inputX = 2, Angle Below X-Axis = -73.8°
    /// if inputX = 3, Angle Below X-Axis = -69.2°
    /// if inputX = 4, Angle Below X-Axis = -66.1°
    /// if inputX = 5, Angle Below X-Axis = -62.7°
    /// if inputX = 6, Angle Below X-Axis = -60.1°
    /// if inputX = 7, Angle Below X-Axis = -57.6°
    /// if inputX = 8, Angle Below X-Axis = -55.0°
    /// if inputX = 9, Angle Below X-Axis = -53.1°
    /// if inputX = 10, Angle Below X-Axis = -51.4°
    /// if inputX = 11, Angle Below X-Axis = -49.8°
    /// if inputX = 12, Angle Below X-Axis = -48.3°
    /// if inputX = 13, Angle Below X-Axis = -47.0°
    /// if inputX = 14, Angle Below X-Axis = -45.8°
    /// if inputX = 15, Angle Below X-Axis = -44.7°
    /// if inputX = 16, Angle Below X-Axis = -42.1°
    /// if inputX = 17, Angle Below X-Axis = -40.1°
    /// if inputX = 18, Angle Below X-Axis = -39.8°
    /// if inputX = 19, Angle Below X-Axis = -38.7°
    /// if inputX = 20, Angle Below X-Axis = -37.8°
    /// if inputX = 21, Angle Below X-Axis = -36.7°
    /// if inputX = 22, Angle Below X-Axis = -35.8°
    /// if inputX = 23, Angle Below X-Axis = -35.0°
    /// if inputX = 24, Angle Below X-Axis = -34.2°
    /// if inputX = 25, Angle Below X-Axis = -33.4°
    /// if inputX = 26, Angle Below X-Axis = -32.6°
    /// if inputX = 27, Angle Below X-Axis = -32.0°
    /// if inputX = 28, Angle Below X-Axis = -31.3°
    /// if inputX = 29, Angle Below X-Axis = -30.6°
    /// if inputX = 30, Angle Below X-Axis = -30.0°
    /// </summary>
    /// <param name="inputX">Horizontal influence (must be greater than -2)</param>
    /// <param name="magnitude">Force magnitude multiplier</param>
    public void NegativeLogarithmicBounce(float inputX, float magnitude)
    {
        // Prevent domain errors (log of zero or negative)
        if (inputX <= -2f)
        {
            Debug.LogWarning("Input too small for logarithmic force, must be > -2.");
            return;
        }

        // Invert the Y-axis for downward motion
        float y = -5f * Mathf.Log(inputX + 2f);
        Vector2 direction = new Vector2(inputX, y).normalized;

        Vector2 force = direction * magnitude;
        playerRb.AddForce(force, ForceMode2D.Impulse);
    }
}