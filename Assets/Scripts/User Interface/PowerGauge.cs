using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PowerGauge : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
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

        if (isCharging && Respawner.hasPlayerReturnedToLaunchpad && !fullyCharged)
        {
            chargeAmount += chargeSpeed * Time.deltaTime;
            chargeAmount = Mathf.Clamp01(chargeAmount);
            gauge.fillAmount = chargeAmount;
            if (gauge.fillAmount == 1.0) { fullyCharged = true; }
        }
        if (isCharging && Respawner.hasPlayerReturnedToLaunchpad && fullyCharged)
        {
            chargeAmount -= chargeSpeed * Time.deltaTime;
            chargeAmount = Mathf.Clamp01(chargeAmount);
            gauge.fillAmount = chargeAmount;
            if (gauge.fillAmount == 0.0) { fullyCharged = false; }
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

        LogarithmicBounce(14f, appliedForce);
        
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
}
