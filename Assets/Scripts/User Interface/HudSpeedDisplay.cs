using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudSpeedDisplay : MonoBehaviour
{
    public SpeedLimit speedLimit;

    public TextMeshProUGUI speedText;
    public TextMeshProUGUI heightText;
    public TextMeshProUGUI goalText;

    public TextMeshProUGUI warningTextLow;
    public TextMeshProUGUI warningTextHigh;

    public Image heightImage;

    public float groundHeight;
    public float ceilingHeight = 100;

    public float tooHighWarning = 0.80f;
    private float tooLowWarning = 0.0999f; // a tiny bit under 10 so the starting position height doesn't yield this warning

    void FixedUpdate()
    {
        MeasurePlayerSpeed();
        MeasurePlayerHeight();
        FillHeight();
        MeasureTravel();
    }

    public void MeasurePlayerSpeed()
    {
        if (speedLimit.overSpeed) 
        {
            speedText.text = $"Beyond Speed Limt: \n{PlayerResultsManager.globalPlayerSpeedX:F2} meters"; 
        }

        if (!speedLimit.overSpeed) 
        {
            speedText.text = $"Speed: {PlayerResultsManager.globalPlayerSpeedX:F2} meters"; 
        }
    }

    public void MeasurePlayerHeight()
    {
        float currentHeight = PlayerResultsManager.currentHeight - groundHeight;
        heightText.text = $"Height\n{currentHeight:F1}";
    }

    public void FillHeight()
    {
        float rawHeight = PlayerResultsManager.currentHeight - groundHeight;

        float normalizedFillValue = Mathf.Clamp01(rawHeight / (ceilingHeight - groundHeight));

        heightImage.fillAmount = normalizedFillValue;

        if (normalizedFillValue < tooLowWarning)
        {
            warningTextLow.text = "Warning!\nLow Altitude";
            warningTextHigh.text = "";
        }
        else if (normalizedFillValue > tooHighWarning)
        {
            warningTextHigh.text = "Warning!\nHigh Altitude";
            warningTextLow.text = "";
        }
        else
        {
            warningTextLow.text = "";
            warningTextHigh.text = "";
        }
    }


    public void MeasureTravel()
    {
        goalText.text = $"{PlayerResultsManager.currentDistance:F0} / 5000m";
    }

}
