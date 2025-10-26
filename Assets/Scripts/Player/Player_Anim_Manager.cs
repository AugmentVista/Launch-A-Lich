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
        PlayerStateMachine.OnFlying += EvaluateFlyingState;
        PlayerStateMachine.OnGrounded += EvaluateFlyingState;
        PlayerStateMachine.OnStopped += PlayDeath;
    }

    private void OnDisable()
    {
        PlayerStateMachine.OnReadyToLaunch -= PlayIdle;
        PlayerStateMachine.OnFlying -= EvaluateFlyingState;
        PlayerStateMachine.OnGrounded -= EvaluateFlyingState;
        PlayerStateMachine.OnStopped -= PlayDeath;
    }

    private void PlayIdle()
    {
        animator.Play("Player_Idle");
    }

    private void EvaluateFlyingState()
    {
        if (PlayerResultsManager.globalPlayerSpeedY > 0)
            animator.Play("Player_Rising");
        else if (PlayerResultsManager.globalPlayerSpeedY < 0)
            animator.Play("Player_Falling");
    }

    public void PlayRolling()
    {
        animator.Play("Player_Rolling");
    }

    private void PlayDeath()
    {
        animator.Play("Player_Death");
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
