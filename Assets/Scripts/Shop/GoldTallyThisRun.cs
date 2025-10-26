using TMPro;
using UnityEngine;

public class GoldTallyThisRun : MonoBehaviour
{
    public float flyingEnemyValue;
    public float groundEnemyValue;

    public float totalGoldEarned;

    public TextMeshProUGUI goldText;
    public CentralBank bank;

    private void Update()
    {
        goldText.text = $"Total Gold: {bank.totalBalance}";
    }

    private void OnEnable()
    {
        PlayerInteractionHandler.OnFlyingEnemyDefeated += AddGold;
        PlayerInteractionHandler.OnGroundEnemyDefeated += AddGold;
    }

    private void OnDisable()
    {
        PlayerInteractionHandler.OnFlyingEnemyDefeated -= AddGold;
        PlayerInteractionHandler.OnGroundEnemyDefeated -= AddGold;
    }

    private void AddGold(int amount)
    {
        totalGoldEarned += amount;
        bank.totalBalance += amount;
    }

}
