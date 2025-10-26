using UnityEngine;

public class PillarFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 4f, -10f);
    [SerializeField] private GameObject CenterPillar;

    public bool playerIsDead = false;

    void Update()
    {
        transform.position = target.position + offset;
    }


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
        if (CenterPillar != null) { CenterPillar.SetActive(true); }
    }

    void PlayerAlive()
    {
        playerIsDead = false;
        if (CenterPillar != null) { CenterPillar.SetActive(false); }
    }
}
