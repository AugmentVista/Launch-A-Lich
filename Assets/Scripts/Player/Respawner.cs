using UnityEngine;

public class Respawner : MonoBehaviour
{
    [SerializeField] private GameObject player;

    [SerializeField] PlayerStateMachine playerStateMachine;

    public static bool hasPlayerReturnedToLaunchpad = false;

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
            if (player == null) { return; }
            hasPlayerReturnedToLaunchpad = true;
            playerStateMachine.StoppedToLaunchReady();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.gameObject;
            hasPlayerReturnedToLaunchpad = false;
        }
    }
}