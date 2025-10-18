using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    public virtual int Health { get; set; } = 100;
    public virtual int MaxHealth { get; set; } = 1000;

    public Rigidbody2D playerRb;

    [SerializeField] private PlayerStateMachine stateMachine;

    private void Awake()
    {
        if (stateMachine == null)
            Debug.LogError("PlayerBase requires a PlayerStateMachine component.");
    }

    private void Start()
    {
        PlayerStateMachine.OnInactive += Inactive;
        PlayerStateMachine.OnGrounded += Grounded;
        PlayerStateMachine.OnFlying += Flying;
        PlayerStateMachine.OnStopped += Stopped;
        PlayerStateMachine.OnReadyToLaunch += ReadyToLaunch;
    }

    private void OnDestroy()
    {
        PlayerStateMachine.OnInactive -= Inactive;
        PlayerStateMachine.OnGrounded -= Grounded;
        PlayerStateMachine.OnFlying -= Flying;
        PlayerStateMachine.OnStopped -= Stopped;
        PlayerStateMachine.OnReadyToLaunch -= ReadyToLaunch;
    }

    void Inactive() { }
    void Grounded() {  }
    void Flying() { }
    void Stopped() { }
    void ReadyToLaunch() { ResetHealth(); }

    public virtual void ResetHealth()
    {
        Health = MaxHealth;
    }

#region AngleMath
    private float DirectionToCustomPolarAngle(Vector2 direction)
    {
        float unityAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float customPolarAngle = (90f - unityAngle + 360f) % 360f;
        return customPolarAngle;
    }

    public virtual void XBiasNegative(float magnitude)
    {
        float radians = (90f - 120f) * Mathf.Deg2Rad; // Intended angle: 120°
        Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        Vector2 force = direction * magnitude;
        playerRb.AddForce(force, ForceMode2D.Impulse);

        float angle = DirectionToCustomPolarAngle(direction);
        Debug.LogWarning($"XBiasNegative Angle: {angle:F1}°, Magnitude: {magnitude}, Direction: {direction}");
    }

    public virtual void XBiasPositive(float magnitude)
    {
        float radians = (90f - 60f) * Mathf.Deg2Rad; // Intended angle: 60°
        Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        Vector2 force = direction * magnitude;
        playerRb.AddForce(force, ForceMode2D.Impulse);

        float angle = DirectionToCustomPolarAngle(direction);
        Debug.LogWarning($"XBiasPositive Angle: {angle:F1}°, Magnitude: {magnitude}, Direction: {direction}");
    }

    public virtual void YBiasNegative(float magnitude)
    {
        float radians = (90f - 150f) * Mathf.Deg2Rad; // Intended angle: 150°
        Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        Vector2 force = direction * magnitude;
        playerRb.AddForce(force, ForceMode2D.Impulse);

        float angle = DirectionToCustomPolarAngle(direction);
        Debug.LogWarning($"YBiasNegative Angle: {angle:F1}°, Magnitude: {magnitude}, Direction: {direction}");
    }

    public virtual void YBiasPositive(float magnitude)
    {
        float radians = (90f - 30f) * Mathf.Deg2Rad; // Intended angle: 30°
        Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        Vector2 force = direction * magnitude;
        playerRb.AddForce(force, ForceMode2D.Impulse);

        float angle = DirectionToCustomPolarAngle(direction);
        Debug.LogWarning($"YBiasPositive Angle: {angle:F1}°, Magnitude: {magnitude}, Direction: {direction}");
    }

    public virtual void RandBiasNegative(float magnitude)
    {
        float angleDeg = Random.Range(95f, 150f); // Custom polar angle range: 95°–150°
        float radians = (90f - angleDeg) * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        Vector2 force = direction * magnitude;
        playerRb.AddForce(force, ForceMode2D.Impulse);

        float angle = DirectionToCustomPolarAngle(direction);
        Debug.LogWarning($"RandBiasNegative Angle: {angle:F1}°, Magnitude: {magnitude}, Direction: {direction}");
    }

    public virtual void RandBiasPositive(float magnitude)
    {
        float angleDeg = Random.Range(30f, 85f); // Custom polar angle range: 30°–85°
        float radians = (90f - angleDeg) * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        Vector2 force = direction * magnitude;
        playerRb.AddForce(force, ForceMode2D.Impulse);

        float angle = DirectionToCustomPolarAngle(direction);
        Debug.LogWarning($"RandBiasPositive Angle: {angle:F1}°, Magnitude: {magnitude}, Direction: {direction}");
    }

    public virtual void Forward(float magnitude)
    {
        float radians = (90f - 90f) * Mathf.Deg2Rad; // Intended angle: 90°
        Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        Vector2 force = direction * magnitude;
        playerRb.AddForce(force, ForceMode2D.Impulse);

        float angle = DirectionToCustomPolarAngle(direction);
        Debug.LogWarning($"Forward Angle: {angle:F1}°, Magnitude: {magnitude}, Direction: {direction}");
    }

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

        Debug.LogWarning($"LogarithmicBounce: InputX: {inputX}, Y: {y:F2}, Force: {force}");
    }

    public virtual void ApplyLog2Force(float inputX, float magnitude) // forward curve
    {
        if (inputX <= 0f)
        {
            Debug.LogWarning("ApplyLog2Force: inputX must be > 0 for log2(x)");
            return;
        }

        float y = Mathf.Log(inputX) / Mathf.Log(2f); // log base 2
        Vector2 direction = new Vector2(inputX, y).normalized;

        Vector2 force = direction * magnitude;
        playerRb.AddForce(force, ForceMode2D.Impulse);

        Debug.Log($"[Log2] inputX: {inputX}, y: {y}, direction: {direction}, force: {force}");
    }

    public virtual void ApplyExp2Force(float inputX, float magnitude) // upward curve
    {
        float y = Mathf.Pow(2f, inputX);
        Vector2 direction = new Vector2(inputX, y).normalized;

        Vector2 force = direction * magnitude;
        playerRb.AddForce(force, ForceMode2D.Impulse);

        Debug.Log($"[Exp2] inputX: {inputX}, y: {y}, direction: {direction}, force: {force}");
    }


    #endregion

    public virtual void Stop() { playerRb.linearVelocity = Vector2.zero; playerRb.angularVelocity = 0f; Debug.LogError("STOP CALLED"); }

    public virtual void TakeDamage(int damage)
    {
        float currentVelocity = playerRb.linearVelocityX;

        int newHealth = Health -= damage;

        if (newHealth <= 0f) { Health = 0; }
        else if (newHealth > 0) { Health = newHealth; }


        if (Health == 0f) { Stop(); }
            
    }

}