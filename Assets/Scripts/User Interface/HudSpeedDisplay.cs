using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudSpeedDisplay : MonoBehaviour
{
    public Rigidbody2D playerRb;

    public TextMeshProUGUI speedText;

    public TextMeshProUGUI heightText;

    public TextMeshProUGUI goalText;

    public Image heightImage;

    public float groundHeight;

    public void MeasurePlayerSpeed()
    {
        speedText.text = $"Speed: {playerRb.linearVelocityX:F2} meters";
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
    }

    void FixedUpdate()
    {
        MeasurePlayerSpeed();
        MeasurePlayerHeight();
        FillHeight();
        MeasureTravel();
    }
}
