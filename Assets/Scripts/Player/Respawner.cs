using UnityEngine;
using System.Collections;


public class Respawner : MonoBehaviour
{
    [SerializeField] GameObject player;


    public static bool hasPlayerReturnedToLaunchpad = false;

    void Start()
    {
        PlayerStateMachine.OnStopped += StartRespawnTimer;
    }

    void StartRespawnTimer()
    {
        StartCoroutine(DelayBeforeRespawn(2.5f));
    }

    IEnumerator DelayBeforeRespawn(float delay)
    {
        yield return new WaitForSeconds(delay);
        RespawnPlayer();
    }

    public void RespawnPlayer()
    {
        if (player != null)
        {
            player.transform.position = transform.position;
            hasPlayerReturnedToLaunchpad = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player = collision.gameObject;
            PlayerStateMachine.OnStopped -= StartRespawnTimer;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player = collision.gameObject;
            hasPlayerReturnedToLaunchpad = false;
            PlayerStateMachine.OnStopped += StartRespawnTimer;
        }
    }

    private void OnDestroy()
    {
        Debug.Log("Respawner destroyed — unsubscribing from event.");
        PlayerStateMachine.OnStopped -= StartRespawnTimer;
    }

}
