using TMPro;
using UnityEngine;

public class PlayerResultsManager : MonoBehaviour
{
    public Respawner respawner;

    [SerializeField] Rigidbody2D playerRb;
    public Vector2 globalPlayerSpeedV2;
    public float globalPlayerSpeedX;
    public float globalPlayerSpeedY;

    public GameObject player;
    public GameObject ground;
    public GameObject respawnPoint;
    public Vector2 startPosition;
    public UIManager UIManager;

    public GameObject highScoreBanner;
    public GameObject distanceBanner;
    public GameObject nextButton;

    public float highScoreX = 0f;
    public float highScoreY = 0f;
    private float heightReached;

    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI distanceTraveledThisRunText;

    Vector2 finalPosition;

    private void Start()
    {
        startPosition = new Vector2(respawnPoint.transform.position.x, respawnPoint.transform.position.y);
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
        if (heightReached < currentHeight) { heightReached = currentHeight; }
    }

    float RecordedFinalDistanceX()
    {
        Vector2 finalPosition = new Vector2(player.transform.position.x, player.transform.position.y);
        float distanceCovered = finalPosition.x - startPosition.x;
        return distanceCovered;
    }

    float RecordedFinalDistanceY()
    {
        Vector2 finalPosition = new Vector2(player.transform.position.x, heightReached);
        float distanceCovered = finalPosition.y - player.transform.position.y;
        return distanceCovered;
    }


    void ShowDistanceTraveled()
    {
        float distanceThisRun = RecordedFinalDistanceX();
        float heightThisRun = RecordedFinalDistanceY();
        Vector3 basePosition = new Vector3(player.transform.position.x, player.transform.position.y, 0f );

        // Determine if this run broke any records
        bool brokeX = distanceThisRun > highScoreX;
        bool brokeY = heightThisRun > highScoreY;

        if (brokeX) highScoreX = distanceThisRun;
        if (brokeY) highScoreY = heightThisRun;

        Vector3 highScorePosition = basePosition + new Vector3(0f, 9f, 0f);
        // Build the high score message based on what was broken
        if (brokeX && brokeY)
        {
            highScoreText.text = $"New Distance High Score! {distanceThisRun:F1} meters\nNew Height High Score! {heightThisRun:F1} meters";
            highScoreBanner.SetActive(true);
            highScoreBanner.GetComponent<RectTransform>().position = highScorePosition;
        }
        else if (brokeX)
        {
            highScoreText.text = $"New Distance High Score! {distanceThisRun:F1} meters\nHeight reached {heightThisRun:F1} meters";
            highScoreBanner.SetActive(true);
            highScoreBanner.GetComponent<RectTransform>().position = highScorePosition;
        }
        else if (brokeY)
        {
            highScoreBanner.SetActive(true);
            highScoreBanner.GetComponent<RectTransform>().position = highScorePosition;
            highScoreText.text = $"Distance traveled {distanceThisRun:F1} meters\nNew Height High Score! {heightThisRun:F1} meters";
        }
        else
        {
            distanceBanner.SetActive(true);
            distanceBanner.GetComponent<RectTransform>().position = new Vector3(basePosition.x, basePosition.y + 2f, basePosition.z);
            distanceTraveledThisRunText.text = $"Distance traveled {distanceThisRun:F1} meters\nHeight reached {heightThisRun:F1} meters";
        }

        nextButton.SetActive(true);
        Vector3 confirmButtonPosition = basePosition + new Vector3(15.1f, -4f, 0f);
        nextButton.GetComponent<RectTransform>().position = confirmButtonPosition;
    }

    public void NextButton()
    {
        ResultsMenu();
    }

    void ResultsMenu()
    {
        nextButton.SetActive(false);
        UIManager.B_Results();

        if (respawner != null)
        {
            respawner.RespawnPlayer();
        }

        PlayerStateMachine playerState = player.GetComponent<PlayerStateMachine>();
        if (playerState != null) { playerState.StoppedToLaunchReady(); }
    }

    void ResetResults()
    {
        distanceBanner.SetActive(false);
        distanceTraveledThisRunText.text = "";
    }

    private void OnDestroy()
    {
        Debug.Log("PlayerResultsManager destroyed — unsubscribing from event.");
        PlayerStateMachine.OnStopped -= ShowDistanceTraveled;
        PlayerStateMachine.OnReadyToLaunch -= ResetResults;
    }
}