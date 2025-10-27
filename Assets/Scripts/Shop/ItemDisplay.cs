using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour
{
    [Header("Data")]
    public UpgradeItem upgradeData;

    [Header("UI")]
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public TMP_Text priceText;
    public TMP_Text purchaseCountText;  // Shows "x/5"
    public Image icon;
    public Button purchaseButton; // One button per card

    private int timesPurchased = 0;

    private void Start()
    {
        if (upgradeData != null)
            SetupCard(upgradeData);
    }

    public void SetupCard(UpgradeItem data)
    {
        titleText.text = data.title;
        descriptionText.text = data.description;
        priceText.text = $"${data.price}";
        icon.sprite = data.itemImage;
        UpdatePurchaseVisuals();
    }

    public void IncrementPurchaseCount()
    {
        timesPurchased = Mathf.Min(timesPurchased + 1, upgradeData.maxPurchases);
        UpdatePurchaseVisuals();
    }

    private void UpdatePurchaseVisuals()
    {
        bool canBuyMore = timesPurchased < upgradeData.maxPurchases;
        purchaseButton.interactable = canBuyMore;

        if (purchaseCountText != null)
            purchaseCountText.text = $"{timesPurchased}/{upgradeData.maxPurchases}";
    }

    public int GetPurchaseCount() => timesPurchased;
}