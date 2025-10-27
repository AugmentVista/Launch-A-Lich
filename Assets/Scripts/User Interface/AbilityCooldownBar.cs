using UnityEngine;
using UnityEngine.UI;

public class AbilityCooldownBar : MonoBehaviour
{
    [SerializeField] PlayerAbility ability;
    public Image fillImage;

    private bool isCoolingDown = false;

    public bool IsOnCooldown() => isCoolingDown;

    private void Start()
    {
        fillImage.fillAmount = 1f;
    }

    private void Update()
    {
        if (!isCoolingDown) return;

        // Ensure cooldown stays up to date with playerAbility's cooldown
        float cooldownDuration = ability.cooldown;

        float elapsed = Time.time - ability.LastUseTime;
        float normalized = Mathf.Clamp01(elapsed / cooldownDuration);
        fillImage.fillAmount = normalized;

        if (normalized >= 1f)
        {
            fillImage.fillAmount = 1f;
            isCoolingDown = false;
        }
    }

    public void StartCooldown()
    {
        isCoolingDown = true;
        fillImage.fillAmount = 0f;
    }
}