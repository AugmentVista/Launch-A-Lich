using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class HudSpeedDisplay : MonoBehaviour
{
    public Rigidbody2D playerRb;
    public TextMeshProUGUI speedText;

    public TextMeshProUGUI heightText;

    public float groundHeight = -0.51f;

    public void measurePlayerSpeed()
    {
        speedText.text = $"Speed: {playerRb.linearVelocityX:F2}";
    }

    public void measurePlayerHeight()
    {
        float currentHeight = playerRb.transform.position.y - groundHeight;
        heightText.text = $"Height: {currentHeight:F2} meters";
    }

    void FixedUpdate()
    {
        measurePlayerSpeed();
        measurePlayerHeight();
    }
}
