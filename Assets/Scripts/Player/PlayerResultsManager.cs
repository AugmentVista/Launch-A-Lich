using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerResultsManager : MonoBehaviour
{
    public CentralBank bank;
    public TreatManager treatManager;

    public Transform Environment;

    public Image progressBarFill;

    [SerializeField] Rigidbody2D playerRb;

    private bool isGameplayHappening = false;
    private bool badValueX = false;
    private bool badValueY = false;

    public static float globalPlayerSpeedX = 0;
    public static float globalPlayerSpeedY = 0;
    public static float currentDistance = 0;
    public static float currentHeight = 0;

    public GameObject player;
    public GameObject ground;

    public UIManager UIManager;

    private float victoryDistance = 5000;

    public float highScoreX = 0f;
    public float highScoreY = 0f;
    private float heightReached = 0f;
    private float distanceReached = 0f;

    private int enemyGoldThisRun = 0;
    private int itemGoldThisRun = 0;

    public float incomeUpgradeValue;
    public float incomeUpgradeCount;

    public TextMeshProUGUI goldText;

    void Awake()
    {
        globalPlayerSpeedX = 0;
        globalPlayerSpeedY = 0;
        currentDistance = 0;
        currentHeight = 10f;

        heightReached = 0;
        distanceReached = 0;

        enemyGoldThisRun = 0;
        itemGoldThisRun = 0;

        isGameplayHappening = false;
    }

    private void Start()
    {
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
        currentHeight = player.transform.position.y;

        if (currentDistance > 5000 + Environment.position.x)
        {
            Environment.position += new Vector3(2000f, 0f, 0f);
        }

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
        if (!treatManager.CheckVictoryCondition())
        {
            StartCoroutine(ResultsDelay());
        }
        else 
        {
            StartCoroutine(ResultsDelay());
        }
    }


    IEnumerator ResultsDelay()
    {
        yield return new WaitForSeconds(1.5f);
        CalcuateGoldEarned(distanceReached);
        UIManager.B_Results();
        if (treatManager.CheckVictoryCondition()) { UIManager.SpawnVictoryMeal(); }
    }

    private void CalcuateGoldEarned(float distance)
    {
        int totalRunGold = Mathf.RoundToInt(distance + enemyGoldThisRun + itemGoldThisRun);
        float enemyGoldAfterIncomeMult = (enemyGoldThisRun * ApplyIncomeUpgrade() - enemyGoldThisRun);
        float itemGoldAfterIncomeMult = (itemGoldThisRun * ApplyIncomeUpgrade() - itemGoldThisRun);
        int deposit = Mathf.RoundToInt(distance * ApplyIncomeUpgrade() + itemGoldAfterIncomeMult + enemyGoldAfterIncomeMult);
        bank.DepositRunEarnings(deposit);

        goldText.text =
            $"Distance = ${distance:F0}\n" +
            $"Enemies = ${enemyGoldThisRun}\n" +
            $"Treats = ${itemGoldThisRun}\n" +
            $"Income Multiplier {ApplyIncomeUpgrade()}x\n\n" +
            $"Earned This Run: ${deposit + enemyGoldThisRun + itemGoldThisRun}\n\n" +
            $"New Balance: ${bank.Balance - (deposit + enemyGoldThisRun + itemGoldThisRun)} + ${deposit + enemyGoldThisRun + itemGoldThisRun}";
    }

    public void ResetVariables()
    {
        globalPlayerSpeedX = 0f;
        globalPlayerSpeedY = 0f;
        currentDistance = 0f;
        currentHeight = 10f;
        ResetResults();
        GamePlayPause();
        Environment.position = new Vector3(0f, 0f, 0f);
    }

    void ResetResults()
    {
        heightReached = 0f;
        distanceReached = 0f;
        enemyGoldThisRun = 0;
        itemGoldThisRun = 0;
    }

}