using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public bool isBackground;

    public bool playerIsDead = false;

    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 10f, -10f);

    private void OnEnable()
    {
        PlayerStateMachine.OnReadyToLaunch += PlayerAlive;
        PlayerStateMachine.OnStopped += PlayerDead;
    }

    private void OnDisable()
    {
        PlayerStateMachine.OnReadyToLaunch -= PlayerAlive;
        PlayerStateMachine.OnStopped -= PlayerDead;
    }

    void PlayerDead()
    {
        playerIsDead = true;
    }

    void PlayerAlive()
    {
        playerIsDead = false;
    }

    private void AdjustBackground()
    {
        if (isBackground)
        {
            if (playerIsDead)
            {
                offset = new Vector3(0, 7f, -10f);
            }
            else 
            {
                offset = new Vector3(0, 2f, -10f);
            }
        }
    }


    void LateUpdate()
    {
        if (target != null)
        {
            AdjustBackground();
            transform.position = target.position + offset;
        }
    }
}