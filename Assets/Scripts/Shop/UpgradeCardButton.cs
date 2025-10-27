using UnityEngine;

public class UpgradeCardButton : MonoBehaviour
{
    [SerializeField] private CentralBank bank;
    [SerializeField] private UpgradeDistributor distributor;
    [SerializeField] private ItemDisplay display;

    public void TryPurchaseUpgrade()
    {
        UpgradeItem item = display.upgradeData;

        // check if player can afford
        if (!bank.TrySpendMoney(item.price))
        {
            Debug.Log("Not enough currency to buy " + item.title);
            return;
        }

        // increment the purchase count for that individual card pertaining to this indiviual upgrade button
        display.IncrementPurchaseCount();

        int currentCount = display.GetPurchaseCount();

        // update count to pass to distributor
        distributor.ApplyUpgrade(item, currentCount);
    }
}
