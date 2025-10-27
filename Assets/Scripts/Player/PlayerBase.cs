using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    public virtual int Health { get; set; } = 100;
    public virtual int MaxHealth { get; set; } = 100;

    private const int baseMaxHealth = 100;

    public float maxHealthUpgradeValue;

    public float maxHealthUpgradeCount;

    public Rigidbody2D playerRb;

    [SerializeField] private float maxFallSpeed = -100f;

    [SerializeField] private PlayerStateMachine stateMachine;

    private void Awake()
    {
        if (stateMachine == null)
            Debug.LogError("PlayerBase requires a PlayerStateMachine component.");
    }

    public virtual void ResetHealth()
    {
        MaxHealth = baseMaxHealth + (int)ApplyMaxHealthUpgrade();
        Health = MaxHealth;
    }

    public void UpgradeMaxHealth(float purchaseCount, float improvementMod)
    {
        maxHealthUpgradeCount = purchaseCount;
        maxHealthUpgradeValue = improvementMod;
        ResetHealth();
    }

    float ApplyMaxHealthUpgrade()
    {
        return maxHealthUpgradeCount * maxHealthUpgradeValue;
    }


    #region Physics Stuff

    private void FixedUpdate()
    {
        float zRotation = transform.eulerAngles.z;

        // Convert 0–360 to -180 to 180
        if (zRotation > 180)
            zRotation -= 360;

        zRotation = Mathf.Clamp(zRotation, -5f, 5f);

        transform.rotation = Quaternion.Euler(0f, 0f, zRotation);

        if (playerRb.linearVelocityY < maxFallSpeed) { playerRb.linearVelocityY = maxFallSpeed; }
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
    public virtual void LogarithmicBounce(float inputX, float magnitude)
    {
        // Prevent domain errors (log of zero or negative)
        if (inputX <= -2f)
        {
            Debug.LogWarning("Input too small for logarithmic force, must be > -2.");
            return;
        }

        float y = 5f * Mathf.Log(inputX + 2f);  // Your logarithmic function
        Vector2 direction = new Vector2(inputX, y).normalized;  // Normalize to get direction

        Vector2 force = direction * magnitude;
        playerRb.AddForce(force, ForceMode2D.Impulse);

        //Debug.LogWarning($"LogarithmicBounce: InputX: {inputX}, Y: {y:F2}, Force: {force}");
    }

    /// <summary>
    /// if inputX = 2, Angle Above X-Axis = 26.6°
    /// if inputX = 3, Angle Above X-Axis = 27.8°
    /// if inputX = 4, Angle Above X-Axis = 26.6°
    /// if inputX = 5, Angle Above X-Axis = 24.9°
    /// if inputX = 6, Angle Above X-Axis = 23.4°
    /// if inputX = 7, Angle Above X-Axis = 21.8°
    /// if inputX = 8, Angle Above X-Axis = 20.5°
    /// if inputX = 9, Angle Above X-Axis = 19.4°
    /// if inputX = 10, Angle Above X-Axis = 18.7°
    /// </summary>
    /// <param name="inputX"></param>
    /// <param name="magnitude"></param>
    public virtual void ApplyLog2Force(float inputX, float magnitude)
    {
        if (inputX <= 0f)
        {
            //Debug.LogWarning("ApplyLog2Force: inputX must be > 0 for log2(x)");
            return;
        }

        float y = Mathf.Log(inputX) / Mathf.Log(2f); // log base 2
        Vector2 direction = new Vector2(inputX, y).normalized;

        Vector2 force = direction * magnitude;
        playerRb.AddForce(force, ForceMode2D.Impulse);

        //Debug.Log($"[Log2] inputX: {inputX}, y: {y}, direction: {direction}, force: {force}");
    }

    /// <summary>
    ///  if inputX = 2, Angle Above X-Axis = 63.4°
    /// if inputX = 3, Angle Above X-Axis = 69.4°
    /// if inputX = 4, Angle Above X-Axis = 76°
    /// if inputX = 5, Angle Above X-Axis = 81.1°
    /// if inputX = 6, Angle Above X-Axis = 84.6°
    /// if inputX = 7, Angle Above X-Axis = 86.9°
    /// if inputX = 8, Angle Above X-Axis = 88.2°
    /// if inputX = 9, Angle Above X-Axis = 88.9°
    /// </summary>
    /// <param name="inputX"></param>
    /// <param name="magnitude"></param>
    public virtual void ApplyExp2Force(float inputX, float magnitude) 
    {
        float y = Mathf.Pow(2f, inputX);
        Vector2 direction = new Vector2(inputX, y).normalized;

        Vector2 force = direction * magnitude;
        playerRb.AddForce(force, ForceMode2D.Impulse);

        Debug.Log($"[Exp2] inputX: {inputX}, y: {y}, direction: {direction}, force: {force}");
    }

    public virtual void Stop() { playerRb.linearVelocity = Vector2.zero; playerRb.angularVelocity = 0f; /*Debug.LogError("STOP CALLED");*/ }

    #endregion

    public virtual void TakeDamage(int damage)
    {
        int newHealth = Health -= damage;

        if (newHealth <= 0f) { Health = 0; }
        else if (newHealth > 0) { Health = newHealth; }

        if (Health == 0f) { Stop(); }
    }
}