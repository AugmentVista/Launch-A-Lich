using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PowerGauge : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
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

    [SerializeField] Transform launchArrowTransform;

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


    // Cheat sheet index inputX -> angle above x-axis (logarithmic scaling)
    private readonly Dictionary<int, float> inputXToAngle = new Dictionary<int, float>()
    {
        {2, 85f}, {3, 81f}, {4, 78f}, {5, 75f}, {6, 72f},
        {7, 70f}, {8, 67f}, {9, 65f}, {10, 63f}, {11, 61f},
        {12, 59f}, {13, 57f}, {14, 55f}, {15, 53f}, {16, 51f},
        {17, 49f}, {18, 48f}, {19, 46f}, {20, 44f}, {21, 43f},
        {22, 41f}, {23, 40f}, {24, 38f}, {25, 37f}, {26, 36f},
        {27, 34f}, {28, 33f}, {29, 32f}, {30, 31f}, {31, 30f},
        {32, 29f}, {33, 28f}, {34, 27f}, {35, 26f}, {36, 25f},
        {37, 24f}, {38, 23f}, {39, 22f}, {40, 21f}, {41, 20f},
        {42, 19f}, {43, 18f}, {44, 17f}, {45, 16f}, {46, 15f},
        {47, 14f}, {48, 13f}, {49, 12f}, {50, 10f}
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
        angle = Mathf.Clamp(angle, 10f, 85f);

        if (launchArrowTransform != null) { launchArrowTransform.rotation = Quaternion.Euler(0f, 180f, 90f - angle); }

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

        float appliedForce = force * Mathf.Lerp(minChargeMultiplier + ApplyLauncherUpgrade(),maxChargeMultiplier + ApplyLauncherUpgrade(),chargeAmount);

        float angle = GetAimAngleFromMouse();
        float inputX = GetClosestInputXForAngle(angle);
        Vector2 direction = GetDirectionFromInputX(inputX);

        Vector2 startVelocity = (direction * appliedForce) / playerRb.mass;

        Vector2 gravity = Physics2D.gravity * playerRb.gravityScale;

        // Configure line renderer
        lineRenderer.positionCount = linePoints;

        float time = 0f;
        for (int i = 0; i < linePoints; i++)
        {
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
    /// Mapping of inputX to Angle Above X-Axis using logarithmic mapping:
    /// 
    /// if inputX = 2, Angle Above X-Axis ≈ 85°
    /// if inputX = 3, Angle Above X-Axis ≈ 81°
    /// if inputX = 4, Angle Above X-Axis ≈ 78°
    /// if inputX = 5, Angle Above X-Axis ≈ 75°
    /// if inputX = 6, Angle Above X-Axis ≈ 72°
    /// if inputX = 7, Angle Above X-Axis ≈ 70°
    /// if inputX = 8, Angle Above X-Axis ≈ 67°
    /// if inputX = 9, Angle Above X-Axis ≈ 65°
    /// if inputX = 10, Angle Above X-Axis ≈ 63°
    /// if inputX = 11, Angle Above X-Axis ≈ 61°
    /// if inputX = 12, Angle Above X-Axis ≈ 59°
    /// if inputX = 13, Angle Above X-Axis ≈ 57°
    /// if inputX = 14, Angle Above X-Axis ≈ 55°
    /// if inputX = 15, Angle Above X-Axis ≈ 53°
    /// if inputX = 16, Angle Above X-Axis ≈ 51°
    /// if inputX = 17, Angle Above X-Axis ≈ 49°
    /// if inputX = 18, Angle Above X-Axis ≈ 48°
    /// if inputX = 19, Angle Above X-Axis ≈ 46°
    /// if inputX = 20, Angle Above X-Axis ≈ 44°
    /// if inputX = 21, Angle Above X-Axis ≈ 43°
    /// if inputX = 22, Angle Above X-Axis ≈ 41°
    /// if inputX = 23, Angle Above X-Axis ≈ 40°
    /// if inputX = 24, Angle Above X-Axis ≈ 38°
    /// if inputX = 25, Angle Above X-Axis ≈ 37°
    /// if inputX = 26, Angle Above X-Axis ≈ 36°
    /// if inputX = 27, Angle Above X-Axis ≈ 34°
    /// if inputX = 28, Angle Above X-Axis ≈ 33°
    /// if inputX = 29, Angle Above X-Axis ≈ 32°
    /// if inputX = 30, Angle Above X-Axis ≈ 31°
    /// if inputX = 31, Angle Above X-Axis ≈ 30°
    /// if inputX = 32, Angle Above X-Axis ≈ 29°
    /// if inputX = 33, Angle Above X-Axis ≈ 28°
    /// if inputX = 34, Angle Above X-Axis ≈ 27°
    /// if inputX = 35, Angle Above X-Axis ≈ 26°
    /// if inputX = 36, Angle Above X-Axis ≈ 25°
    /// if inputX = 37, Angle Above X-Axis ≈ 24°
    /// if inputX = 38, Angle Above X-Axis ≈ 23°
    /// if inputX = 39, Angle Above X-Axis ≈ 22°
    /// if inputX = 40, Angle Above X-Axis ≈ 21°
    /// if inputX = 41, Angle Above X-Axis ≈ 20°
    /// if inputX = 42, Angle Above X-Axis ≈ 19°
    /// if inputX = 43, Angle Above X-Axis ≈ 18°
    /// if inputX = 44, Angle Above X-Axis ≈ 17°
    /// if inputX = 45, Angle Above X-Axis ≈ 16°
    /// if inputX = 46, Angle Above X-Axis ≈ 15°
    /// if inputX = 47, Angle Above X-Axis ≈ 14°
    /// if inputX = 48, Angle Above X-Axis ≈ 13°
    /// if inputX = 49, Angle Above X-Axis ≈ 12°
    /// if inputX = 50, Angle Above X-Axis ≈ 10°
    /// </summary>
    /// <param name="inputX"></param>
    /// <param name="magnitude"></param>
    public virtual void LogarithmicBounce(float inputX, float magnitude)
    {
        // Prevent domain errors (log of zero or negative)
        if (inputX <= -2f)
        {
            return;
        }

        Vector2 direction = GetDirectionFromInputX(inputX);

        Vector2 force = direction * magnitude;
        playerRb.AddForce(force, ForceMode2D.Impulse);

    }

    public Vector2 GetDirectionFromInputX(float inputX)
    {
        if (inputX < 2f) inputX = 2f;
        if (inputX > 50f) inputX = 50f;

        float logMin = Mathf.Log(2 + 2f);
        float logMax = Mathf.Log(50 + 2f);
        float raw = Mathf.Log(inputX + 2f);
        float normalized = (raw - logMin) / (logMax - logMin);
        float angle = Mathf.Lerp(85f, 10f, normalized); // mapped angle in degrees

        float angleRad = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)).normalized;
    }

    private float GetAimAngleFromMouse()
    {
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        return Mathf.Clamp(angle, 10f, 85f);
    }

    float GetClosestInputXForAngle(float targetAngle)
    {
        int closestInputX = 14;
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