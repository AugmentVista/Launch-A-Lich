using UnityEngine;

public class Respawner : MonoBehaviour
{
    [SerializeField] private GameObject player;

    public static bool hasPlayerReturnedToLaunchpad = false;

    // Call this from PlayerResultsManager after results screen opens
    public void RespawnPlayer()
    {
        if (player != null)
        {
            player.transform.position = transform.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.gameObject;
            hasPlayerReturnedToLaunchpad = true;
            Debug.Log("Player has returned to launchpad.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            hasPlayerReturnedToLaunchpad = false;
            Debug.Log("Player has left the launchpad.");
        }
    }
}
