using TMPro;
using UnityEngine;

public class CentralBank : MonoBehaviour
{
    [SerializeField] private TMP_Text balanceText;

    public int totalBalance;

    private int balance;

    public int Balance => balance;

    private void Start()
    {
        UpdateBalanceUI();
    }

    public void AddMoney(int amount)
    {
        balance += amount;
        UpdateBalanceUI();
    }

    public bool TrySpendMoney(int cost)
    {
        if (balance >= cost)
        {
            balance -= cost;
            UpdateBalanceUI();
            return true;
        }

        Debug.Log("Not enough money!");
        return false;
    }

    private void UpdateBalanceUI()
    {
        if (balanceText != null)
            balanceText.text = $"${balance}";
    }
}
