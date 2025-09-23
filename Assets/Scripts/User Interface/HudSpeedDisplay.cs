using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudSpeedDisplay : MonoBehaviour
{
    public Rigidbody2D playerRb;
    public TextMeshProUGUI speedText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void measurePlayerSpeed()
    {
        speedText.text = "Speed: " + playerRb.linearVelocityX.ToString("F2");
    }

    void FixedUpdate()
    {
        measurePlayerSpeed();
    }
}
