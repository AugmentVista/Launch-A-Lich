using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudSpeedDisplay : MonoBehaviour
{
    public Rigidbody2D playerRb;

    public SpeedLimit speedLimit;

    public TextMeshProUGUI speedText;

    public TextMeshProUGUI heightText;

    public TextMeshProUGUI goalText;

    public TextMeshProUGUI warningTextLow;

    public TextMeshProUGUI warningTextHigh;

    public Image heightImage;

    public float groundHeight;


    public void MeasurePlayerSpeed()
    {
        if (speedLimit.overSpeed) { speedText.text = $"Beyond Speed Limt: \n{playerRb.linearVelocityX:F2} meters"; }
        else if (!speedLimit.overSpeed) { speedText.text = $"Speed: {playerRb.linearVelocityX:F2} meters"; }
    }

    public void MeasurePlayerHeight()
    {
        float currentHeight = playerRb.transform.position.y - groundHeight;
        heightText.text = $"Height\n{currentHeight:F1}";
    }

    public void MeasureTravel()
    {
        goalText.text = $"{playerRb.gameObject.transform.position.x:F0} / 5000m";
    }

    public void FillHeight()
    {
        float normalized = Mathf.Clamp01(playerRb.gameObject.transform.position.y / 100f);
        if (normalized >= 1f)
        {
            heightImage.fillAmount = 1f;
        }
        heightImage.fillAmount = normalized;


        if (heightImage.fillAmount < 0.099f)
        {
            warningTextLow.text = "Warning!\nLow Altitude";
        }
        else if (heightImage.fillAmount > 0.85f)
        {
            warningTextHigh.text = "Warning!\nHigh Altitude";
        }
        else if (heightImage.fillAmount > 0.099f && heightImage.fillAmount < 0.85f)
        {
            warningTextLow.text = "";
            warningTextHigh.text = "";
        }
    }

    void FixedUpdate()
    {
        MeasurePlayerSpeed();
        MeasurePlayerHeight();
        FillHeight();
        MeasureTravel();
    }
}
