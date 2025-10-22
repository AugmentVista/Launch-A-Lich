using TMPro;
using UnityEngine;

public class GoldTallyThisRun : MonoBehaviour
{
    public float flyingEnemyValue;
    public float groundEnemyValue;

    public float totalGoldEarned;


    public TextMeshProUGUI goldText;

    void Start()
    {
        
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

    private void AddGold(float amount)
    {
        totalGoldEarned += amount;
        Debug.Log($"[Gold] +{amount} | Total Gold: {totalGoldEarned}");
    }

}
