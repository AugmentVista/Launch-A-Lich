using UnityEngine;
using UnityEngine.UI;

public class AbilityCooldownBar : MonoBehaviour
{
    [SerializeField] PlayerAbility ability;

    float cooldownDuration;
    private float cooldownTimer;

    public Image fillImage;

    private bool isCoolingDown = false;

    private void Start()
    {
        cooldownDuration = ability.cooldown;
        fillImage.fillAmount = 1f;
    }

    void Update()
    {
        if (isCoolingDown)
        {
            cooldownTimer -= Time.deltaTime;

            float fillAmount = Mathf.Clamp01(1 - (cooldownTimer / cooldownDuration));
            fillImage.fillAmount = fillAmount;

            if (cooldownTimer <= 0f)
            {
                isCoolingDown = false;
                fillImage.fillAmount = 1f;
            }
        }
    }

    public void StartCooldown()
    {
        cooldownTimer = cooldownDuration;
        isCoolingDown = true;
        fillImage.fillAmount = 0f;
    }

    public bool IsOnCooldown()
    {
        return isCoolingDown;
    }
}
