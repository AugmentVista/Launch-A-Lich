using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerResultsManager : MonoBehaviour
{
    public CentralBank bank;

    public Image progressBarFill;

    [SerializeField] Rigidbody2D playerRb;

    private bool isGameplayHappening;
    private bool badValueX = false;
    private bool badValueY = false;

    public static float globalPlayerSpeedX;
    public static float globalPlayerSpeedY;
    public static float currentDistance;

    public GameObject player;
    public GameObject ground;

    public GameObject ParallaxBackground;

    public UIManager UIManager;

    private float victoryDistance = 5000;

    public float highScoreX = 0f;
    public float highScoreY = 0f;
    private float heightReached;
    private float distanceReached;

    private int enemyGoldThisRun = 0;
    private int itemGoldThisRun = 0;

    public float incomeUpgradeValue;
    public float incomeUpgradeCount;

    public TextMeshProUGUI goldText;

    private void Start()
    {
        ParallaxBackground.SetActive(true);

        progressBarFill.fillAmount = 0f;
    }

    private void Update()
    {
        if (!isGameplayHappening) { return; }

        UpdateVelocityTracking(badValueX, badValueY);
        ScoreTracking();
    }

    private void UpdateVelocityTracking(bool badX, bool badY)
    {
        float linearX = playerRb.linearVelocityX;
        float linearY = playerRb.linearVelocityY;

        // Tell X to behave :3
        if (float.IsNaN(linearX) || float.IsInfinity(linearX))
        {
            linearX = 0f;
            badValueX = true;
        }
        else { badValueX = false; }

        // Tell Y to behave :3
        if (float.IsNaN(linearY) || float.IsInfinity(linearY))
        {
            linearY = 0f;
            badValueY = true;
        }
        else { badValueY = false; }

        // ALWAYS use verified values
        globalPlayerSpeedX = linearX;
        globalPlayerSpeedY = linearY;
    }

    public void ScoreTracking()
    {
        currentDistance = player.transform.position.x;
        float currentHeight = player.transform.position.y;

        if (heightReached < currentHeight) { heightReached = currentHeight; if (heightReached > highScoreY) highScoreY = heightReached; }
        if (distanceReached < currentDistance) { distanceReached = currentDistance; if (distanceReached > highScoreX) highScoreX = distanceReached; }

        float fillAmount = Mathf.Clamp01(currentDistance / victoryDistance);
        progressBarFill.fillAmount = fillAmount;
    }

    private void GamePlayResume()
    {
        isGameplayHappening = true;
    }

    private void GamePlayPause()
    {
        isGameplayHappening = false;
    }

    private void OnEnable()
    {
        PlayerInteractionHandler.OnFlyingEnemyDefeated += TrackEnemyMoneyGain;
        PlayerInteractionHandler.OnGroundEnemyDefeated += TrackEnemyMoneyGain;
        PlayerInteractionHandler.OnGroundItemCollected += TrackItemMoneyGain;
        PlayerInteractionHandler.OnFlyingItemCollected += TrackItemMoneyGain;


        PlayerStateMachine.OnFlying += GamePlayResume;
        PlayerStateMachine.OnGrounded += GamePlayResume;
        PlayerStateMachine.OnStopped += OnRunEnded;
        PlayerStateMachine.OnReadyToLaunch += ResetVariables;
    }
    private void OnDisable()
    {
        PlayerInteractionHandler.OnFlyingEnemyDefeated -= TrackEnemyMoneyGain;
        PlayerInteractionHandler.OnGroundEnemyDefeated -= TrackEnemyMoneyGain;
        PlayerInteractionHandler.OnGroundItemCollected -= TrackItemMoneyGain;
        PlayerInteractionHandler.OnFlyingItemCollected -= TrackItemMoneyGain;


        PlayerStateMachine.OnFlying -= GamePlayResume;
        PlayerStateMachine.OnGrounded -= GamePlayResume;
        PlayerStateMachine.OnStopped -= OnRunEnded;
        PlayerStateMachine.OnReadyToLaunch -= ResetVariables;
    }

    private void TrackEnemyMoneyGain(int amount)
    {
        enemyGoldThisRun += amount;
    }

    private void TrackItemMoneyGain(int amount, Enum someEnum)
    {
        itemGoldThisRun += amount;
    }

    public void UpgradeIncome(float improvementMod, float purchaseCount)
    {
        incomeUpgradeCount = purchaseCount;
        incomeUpgradeValue = improvementMod;
    }

    float ApplyIncomeUpgrade()
    {
        return 1f + (incomeUpgradeCount * (incomeUpgradeValue - 1f));
    }

    void OnRunEnded()
    {
        ResultsMenu();
    }

    void ResultsMenu()
    {
        if (highScoreX < victoryDistance)
        {
            StartCoroutine(ResultsDelay());
        }
        else 
        {
            UIManager.SetVictory();
        }
    }

    IEnumerator ResultsDelay()
    {
        yield return new WaitForSeconds(1.5f);
        CalcuateGoldEarned(distanceReached, heightReached); // I don't need the last two arguments?
        // They are already updated acessible variables
        UIManager.B_Results();
    }

    private void CalcuateGoldEarned(float distance, float height)
    {
        int totalRunGold = Mathf.RoundToInt(distance / 2 + height + enemyGoldThisRun + itemGoldThisRun);

        int deposit = Mathf.RoundToInt(distance / 2 + height * ApplyIncomeUpgrade());
        bank.DepositRunEarnings(deposit);

        goldText.text =
            $"Height = {height:F0}\n" +
            $"Distance = {distance / 2:F0}\n" +
            $"Enemies = {enemyGoldThisRun}\n" +
            $"Treats = {itemGoldThisRun}\n" +
            $"Income Multiplier {ApplyIncomeUpgrade()}x\n\n" +
            $"Earned This Run: {totalRunGold}\n\n" +
            $"Total: {bank.Balance - totalRunGold} + {totalRunGold}";
    }

    public void ResetVariables() // was trying to work out the order this should be and when it should be when I discovered everything above that has been commented out
    {
        globalPlayerSpeedX = 0f;
        globalPlayerSpeedY = 0f;
        currentDistance = 0f;
        ResetResults();
        GamePlayPause();
    }

    void ResetResults()
    {
        heightReached = 0f;
        distanceReached = 0f;
        enemyGoldThisRun = 0;
        itemGoldThisRun = 0;
    }

}