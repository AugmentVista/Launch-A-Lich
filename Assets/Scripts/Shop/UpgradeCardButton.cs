using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UpgradeCardButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CentralBank bank;
    [SerializeField] private UpgradeDistributor distributor;
    [SerializeField] private ItemDisplay display;
    [SerializeField] private Image cardBackground;

    public void TryPurchaseUpgrade()
    {
        UpgradeItem item = display.upgradeData;

        // check if player can afford
        if (!bank.TrySpendMoney(item.price, true))
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        UpgradeItem item = display.upgradeData;
        if (bank.TrySpendMoney(item.price, false))
        {
            cardBackground.color = Color.green;
        }
        else if (!bank.TrySpendMoney(item.price, false))
        {
            cardBackground.color = Color.red;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UpgradeItem item = display.upgradeData;
        //if (bank.TrySpendMoney(item.price, false))
        //{

        //}
        //else if (!bank.TrySpendMoney(item.price, false))
        //{

        //}
        cardBackground.color = Color.white;
    }
}
