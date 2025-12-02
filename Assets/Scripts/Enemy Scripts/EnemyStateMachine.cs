using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    public enum EnemyState
    {
        BeforeCameraView, CameraRight, CameraCenter, CameraLeft, AfterCamera, Advancing, Death
    }
    public EnemyState enemyState;

    public delegate void EnemyStateChange();

    public event EnemyStateChange OnBeforeCameraView;
    public event EnemyStateChange OnCameraRight;
    public event EnemyStateChange OnCameraCenter;
    public event EnemyStateChange OnCameraLeft;
    public event EnemyStateChange OnAfterCamera;
    public event EnemyStateChange OnAdvancing;
    public event EnemyStateChange OnDeath;


    private void Start()
    {
        EnterState(EnemyState.BeforeCameraView);

    }


    public void ChangeEnemyState(EnemyState state)
    {
        if (enemyState == state)
            return;

        enemyState = state;

        switch (state)
        {
            case EnemyState.BeforeCameraView:
                OnBeforeCameraView?.Invoke();
                break;
            case EnemyState.CameraRight:
                OnCameraRight?.Invoke();
                break;
            case EnemyState.CameraCenter:
                OnCameraCenter?.Invoke();
                break;
            case EnemyState.CameraLeft:
                OnCameraLeft?.Invoke();
                break;
            case EnemyState.AfterCamera:
                OnAfterCamera?.Invoke();
                break;
            case EnemyState.Advancing:
                OnAdvancing?.Invoke();
                break;
            case EnemyState.Death:
                OnDeath?.Invoke();
                break;
            default:
                OnDeath?.Invoke();
                break;
        }
    }

    public void ExitState(EnemyState stateToExit)
    {



        ChangeEnemyState(EnemyState.Advancing);
    }

    public void EnterState(EnemyState stateToEnter)
    {
        ChangeEnemyState(stateToEnter);
        

    }

    public void AttackRanged()
    { 
    
    }

    public void AttackMelee()
    { 
    
    }



}