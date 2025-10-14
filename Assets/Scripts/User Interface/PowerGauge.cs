using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PowerGauge : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] Image gauge;
    [SerializeField] Rigidbody2D playerRb;
    [SerializeField] UIManager UIManager;

    [SerializeField] float chargeSpeed;
    [SerializeField]float launchForceMultiplier;
    float chargeAmount;
    public float force;

    bool isCharging = false;


    private void Update()
    {
        if (!UIManager.Gameplay.activeSelf) { return; }
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
        playerRb.bodyType = RigidbodyType2D.Dynamic;
        Launch();
    }

    private void Launch()
    {
        launchForceMultiplier = Mathf.Clamp(launchForceMultiplier, 0.2f, 1f);
        float appliedForce = force * launchForceMultiplier;

        //playerRb.AddForce(transform.up * appliedForce, ForceMode2D.Impulse);
        //playerRb.AddForce(transform.right * appliedForce, ForceMode2D.Impulse);


        float radians = (45f) * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        Vector2 launchVector = direction * appliedForce;
        playerRb.AddForce(launchVector, ForceMode2D.Impulse);

    }
}
