using TMPro;
using UnityEngine;

public class HudSpeedDisplay : MonoBehaviour
{
    public Rigidbody2D playerRb;
    public TextMeshProUGUI speedText;

    public TextMeshProUGUI heightText;

    public float groundHeight;

    //public void HighScoreTextColor()
    //{
    //    if (playerResults != null)
    //    { 
        
    //    }
    //}

    public void measurePlayerSpeed()
    {
        speedText.text = $"Speed: {playerRb.linearVelocityX:F2} meters";
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
