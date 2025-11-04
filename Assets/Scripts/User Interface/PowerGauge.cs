using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PowerGauge : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Launcher Visuals")]
    [SerializeField] Transform launchArrowTransform;

    [Header("Trajectory Display")]
    public LineRenderer lineRenderer;
    public int linePoints = 175;
    public float timeIntervalInPoints = 0.01f;
    public Transform launchPoint;
    Vector2 aimDirection;

    [Header("Launcher Variables")]
    [SerializeField] Image gauge;
    [SerializeField] Rigidbody2D playerRb;
    [SerializeField] UIManager UIManager;

    [SerializeField] float chargeSpeed;
    [SerializeField]float launchForceMultiplier;
    float chargeAmount;

    public float force;

    float minChargeMultiplier = 1f;
    float maxChargeMultiplier = 1.5f;

    public float launcherUpgradeValue;
    public float launcherUpgradesCount;

    private float launcherUpgradesActive = 0;

    bool isCharging = false;

    bool fullyCharged = false;


    // Lookup table mapping inputX -> angle above x-axis
    private readonly Dictionary<int, float> inputXToAngle = new Dictionary<int, float>()
    {
        {2, 73.8f}, {3, 69.2f}, {4, 66.1f}, {5, 62.7f}, {6, 60.1f},
        {7, 57.6f}, {8, 55.0f}, {9, 53.1f}, {10, 51.4f}, {11, 49.8f},
        {12, 48.3f}, {13, 47.0f}, {14, 45.8f}, {15, 44.7f}, {16, 42.1f},
        {17, 40.1f}, {18, 39.8f}, {19, 38.7f}, {20, 37.8f}, {21, 36.7f},
        {22, 35.8f}, {23, 35.0f}, {24, 34.2f}, {25, 33.4f}, {26, 32.6f},
        {27, 32.0f}, {28, 31.3f}, {29, 30.6f}, {30, 30.0f}
    };


    public void UpgradeLauncher(float improvementMod, float purchaseCount)
    {
        launcherUpgradesCount = purchaseCount;
        launcherUpgradeValue = improvementMod;
    }

    float ApplyLauncherUpgrade()
    {
        if (launcherUpgradesCount > 0)
            launcherUpgradesActive = launcherUpgradesCount * launcherUpgradeValue;
        else
            launcherUpgradesActive = 0;

        return launcherUpgradesActive;
    }

    private void Update()
    {
        if (!UIManager.Gameplay.activeSelf) { return; }
        if (!Respawner.hasPlayerReturnedToLaunchpad) { return; }

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        aimDirection = (mouseWorldPos - transform.position).normalized;

        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        angle = Mathf.Clamp(angle, 30f, 74f);

        if (launchArrowTransform != null)
            launchArrowTransform.rotation = Quaternion.Euler(0f, 180f, 90f - angle);

        if (isCharging && Respawner.hasPlayerReturnedToLaunchpad && !fullyCharged)
        {
            DrawTrajectory();
            lineRenderer.enabled = true;
            chargeAmount += chargeSpeed * Time.deltaTime;
            chargeAmount = Mathf.Clamp01(chargeAmount);
            gauge.fillAmount = chargeAmount;
            if (gauge.fillAmount == 1.0) { fullyCharged = true; }
        }
        if (isCharging && Respawner.hasPlayerReturnedToLaunchpad && fullyCharged)
        {
            DrawTrajectory();
            lineRenderer.enabled = true;
            chargeAmount -= chargeSpeed * Time.deltaTime;
            chargeAmount = Mathf.Clamp01(chargeAmount);
            gauge.fillAmount = chargeAmount;
            if (gauge.fillAmount == 0.0) { fullyCharged = false; }
        }
    }

    void DrawTrajectory()
    {
        Vector2 origin = launchPoint.position;

        // Compute the *actual applied force* based on your current charge and upgrades
        float appliedForce = force * Mathf.Lerp(minChargeMultiplier + ApplyLauncherUpgrade(),
                                                maxChargeMultiplier + ApplyLauncherUpgrade(),
                                                chargeAmount);

        float angle = GetAimAngleFromMouse();
        float inputX = GetClosestInputXForAngle(angle);
        float y = 5f * Mathf.Log(inputX + 2f);
        Vector2 direction = new Vector2(inputX, y).normalized;

        // Calculate the *initial velocity* based on the player's Rigidbody2D mass and applied force
        Vector2 startVelocity = (direction * appliedForce) / playerRb.mass;

        // Gravity (use Rigidbody2D gravity scale)
        Vector2 gravity = Physics2D.gravity * playerRb.gravityScale;

        // Configure line renderer
        lineRenderer.positionCount = linePoints;

        float time = 0f;
        for (int i = 0; i < linePoints; i++)
        {
            // Physics equation: p = p0 + v0 * t + ½ * g * t²
            Vector2 point = origin + startVelocity * time + 0.5f * gravity * (time * time);
            lineRenderer.SetPosition(i, point);
            time += timeIntervalInPoints;
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        isCharging = true;
        chargeAmount = 0f;
        gauge.fillAmount = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        lineRenderer.enabled = false;
        isCharging = false;
        launchForceMultiplier = Mathf.Lerp(minChargeMultiplier + ApplyLauncherUpgrade(), maxChargeMultiplier + ApplyLauncherUpgrade(), chargeAmount);
        gauge.fillAmount = 0;
        playerRb.bodyType = RigidbodyType2D.Dynamic;
        Launch();
    }

    private void Launch()
    {
        if (!Respawner.hasPlayerReturnedToLaunchpad) { return; }

        // Always base force (50), scaled by charge percentage value 0% = min 100% = max
        float appliedForce = force * launchForceMultiplier;

        float angle = GetAimAngleFromMouse();
        float inputX = GetClosestInputXForAngle(angle);

        LogarithmicBounce(inputX, appliedForce);
        
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

        float y = 5f * Mathf.Log(inputX + 2f);
        Vector2 direction = new Vector2(inputX, y).normalized;

        Vector2 force = direction * magnitude;
        playerRb.AddForce(force, ForceMode2D.Impulse);

        Debug.LogWarning($"LogarithmicBounce: InputX: {inputX}, Y: {y:F2}, Force: {force}");
    }

    private float GetAimAngleFromMouse()
    {
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        return Mathf.Clamp(angle, 30f, 74f);
    }

    float GetClosestInputXForAngle(float targetAngle)
    {
        int closestInputX = 14; // fallback (your current default)
        float smallestDifference = float.MaxValue;

        foreach (var kvp in inputXToAngle)
        {
            float diff = Mathf.Abs(kvp.Value - targetAngle);
            if (diff < smallestDifference)
            {
                smallestDifference = diff;
                closestInputX = kvp.Key;
            }
        }

        return closestInputX;
    }



}