using TMPro;
using UnityEngine;

public class PlayerResultsManager : MonoBehaviour
{
    public Respawner respawner;

    public GameObject player;
    public GameObject respawnPoint;
    public Vector2 startPosition;
    public ScreenChangingButtons UIManager;

    public GameObject highScoreBanner;
    public GameObject distanceBanner;
    public GameObject confirmButton;

    float highScore = 0f;

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
        confirmButton.SetActive(false);
    }

    float RecordedFinalDistanceX()
    {
        finalPosition = new Vector2(player.transform.position.x, player.transform.position.y);
        Vector2 distanceCovered = finalPosition - startPosition;
        return distanceCovered.x;
    }

    void ShowDistanceTraveled()
    {
        float distance = RecordedFinalDistanceX();
        Vector3 basePosition = new Vector3(finalPosition.x, finalPosition.y + 2f, 0f); // distance banner base

        // Position and show distance banner
        distanceBanner.SetActive(true);
        distanceBanner.GetComponent<RectTransform>().position = basePosition;

        // Position and show high score banner if it's a new high score
        if (highScore < distance)
        {
            highScore = distance;
            highScoreText.text = $"New HighScore! {highScore:F1}";
            highScoreBanner.SetActive(true);

            Vector3 highScorePosition = basePosition + new Vector3(0f, 3f, 0f); // 3f higher
            highScoreBanner.GetComponent<RectTransform>().position = highScorePosition;
        }
        else
        {
            highScoreBanner.SetActive(false);
        }

        // Update distance text
        distanceTraveledThisRunText.text = $"Distance Traveled: {distance:F1} meters";

        // Position and show confirm button
        confirmButton.SetActive(true);
        Vector3 confirmButtonPosition = basePosition + new Vector3(10f, 0f, 0f); // 10f to the right
        confirmButton.GetComponent<RectTransform>().position = confirmButtonPosition;
    }

    public void distanceArrowButton()
    {
        ResultsMenu();
    }

    void ResultsMenu()
    {
        confirmButton.SetActive(false);
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
        highScoreBanner.SetActive(false);
        highScoreText.text = "";
    }

    private void OnDestroy()
    {
        Debug.Log("PlayerResultsManager destroyed — unsubscribing from event.");
        PlayerStateMachine.OnStopped -= ShowDistanceTraveled;
        PlayerStateMachine.OnReadyToLaunch -= ResetResults;
    }
}