using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    public virtual int Health { get; set; } = 5000;
    public virtual int MaxHealth { get; set; } = 5000;

    public Rigidbody2D playerRb;

    public float minBounceVelocity = 4f;

    [SerializeField] private PlayerStateMachine stateMachine;

    private GameObject lastCollidedObject = null;
    private float lastCollisionTime = 0f;
    private float collisionIgnoreTime = 0.25f;

    private void Awake()
    {
        if (stateMachine == null)
            Debug.LogError("PlayerBase requires a PlayerStateMachine component.");
    }

    private void Start()
    {
        PlayerStateMachine.OnInactive += Inactive;
        PlayerStateMachine.OnRolling += Rolling;
        PlayerStateMachine.OnFlying += Flying;
        PlayerStateMachine.OnStopped += Stopped;
        PlayerStateMachine.OnReadyToLaunch += ReadyToLaunch;
    }

    private void OnDestroy()
    {
        PlayerStateMachine.OnInactive -= Inactive;
        PlayerStateMachine.OnRolling -= Rolling;
        PlayerStateMachine.OnFlying -= Flying;
        PlayerStateMachine.OnStopped -= Stopped;
        PlayerStateMachine.OnReadyToLaunch -= ReadyToLaunch;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (stateMachine.playerState == PlayerStateMachine.PlayerState.Stopped ||
            stateMachine.playerState == PlayerStateMachine.PlayerState.ReadyToLaunch)
            return;

        // Prevent multiple triggers from same object
        if (collision.gameObject == lastCollidedObject && Time.time - lastCollisionTime < collisionIgnoreTime)
            return;

        lastCollidedObject = collision.gameObject;
        lastCollisionTime = Time.time;

        //Vector2 vel = playerRb.linearVelocity;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            //vel.y = Mathf.Max(vel.y, minBounceVelocity);
        }

        //playerRb.linearVelocity = vel;
    }

    void Inactive() { }
    void Rolling() {  }
    void Flying() { }
    void Stopped() { }
    void ReadyToLaunch() { }

    public virtual void ResetHealth()
    {
        Health = MaxHealth;
    }

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

    public virtual void Stop() { playerRb.linearVelocity = Vector2.zero; playerRb.angularVelocity = 0f; }

    public virtual void TakeDamage(int damage)
    {
        float currentVelocity = playerRb.linearVelocityX;

        int newHealth = Health -= damage;
        float newVelocity = currentVelocity - damage;

        if (newHealth <= 0f) { Health = 0; }
        else if (newHealth > 0) { Health = newHealth; }

        if (newVelocity <= 3.0f) { Stop(); }
        else if (newVelocity > 3.0f) { playerRb.linearVelocityX = newVelocity; }
            
    }

}