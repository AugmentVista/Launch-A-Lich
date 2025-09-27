using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PowerGauge : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] Image gauge;
    [SerializeField] Rigidbody2D playerRb;
    [SerializeField] ScreenChangingButtons UIManager;

    [SerializeField] float chargeSpeed;
    public float launchForceMultiplier;
    float chargeAmount;
    public float force;

    bool isCharging = false;


    private void Update()
    {
        if (UIManager.Results.activeSelf) { return; }
        if (isCharging)
        {
            chargeAmount += chargeSpeed * Time.deltaTime;
            chargeAmount = Mathf.Clamp01(chargeAmount);
            gauge.fillAmount = chargeAmount;
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
        launchForceMultiplier = chargeAmount;
        gauge.fillAmount = 0;
        Launch();
    }

    private void Launch()
    {
        playerRb.AddForce(transform.up * force * launchForceMultiplier, ForceMode2D.Impulse);
        playerRb.AddForce(transform.right * force * launchForceMultiplier, ForceMode2D.Impulse);
    }
}
