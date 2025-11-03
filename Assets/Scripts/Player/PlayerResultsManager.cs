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

    public GameObject ParallaxBackground;
    public GameObject ParallaxBackground2;

    public UIManager UIManager;

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

    public TextMeshProUGUI goldText;

    private void Start()
    {
        ParallaxBackground.SetActive(true);
        ParallaxBackground2.SetActive(false);

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

        if (currentDistance > 2500)
        {
            ParallaxBackground.SetActive(false);
            ParallaxBackground2.SetActive(true);
        }

    }

    void ShowDistanceTraveled()
    {
        float distanceThisRun = distanceReached;
        float heightThisRun = heightReached;
        Vector3 basePosition = new Vector3(player.transform.position.x, ground.transform.position.y + 5.5f, 0f );

        bool brokeX = distanceThisRun >= highScoreX;
        bool brokeY = heightThisRun >= highScoreY;

        Vector3 highScorePosition = basePosition + new Vector3(0f, 9f, 0f);
        //if (brokeX && brokeY)
        //{
        //    if (distanceBanner != null) { distanceBanner.SetActive(false); } 
        //    highScoreBanner.SetActive(true);
        //    highScoreBanner.GetComponent<RectTransform>().position = new Vector3(basePosition.x, basePosition.y + 4f, basePosition.z);
        //    highScoreText.text = $"New Distance High Score!\n {distanceThisRun:F1} meters\nEarned {distanceThisRun / 2:F0} gold\nNew Height High Score!\n {heightThisRun:F1} meters\nEarned {heightThisRun:F0} gold";
        //}
        //else if (brokeX)
        //{
        //    if (distanceBanner != null) { distanceBanner.SetActive(false); }
        //    highScoreBanner.SetActive(true);
        //    highScoreBanner.GetComponent<RectTransform>().position = new Vector3(basePosition.x, basePosition.y + 4f, basePosition.z);
        //    highScoreText.text = $"New Distance High Score!\n {distanceThisRun:F1} meters\nEarned {distanceThisRun / 2:F0} gold\nHeight reached\n {heightThisRun:F1} meters\nEarned {heightThisRun:F0} gold";
        //}
        //else if (brokeY)
        //{
        //    if (distanceBanner != null) { distanceBanner.SetActive(false); }
        //    highScoreBanner.SetActive(true);
        //    highScoreBanner.GetComponent<RectTransform>().position = new Vector3(basePosition.x, basePosition.y + 4f, basePosition.z);
        //    highScoreText.text = $"Distance traveled\n {distanceThisRun:F1} meters\nEarned {distanceThisRun/2:F0} gold\nNew Height High Score!\nHeight reached \n {heightThisRun:F1} meters\nEarned {heightThisRun:F0} gold";
        //}
        //else if (!brokeX && !brokeY)
        //{
        //    distanceBanner.SetActive(true);
        //    distanceBanner.GetComponent<RectTransform>().position = new Vector3(basePosition.x, basePosition.y + 4f, basePosition.z);
        //    distanceTraveledThisRunText.text = $"Distance traveled\n {distanceThisRun:F1} meters\nEarned {distanceThisRun/2:F0} gold\nHeight reached\n {heightThisRun:F1} meters\nEarned {heightThisRun:F0} gold";
        //}
        ResultsMenu();
    }

    IEnumerator ResultsDelay()
    {
        yield return new WaitForSeconds(2);
        CalcuateGoldEarned(distanceReached, heightReached, enemyGoldThisRun, itemGoldThisRun);
        UIManager.B_Results();
    }

    private void OnEnable()
    {
        PlayerInteractionHandler.OnFlyingEnemyDefeated += TrackEnemyMoneyGain;
        PlayerInteractionHandler.OnGroundEnemyDefeated += TrackEnemyMoneyGain;
        PlayerInteractionHandler.OnGroundItemCollected += TrackItemMoneyGain;
        PlayerInteractionHandler.OnFlyingItemCollected += TrackItemMoneyGain;
        PlayerStateMachine.OnStopped += ShowDistanceTraveled;
        PlayerStateMachine.OnReadyToLaunch += ResetResults;
        PlayerStateMachine.OnReadyToLaunch += ResetBackground;
    }
    private void OnDisable()
    {
        PlayerInteractionHandler.OnFlyingEnemyDefeated -= TrackEnemyMoneyGain;
        PlayerInteractionHandler.OnGroundEnemyDefeated -= TrackEnemyMoneyGain;
        PlayerInteractionHandler.OnGroundItemCollected -= TrackItemMoneyGain;
        PlayerInteractionHandler.OnFlyingItemCollected -= TrackItemMoneyGain;
        PlayerStateMachine.OnStopped -= ShowDistanceTraveled;
        PlayerStateMachine.OnReadyToLaunch -= ResetResults;
        PlayerStateMachine.OnReadyToLaunch -= ResetBackground;
    }

    public void ResetBackground()
    {
        ParallaxBackground.SetActive(true);
        ParallaxBackground2.SetActive(false);
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

    private void CalcuateGoldEarned(float distance, float height, float enemyGold, float itemGold)
    {
        int totalRunGold = Mathf.RoundToInt(distance /2 + height + enemyGold + itemGold);


        int deposit = Mathf.RoundToInt(distance / 2 + height * ApplyIncomeUpgrade());
        bank.DepositRunEarnings(deposit);

        goldText.text =
            $"Height = {height:F0}\n" +
            $"Distance = {distance/2:F0}\n" +
            $"Enemies = {enemyGoldThisRun}\n" +
            $"Treats = {itemGoldThisRun}\n" +
            $"Income Multiplier {ApplyIncomeUpgrade()}x\n" +
            $"Gold Earned This Run: {totalRunGold}\n\n" +
            $"Total: {bank.Balance - totalRunGold} + {totalRunGold}";
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