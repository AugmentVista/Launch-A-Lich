using Unity.VisualScripting;
using UnityEngine;

public class PlayerResultsManager
{
    public GameObject player;
    public Vector2 startPosition;

    // Set the starting position for the player to know to reset to
    private void Start()
    {
        startPosition = new Vector2(player.transform.position.x, player.transform.position.y);
    }

    /// <summary>
    /// Records the position of the player
    /// </summary>
    /// <returns></returns>
    Vector2 recordedFinalPosition()
    { 
       Vector2 finalPosition = new Vector2(player.transform.position.x, player.transform.position.y);
        return finalPosition;
    }


    void Respawn()
    {
        player.transform.position = recordedFinalPosition();
    }
}
