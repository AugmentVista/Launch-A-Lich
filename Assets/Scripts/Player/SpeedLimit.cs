using UnityEngine;

public class SpeedLimit : MonoBehaviour
{
    [SerializeField] Rigidbody2D playerRb;

    public float maxSpeed;

    [SerializeField] float baseLinearDampeningValue;

    [SerializeField] HudSpeedDisplay hudDisplay;

    private float fullColorThreshold = 0.5f;

    void Start()
    {
        if (playerRb == null)
            playerRb = GetComponent<Rigidbody2D>();
        baseLinearDampeningValue = playerRb.linearDamping;
    }

    void Update()
    {
        float currentSpeedX = Mathf.Abs(playerRb.linearVelocityX);

        float dampX = CalculateDamping(currentSpeedX);

        playerRb.linearDamping = Mathf.Lerp(playerRb.linearDamping, dampX, Time.deltaTime * 5f);

        UpdateSpeedTextColor(playerRb.linearDamping);
    }

    private float CalculateDamping(float velocity)
    {
        if (velocity <= maxSpeed)
            return baseLinearDampeningValue;

        float excessRatio = (velocity - maxSpeed) / maxSpeed;
        float addedDamping = excessRatio * (0.05f / 0.05f); 
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
            Color targetColor = Color.Lerp(Color.black, Color.red, t);

            Color currentColor = hudDisplay.speedText.color;
            hudDisplay.speedText.color = Color.Lerp(currentColor, targetColor, Time.deltaTime * 8f);
        }
    }

}