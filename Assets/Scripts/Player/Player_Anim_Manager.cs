using UnityEngine;

public class Player_Anim_Manager : MonoBehaviour
{
    private Animator animator;
    private PlayerStateMachine stateMachine;
    private PlayerInteractionHandler interactionHandler;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        stateMachine = GetComponent<PlayerStateMachine>();
        interactionHandler = GetComponent<PlayerInteractionHandler>();
    }

    private void OnEnable()
    {
        PlayerStateMachine.OnReadyToLaunch += PlayIdle;
        PlayerStateMachine.OnReadyToLaunch += GetBetter;
        PlayerStateMachine.OnStopped += PlayDeath;
        PlayerStateMachine.OnStopped += Die;
        PlayerStateMachine.OnFlying += EvaluateFlyingState;
        PlayerStateMachine.OnGrounded += EvaluateFlyingState;
    }

    private void OnDisable()
    {
        PlayerStateMachine.OnReadyToLaunch -= PlayIdle;
        PlayerStateMachine.OnReadyToLaunch -= GetBetter;
        PlayerStateMachine.OnStopped -= PlayDeath;
        PlayerStateMachine.OnStopped -= Die;
        PlayerStateMachine.OnFlying -= EvaluateFlyingState;
        PlayerStateMachine.OnGrounded -= EvaluateFlyingState;
    }

    private void PlayIdle()
    {
        animator.Play("Player_Idle");
    }

    private void EvaluateFlyingState()
    {
        if (PlayerResultsManager.globalPlayerSpeedY > 0.01)
        {
            animator.Play("Player_Rising");
        }
        else if (PlayerResultsManager.globalPlayerSpeedY < -0.01)
        { 
            animator.Play("Player_Falling");
        }
    }

    public void PlayRolling()
    {
        animator.Play("Player_Rolling");
    }

    private void Die()
    {
        //isDead = true;
    }

    private void GetBetter()
    {
        //isDead = false;
    }

    public void PlayDeath()
    {
        animator.Play("Player_Death");
        //isDead = false;
    }

    public void PlayTakeHit()
    {
        animator.Play("Player_Take_Hit");
    }

    public void PlayAttack()
    {
        throw new System.Exception("Attack animation not implemented in Animator.");
    }
}
