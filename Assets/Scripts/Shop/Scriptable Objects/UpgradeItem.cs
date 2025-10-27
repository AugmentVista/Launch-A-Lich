using UnityEngine;

public enum UpgradeType
{
    MaxHealth,
    MaxSpeed,
    LaunchPower,
    BoostPower,
    Income,
    Bounce
}

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Upgrades/Upgrade Item")]
public class UpgradeItem : ScriptableObject
{
    [Header("Static Data")]
    public string title;
    [TextArea] public string description;
    public Sprite itemImage;
    public int price;
    public float improvementModifier;
    public int maxPurchases = 5;
    public UpgradeType upgradeType;
}
