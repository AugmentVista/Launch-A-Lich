using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] PlayerBase playerBase;
    [SerializeField] PlayerInteractionHandler playerInteract;
    private float currentHealth;

    private float maxHealth;
    [SerializeField] private Image healthFill;

    float fillSpeed = 10f;

    private float targetFillAmount;

    private void Start()
    {
        currentHealth = playerInteract.health;
        maxHealth = playerBase.MaxHealth;
        targetFillAmount = 1.0f;
        healthFill.fillAmount = targetFillAmount;
    }

    private void Update()
    {
        currentHealth = playerInteract.health;
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
            targetFillAmount = currentHealth / maxHealth;
        }
    }

    
}
