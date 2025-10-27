using TMPro;
using UnityEngine;

public class PlayerResultsManager : MonoBehaviour
{
    public Respawner respawner;
    public CentralBank bank;

    [SerializeField] Rigidbody2D playerRb;
    public Vector2 globalPlayerSpeedV2;
    public static float globalPlayerSpeedX;
    public static float globalPlayerSpeedY;

    public GameObject player;
    public GameObject ground;
    public UIManager UIManager;

    public GameObject highScoreBanner;
    public GameObject distanceBanner;
    public GameObject goldIcon;
    public GameObject nextButton;

    public float highScoreX = 0f;
    public float highScoreY = 0f;
    private float heightReached;
    private float distanceReached;

    private int enemyGoldThisRun = 0;
    private int itemGoldThisRun = 0;


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
    }

    private void Update()
    {
        globalPlayerSpeedV2 = playerRb.linearVelocity;
        globalPlayerSpeedX = playerRb.linearVelocityX;
        globalPlayerSpeedY = playerRb.linearVelocityY;

        float currentHeight = player.transform.position.y;
        float currentDistance = player.transform.position.x;
        if (heightReached < currentHeight) { heightReached = currentHeight; if(heightReached > highScoreY) highScoreY = heightReached; }
        if (distanceReached < currentDistance) { distanceReached = currentDistance; if (distanceReached > highScoreX) highScoreX = distanceReached; }
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
            highScoreBanner.SetActive(true);
            highScoreBanner.GetComponent<RectTransform>().position = new Vector3(basePosition.x, basePosition.y + 4f, basePosition.z);
            highScoreText.text = $"New Distance High Score!\n {distanceThisRun:F1} meters\nNew Height High Score!\n {heightThisRun:F1} meters";
        }
        else if (brokeX)
        {
            highScoreBanner.SetActive(true);
            highScoreBanner.GetComponent<RectTransform>().position = new Vector3(basePosition.x, basePosition.y + 4f, basePosition.z);
            highScoreText.text = $"New Distance High Score!\n {distanceThisRun:F1} meters\nHeight reached\n {heightThisRun:F1} meters";
        }
        else if (brokeY)
        {
            highScoreBanner.SetActive(true);
            highScoreBanner.GetComponent<RectTransform>().position = new Vector3(basePosition.x, basePosition.y + 4f, basePosition.z);
            highScoreText.text = $"Distance traveled\n {distanceThisRun:F1} meters\nNew Height High Score!\n {heightThisRun:F1} meters";
        }
        else
        {
            distanceBanner.SetActive(true);
            distanceBanner.GetComponent<RectTransform>().position = new Vector3(basePosition.x, basePosition.y + 4f, basePosition.z);
            distanceTraveledThisRunText.text = $"Distance traveled\n {distanceThisRun:F1} meters\nHeight reached\n {heightThisRun:F1} meters";
        }
        
        nextButton.SetActive(true);
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

    public void NextButton()
    {
        ResultsMenu();
    }

    void ResultsMenu()
    {
        if (highScoreX < 500f)
        {
            nextButton.SetActive(false);
            UIManager.B_Results();
            CalcuateGoldEarned(distanceReached, heightReached);

            if (respawner != null)
            {
                respawner.RespawnPlayer();
            }

            ResetResults();
            PlayerStateMachine playerState = player.GetComponent<PlayerStateMachine>();
            if (playerState != null) { playerState.StoppedToLaunchReady(); }
        }
        else 
        {
            UIManager.SetVictory();
        }
    }

    void ResetResults()
    {
        distanceBanner.SetActive(false);
        distanceTraveledThisRunText.text = "";
        heightReached = 0f;
        distanceReached = 0f;
        enemyGoldThisRun = 0;
        itemGoldThisRun = 0;
    }

    private void OnDestroy()
    {
        Debug.Log("PlayerResultsManager destroyed — unsubscribing from event.");
        PlayerStateMachine.OnStopped -= ShowDistanceTraveled;
        PlayerStateMachine.OnReadyToLaunch -= ResetResults;
    }
}