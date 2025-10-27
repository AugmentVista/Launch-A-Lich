using UnityEngine;

public class UpgradeDistributor : MonoBehaviour
{
    [SerializeField] private PlayerInteractionHandler playerInteract;

    [Header("Gameplay References")]
    [SerializeField] private PlayerInteractionHandler player;

    public void ApplyUpgrade(UpgradeItem item, int purchaseCount)
    {
        switch (item.upgradeType)
        {
            case UpgradeType.MaxHealth:
                // player.UpgradeHealth(item.improvementModifier, purchaseCount);
                break;

            case UpgradeType.MaxSpeed:
                // player.UpgradeSpeed(item.improvementModifier, purchaseCount);
                break;

            case UpgradeType.LaunchPower:
                // player.UpgradeLaunchPower(item.improvementModifier, purchaseCount);
                break;

            case UpgradeType.BoostPower:
                // player.UpgradeBoost(item.improvementModifier, purchaseCount);
                break;

            case UpgradeType.Income:
                // player.UpgradeIncome(item.improvementModifier, purchaseCount);
                break;

            case UpgradeType.Bounce:
                player.UpgradeBounce(purchaseCount, item.improvementModifier);
                break;
        }
    }
}