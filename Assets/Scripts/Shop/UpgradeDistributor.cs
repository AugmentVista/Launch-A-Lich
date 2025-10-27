using UnityEngine;

public class UpgradeDistributor : MonoBehaviour
{
    [Header("Gameplay References")]
    [SerializeField] private PlayerInteractionHandler player;
    [SerializeField] private SpeedLimit speed;
    [SerializeField] private PowerGauge launcher;
    [SerializeField] private PlayerAbility boost;
    [SerializeField] private PlayerResultsManager results;

    public void ApplyUpgrade(UpgradeItem item, int purchaseCount)
    {
        switch (item.upgradeType)
        {
            case UpgradeType.MaxHealth:
                player.UpgradeMaxHealth(item.improvementModifier, purchaseCount);
                break;
            case UpgradeType.MaxSpeed:
                speed.UpgradeMaxSpeed(item.improvementModifier, purchaseCount);
                break;

            case UpgradeType.LaunchPower:
                launcher.UpgradeLauncher(item.improvementModifier, purchaseCount);
                break;

            case UpgradeType.BoostPower:
                boost.UpgradeBoost(item.improvementModifier, purchaseCount);
                break;

            case UpgradeType.Income:
                results.UpgradeIncome(item.improvementModifier, purchaseCount);
                break;

            case UpgradeType.Bounce:
                player.UpgradeBounce(item.improvementModifier, purchaseCount);
                break;
        }
    }
}