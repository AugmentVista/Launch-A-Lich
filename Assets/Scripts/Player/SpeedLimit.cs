using UnityEngine;

public class SpeedLimit : MonoBehaviour
{
    [SerializeField] Rigidbody2D playerRb;

    public float maxSpeedX;

    public float maxSpeedY;

    [SerializeField] float baseLinearDampeningValue;

    [SerializeField] float baseGravityScale;

    [SerializeField] HudSpeedDisplay hudDisplay;

    public float maxSpeedUpgradeValue;
    public float maxSpeedUpgradesCount;

    private float maxSpeedUpgradesActive = 0;


    private float fullColorThreshold = 0.5f;

    void Start()
    {
        if (playerRb == null)
            playerRb = GetComponent<Rigidbody2D>();
        baseLinearDampeningValue = playerRb.linearDamping;
        baseGravityScale = playerRb.gravityScale;
    }

    public void UpgradeMaxSpeed(float improvementMod, float purchaseCount)
    {
        maxSpeedUpgradesCount = purchaseCount;
        maxSpeedUpgradeValue = improvementMod;
    }

    float ApplyMaxSpeedUpgrade()
    {
        if (maxSpeedUpgradesCount > 0)
            maxSpeedUpgradesActive = maxSpeedUpgradesCount * maxSpeedUpgradeValue;
        else
            maxSpeedUpgradesActive = 0;

        return maxSpeedUpgradesActive;
    }


    void Update()
    {
        float currentSpeedX = Mathf.Abs(playerRb.linearVelocityX);
        float currentSpeedY = (playerRb.linearVelocityY);

        float dampX = CalculateDamping(currentSpeedX);
        float gravityY = CalculateGravityDrag(currentSpeedY);

        playerRb.linearDamping = Mathf.Lerp(playerRb.linearDamping, dampX, Time.deltaTime * 5f);

        playerRb.gravityScale = Mathf.Lerp(playerRb.gravityScale, gravityY, Time.deltaTime * 5f);

        UpdateSpeedTextColor(playerRb.linearDamping);
    }

    private float CalculateGravityDrag(float velocity)
    {
        if (velocity <= maxSpeedY)
            return baseGravityScale;

        float excessRatio = (velocity - maxSpeedY) / maxSpeedY;
        float addedGravity = excessRatio * (0.50f / 0.10f);// add 0.5 gravity scale every 10% over maxSpeedY
        return baseGravityScale + addedGravity;
    }

    private float CalculateDamping(float velocity)
    {
        if (velocity <= maxSpeedX + ApplyMaxSpeedUpgrade())
            return baseLinearDampeningValue;

        float excessRatio = (velocity - (maxSpeedX + ApplyMaxSpeedUpgrade())) / (maxSpeedX + ApplyMaxSpeedUpgrade());
        float addedDamping = excessRatio * (0.05f / 0.10f); // Add 0.05 linear dampening every 10% over maxSpeedX
        return baseLinearDampeningValue + addedDamping;
    }

    private void UpdateSpeedTextColor(float currentDamping)
    {
        if (currentDamping >= 0.3f)
        {
            hudDisplay.speedText.color = new Color32(255, 0, 0, 255);
        }
        else
        {
            float excess = currentDamping - baseLinearDampeningValue;

            // Normalize the excess to 0–1 for the range between base and 0.3
            float t = Mathf.InverseLerp(0f, fullColorThreshold - baseLinearDampeningValue, excess);

            // Lerp from white to red
            Color targetColor = Color.Lerp(Color.white, Color.red, t);

            Color currentColor = hudDisplay.speedText.color;
            hudDisplay.speedText.color = Color.Lerp(currentColor, targetColor, Time.deltaTime * 8f);
        }
    }

}