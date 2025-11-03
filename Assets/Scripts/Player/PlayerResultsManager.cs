using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerResultsManager : MonoBehaviour
{
    public Respawner respawner;
    public CentralBank bank;

    public Image progressBarFill;

    [SerializeField] Rigidbody2D playerRb;

    public Vector2 globalPlayerSpeedV2;
    public static float globalPlayerSpeedX;
    public static float globalPlayerSpeedY;

    public GameObject player;
    public GameObject ground;
    public UIManager UIManager;

    public GameObject highScoreBanner;
    public GameObject distanceBanner;
    public GameObject nextButton;

    private float victoryDistance = 5000;

    public float highScoreX = 0f;
    public float highScoreY = 0f;
    private float heightReached;
    private float distanceReached;

    private int enemyGoldThisRun = 0;
    private int itemGoldThisRun = 0;
    public float currentDistance;

    public float incomeUpgradeValue;
    public float incomeUpgradeCount;

    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI distanceTraveledThisRunText;
    public TextMeshProUGUI goldText;

    private void Start()
    {
        PlayerStateMachine.OnStopped += ShowDistanceTraveled;
        PlayerStateMachine.OnReadyToLaunch += ResetResults;

        distanceBanner.SetActive(false);
        highScoreBanner.SetActive(false);
        nextButton.SetActive(false);
        progressBarFill.fillAmount = 0f;
    }

    private void Update()
    {
        globalPlayerSpeedV2 = playerRb.linearVelocity;
        globalPlayerSpeedX = playerRb.linearVelocityX;
        globalPlayerSpeedY = playerRb.linearVelocityY;

        float currentHeight = player.transform.position.y;
        currentDistance = player.transform.position.x;
        if (heightReached < currentHeight) { heightReached = currentHeight; if(heightReached > highScoreY) highScoreY = heightReached; }
        if (distanceReached < currentDistance) { distanceReached = currentDistance; if (distanceReached > highScoreX) highScoreX = distanceReached; }
        float fillAmount = Mathf.Clamp01(currentDistance / victoryDistance);
        progressBarFill.fillAmount = fillAmount;
    }

    void ShowDistanceTraveled()
    {
        float distanceThisRun = distanceReached;
        float heightThisRun = heightReached;
        Vector3 basePosition = new Vector3(player.transform.position.x, ground.transform.position.y + 5.5f, 0f );

        bool brokeX = distanceThisRun >= highScoreX;
        bool brokeY = heightThisRun >= highScoreY;

        Vector3 highScorePosition = basePosition + new Vector3(0f, 9f, 0f);
        if (brokeX && brokeY)
        {
            if (distanceBanner != null) { distanceBanner.SetActive(false); } 
            highScoreBanner.SetActive(true);
            highScoreBanner.GetComponent<RectTransform>().position = new Vector3(basePosition.x, basePosition.y + 4f, basePosition.z);
            highScoreText.text = $"New Distance High Score!\n {distanceThisRun:F1} meters\nEarned {distanceThisRun / 2:F0} gold\nNew Height High Score!\n {heightThisRun:F1} meters\nEarned {heightThisRun:F0} gold";
        }
        else if (brokeX)
        {
            if (distanceBanner != null) { distanceBanner.SetActive(false); }
            highScoreBanner.SetActive(true);
            highScoreBanner.GetComponent<RectTransform>().position = new Vector3(basePosition.x, basePosition.y + 4f, basePosition.z);
            highScoreText.text = $"New Distance High Score!\n {distanceThisRun:F1} meters\nEarned {distanceThisRun / 2:F0} gold\nHeight reached\n {heightThisRun:F1} meters\nEarned {heightThisRun:F0} gold";
        }
        else if (brokeY)
        {
            if (distanceBanner != null) { distanceBanner.SetActive(false); }
            highScoreBanner.SetActive(true);
            highScoreBanner.GetComponent<RectTransform>().position = new Vector3(basePosition.x, basePosition.y + 4f, basePosition.z);
            highScoreText.text = $"Distance traveled\n {distanceThisRun:F1} meters\nEarned {distanceThisRun/2:F0} gold\nNew Height High Score!\nHeight reached \n {heightThisRun:F1} meters\nEarned {heightThisRun:F0} gold";
        }
        else if (!brokeX && !brokeY)
        {
            distanceBanner.SetActive(true);
            distanceBanner.GetComponent<RectTransform>().position = new Vector3(basePosition.x, basePosition.y + 4f, basePosition.z);
            distanceTraveledThisRunText.text = $"Distance traveled\n {distanceThisRun:F1} meters\nEarned {distanceThisRun/2:F0} gold\nHeight reached\n {heightThisRun:F1} meters\nEarned {heightThisRun:F0} gold";
        }
        ResultsMenu();
    }

    IEnumerator ResultsDelay()
    {
        yield return new WaitForSeconds(2);
        CalcuateGoldEarned(distanceReached, heightReached);
        UIManager.B_Results();
    }

    private void OnEnable()
    {
        PlayerInteractionHandler.OnFlyingEnemyDefeated += TrackEnemyMoneyGain;
        PlayerInteractionHandler.OnGroundEnemyDefeated += TrackEnemyMoneyGain;
        PlayerInteractionHandler.OnGroundItemCollected += TrackItemMoneyGain;
        PlayerInteractionHandler.OnFlyingItemCollected += TrackItemMoneyGain;
    }
    private void OnDisable()
    {
        PlayerInteractionHandler.OnFlyingEnemyDefeated -= TrackEnemyMoneyGain;
        PlayerInteractionHandler.OnGroundEnemyDefeated -= TrackEnemyMoneyGain;
        PlayerInteractionHandler.OnGroundItemCollected -= TrackItemMoneyGain;
        PlayerInteractionHandler.OnFlyingItemCollected -= TrackItemMoneyGain;
        PlayerStateMachine.OnStopped -= ShowDistanceTraveled;
        PlayerStateMachine.OnReadyToLaunch -= ResetResults;
    }

    private void TrackEnemyMoneyGain(int amount)
    {
        enemyGoldThisRun += amount;
    }

    private void TrackItemMoneyGain(int amount)
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
        // Ternary Operator condition ? valueIfTrue : valueIfFalse;
        return 1f + (incomeUpgradeCount * (incomeUpgradeValue - 1f));
    }

    private void CalcuateGoldEarned(float distance, float height)
    {
        float travelMoneyEarned = (distance / 2) + height;
        int totalRunGold = Mathf.RoundToInt(travelMoneyEarned + itemGoldThisRun + enemyGoldThisRun);


        int deposit = Mathf.RoundToInt(totalRunGold * ApplyIncomeUpgrade());
        bank.DepositRunEarnings(deposit);

        goldText.text =
            $"Distance Gold Earned: {travelMoneyEarned:F0}\n" +
            $"Enemies Gold Earned: {enemyGoldThisRun}\n" +
            $"Items Gold Earned: {itemGoldThisRun}\n" +
            $"Income bonus {ApplyIncomeUpgrade()}x\n" +
            $"Total earned this run: {deposit}";
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

    void ResetResults()
    {
        heightReached = 0f;
        distanceReached = 0f;
        enemyGoldThisRun = 0;
        itemGoldThisRun = 0;
    }

}