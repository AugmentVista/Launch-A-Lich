using UnityEngine;

public class SpeedLimit : MonoBehaviour
{
    [SerializeField] Rigidbody2D playerRb;

    public float maxSpeedX;
    private float baseMaxSpeedX = 50;

    public float maxSpeedY;
    private float baseMaxSpeedY = 50;

    [SerializeField] float baseLinearDampeningValue;

    [SerializeField] float baseGravityScale;

    [SerializeField] HudSpeedDisplay hudDisplay;

    public float maxSpeedUpgradeValue;
    public float maxSpeedUpgradesCount;

    private float maxSpeedUpgradesActive = 0;

    private float fullColorThreshold = 0.25f;

    private float gravityStepFactor = 0.5f;

    private float percentageIncrement = 0.10f;

    public bool overSpeed = false;

    void Start()
    {
        if (playerRb == null) { playerRb = GetComponent<Rigidbody2D>(); }
            
        baseLinearDampeningValue = playerRb.linearDamping;
        baseGravityScale = playerRb.gravityScale;
    }

    public void UpgradeMaxSpeed(float improvementMod, float purchaseCount)
    {
        maxSpeedUpgradesCount = purchaseCount;
        maxSpeedUpgradeValue = improvementMod;
    }

    private float ApplyMaxSpeedUpgrade()
    {
        if (maxSpeedUpgradesCount > 0)
            maxSpeedUpgradesActive = maxSpeedUpgradesCount * maxSpeedUpgradeValue;
        else
            maxSpeedUpgradesActive = 0;

        return maxSpeedUpgradesActive;
    }

    void Update()
    {
        maxSpeedX = baseMaxSpeedX + ApplyMaxSpeedUpgrade();
        maxSpeedY = baseMaxSpeedY + ApplyMaxSpeedUpgrade();

        float currentSpeedX = Mathf.Abs(PlayerResultsManager.globalPlayerSpeedX);
        float currentSpeedY = PlayerResultsManager.globalPlayerSpeedY;

        // DIRECT overspeed check
        overSpeed = currentSpeedX > maxSpeedX;

        float dampX = CalculateDamping(currentSpeedX);
        float gravityY = CalculateGravityDrag(currentSpeedY);

        playerRb.linearDamping = Mathf.Lerp(playerRb.linearDamping, dampX, Time.deltaTime * 5f);
        playerRb.gravityScale = Mathf.Lerp(playerRb.gravityScale, gravityY, Time.deltaTime * 5f);

        UpdateSpeedTextColor(playerRb.linearDamping);
    }

    private float CalculateGravityDrag(float velocity)
    {
        if (velocity <= maxSpeedY) { return baseGravityScale; }

        float excessRatio = Mathf.Clamp01((velocity - maxSpeedY) / maxSpeedY);

        float addedGravity = excessRatio * (gravityStepFactor / percentageIncrement);// add 0.5 gravity scale every 10% over maxSpeedY
        return baseGravityScale + addedGravity;
    }

    private float CalculateDamping(float velocity)
    {
        if (velocity <= maxSpeedX + ApplyMaxSpeedUpgrade())
            return baseLinearDampeningValue;

        float excessRatio = (velocity - (maxSpeedX + ApplyMaxSpeedUpgrade())) / (maxSpeedX + ApplyMaxSpeedUpgrade());
        float addedDamping = excessRatio * (0.05f / 0.075f); // Add 0.05 linear dampening every 5% over maxSpeedX
        return baseLinearDampeningValue + addedDamping;
    }

    private void UpdateSpeedTextColor(float currentDamping)
    {
        if (currentDamping >= fullColorThreshold)
        {
            // If the level of dampening is past the full color threshold set text color to full red.
            hudDisplay.speedText.color = Color.red;
        }
        else
        {
            float excess = currentDamping - baseLinearDampeningValue;

            // Normalize the excess to 0–1 for the range between base and 0.3
            float t = Mathf.InverseLerp(0f, fullColorThreshold - baseLinearDampeningValue, excess);

            // Lerp from black to red
            Color targetColor = Color.Lerp(Color.black, Color.red, t);

            Color currentColor = hudDisplay.speedText.color;
            hudDisplay.speedText.color = Color.Lerp(currentColor, targetColor, Time.deltaTime * 10f);
        }
    }

}