using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] PlayerInteractionHandler playerInteract;
    private float currentHealth;

    private float maxHealth;
    [SerializeField] private Image healthFill;

    float fillSpeed = 10f;

    private float targetFillAmount;

    private void Start()
    {
        currentHealth = playerInteract.Health;
        maxHealth = playerInteract.MaxHealth;
        targetFillAmount = 1.0f;
        healthFill.fillAmount = targetFillAmount;
    }

    private void Update()
    {
        currentHealth = playerInteract.Health;
        UpdateFillAmount();

        if (Mathf.Abs(healthFill.fillAmount - targetFillAmount) > 0.01f)
        {
            healthFill.fillAmount = Mathf.Lerp(healthFill.fillAmount, targetFillAmount, Time.deltaTime * fillSpeed);
        }
    }

    private void UpdateFillAmount()
    {
        if (healthFill != null)
        {
            maxHealth = playerInteract.MaxHealth;
            targetFillAmount = currentHealth / maxHealth;
        }
    }

    
}
