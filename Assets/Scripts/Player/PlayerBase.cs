using UnityEngine;
using UnityEngine.Playables;


public class PlayerBase : MonoBehaviour
{
    public Rigidbody2D playerRb;

    private void Start()
    {
        PlayerStateMachine.OnInactive += Inactive;
        PlayerStateMachine.OnRolling += Rolling;
        PlayerStateMachine.OnFlying += Flying;
        PlayerStateMachine.OnStopped += Stopped;
        PlayerStateMachine.OnReadyToLaunch += ReadyToLaunch;
    }

    void Inactive()
    { 
    
    }

    void Rolling()
    {
       
    }

    void Flying()
    { 
    
    }

    void Stopped()
    { 
    
    }

    void ReadyToLaunch()
    { 
    
    }
}