using System;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages the player's money.
/// Tracks total lifetime gold and current spendable gold.
/// Subscribes directly to gameplay events.
/// </summary>
public class CentralBank : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text balanceText;

    [Header("Economy Data")]
    [Tooltip("Total gold the player has ever earned (persisted across runs).")]
    public int totalBalance;

    [Tooltip("Gold currently available to spend in shop or upgrades.")]
    [SerializeField] private int balance;

    public int Balance => balance;

    public delegate void OnBalanceChanged(int currentBalance);
    public static event OnBalanceChanged BalanceChanged;

    private void OnEnable()
    {
        PlayerInteractionHandler.OnFlyingEnemyDefeated += AddMoney;
        PlayerInteractionHandler.OnGroundEnemyDefeated += AddMoney;
        PlayerInteractionHandler.OnFlyingItemCollected += AddMoney;
        PlayerInteractionHandler.OnGroundItemCollected += AddMoney;
    }

    private void OnDisable()
    {
        PlayerInteractionHandler.OnFlyingEnemyDefeated -= AddMoney;
        PlayerInteractionHandler.OnGroundEnemyDefeated -= AddMoney;
        PlayerInteractionHandler.OnFlyingItemCollected -= AddMoney;
        PlayerInteractionHandler.OnGroundItemCollected -= AddMoney;
    }

    private void Start()
    {
        UpdateBalanceUI();
    }

    /// <summary>
    /// Add gold to both lifetime and spendable balance.
    /// </summary>
    public void AddMoney(int amount, System.Enum __)
    {
        balance += amount;
        totalBalance += amount;
        UpdateBalanceUI();
    }

    /// <summary>
    /// Spend gold if enough balance is available.
    /// </summary>
    public bool TrySpendMoney(int cost, bool isPurchasing)
    {
        if (balance >= cost && isPurchasing == true)
        {
            balance -= cost;
            UpdateBalanceUI();
            return true;
        }
        else if (balance >= cost)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Called by PlayerResultsManager when a run ends to deposit earned travel gold.
    /// </summary>
    public void DepositRunEarnings(int amount)
    {
        AddMoney(amount, );
    }

    private void UpdateBalanceUI()
    {
        if (balanceText != null)
            balanceText.text = $"${balance}";

        BalanceChanged?.Invoke(balance);
    }

    /// <summary>
    /// Optionally used by save/load system.
    /// </summary>
    public void SetBalance(int newBalance)
    {
        balance = newBalance;
        UpdateBalanceUI();
    }
}