using UnityEngine;
using UnityEngine.UI;

public class AbilityCooldownBar : MonoBehaviour
{
    [SerializeField] PlayerAbility ability;

    public float cooldownDuration;
    private float cooldownTimer;

    public Image fillImage;

    private bool isCoolingDown = false;

    private void Start()
    {
        cooldownDuration = ability.cooldown;
    }

    void Update()
    {
        if (isCoolingDown)
        {
            cooldownTimer -= Time.deltaTime;

            // Clamp value between 0 and 1
            float fillAmount = Mathf.Clamp01(cooldownTimer / cooldownDuration);
            fillImage.fillAmount = fillAmount;

            if (cooldownTimer <= 0f)
            {
                isCoolingDown = false;
                fillImage.fillAmount = 0f;
            }
        }
    }

    public void StartCooldown()
    {
        cooldownTimer = cooldownDuration;
        isCoolingDown = true;
        fillImage.fillAmount = 1f;
    }

    public bool IsOnCooldown()
    {
        return isCoolingDown;
    }
}