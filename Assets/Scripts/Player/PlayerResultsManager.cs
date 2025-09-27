using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerResultsManager : MonoBehaviour
{
    public GameObject player;
    public Vector2 startPosition;

    float resultsMenuWaitTime = 2f;

    public GameObject distanceArrow;
    public Image resultsMenu;
    public TextMeshProUGUI distanceTraveledThisRunText;

    Vector2 finalPosition;


    // Set the starting position for the player to know to reset to
    private void Start()
    {
        startPosition = new Vector2(player.transform.position.x, player.transform.position.y);
        PlayerStateMachine.OnStopped += ShowDistanceTraveled;
        PlayerStateMachine.OnReadyToLaunch += ResetResults;
        distanceArrow.SetActive(false);
        resultsMenu.gameObject.SetActive(false);
    }

    /// <summary>
    /// Records the position of the player
    /// </summary>
    /// <returns></returns>
    float RecordedFinalDistanceX()
    {
        finalPosition = new Vector2(player.transform.position.x, player.transform.position.y);
        Vector2 distanceCovered = finalPosition - startPosition;
        return distanceCovered.x;
    }

    void ShowDistanceTraveled()
    {
        distanceArrow.SetActive(true);
        float distance = RecordedFinalDistanceX();
        distanceTraveledThisRunText.text = $"Distance Traveled: {distance:F1} meters";
        
        StartCoroutine(ShowResultsMenuAfterDelay(resultsMenuWaitTime));
    }

    IEnumerator ShowResultsMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResultsMenu();
    }

    void ResultsMenu()
    {
        resultsMenu.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    void ResetResults()
    {
        distanceArrow.SetActive(false);
        distanceTraveledThisRunText.text = $"";
    }

    private void OnDestroy()
    {
        Debug.Log("PlayerResultsManager destroyed — unsubscribing from event.");
        PlayerStateMachine.OnStopped -= ShowDistanceTraveled;
        PlayerStateMachine.OnReadyToLaunch -= ResetResults;
    }

}
