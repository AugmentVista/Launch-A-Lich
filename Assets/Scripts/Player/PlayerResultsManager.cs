using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerResultsManager : MonoBehaviour
{
    public GameObject player;
    public Vector2 startPosition;

    public GameObject distanceArrow;
    public TextMeshProUGUI distanceTraveledThisRunText;

    Vector2 finalPosition;


    // Set the starting position for the player to know to reset to
    private void Start()
    {
        startPosition = new Vector2(player.transform.position.x, player.transform.position.y);
        PlayerStateMachine.OnStopped += ShowDistanceTraveled;
        distanceArrow.SetActive(false);
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
    }

    void Respawn()
    {
        player.transform.position = startPosition;
    }
}
