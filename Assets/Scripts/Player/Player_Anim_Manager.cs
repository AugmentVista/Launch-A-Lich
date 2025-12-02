using UnityEngine;
using System.Collections;

public class Player_Anim_Manager : MonoBehaviour
{
    private Animator animator;
    private PlayerStateMachine stateMachine;
    private PlayerInteractionHandler interactionHandler;

    private bool animationLocked = false;
    private float lockTimer = 0f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        stateMachine = GetComponent<PlayerStateMachine>();
        interactionHandler = GetComponent<PlayerInteractionHandler>();
    }

    private void Start()
    {
        PlayIdle();
    }

    private void OnEnable()
    {
        PlayerStateMachine.OnReadyToLaunch += PlayIdle;
        PlayerStateMachine.OnStopped += PlayDeath;
        PlayerStateMachine.OnFlying += EvaluateFlyingState;
        PlayerStateMachine.OnGrounded += EvaluateFlyingState;
    }

    private void OnDisable()
    {
        PlayerStateMachine.OnReadyToLaunch -= PlayIdle;
        PlayerStateMachine.OnStopped -= PlayDeath;
        PlayerStateMachine.OnFlying -= EvaluateFlyingState;
        PlayerStateMachine.OnGrounded -= EvaluateFlyingState;
    }

    private void Update()
    {
        if (animationLocked)
        {
            lockTimer -= Time.deltaTime;

            if (lockTimer <= 0f)
                animationLocked = false;

            return;
        }
        // Only evaluate rising/falling if the animation is not locked
        EvaluateFlyingState();
    }

    private IEnumerator PlayAndLockRoutine(string anim)
    {
        animator.Play(anim);

        // Wait 1 frame so animator updates current state
        yield return null;

        float clipLength = animator.GetCurrentAnimatorStateInfo(0).length;

        animationLocked = true;
        lockTimer = clipLength;
    }

    private void EvaluateFlyingState()
    {
        if (animationLocked) return;

        float velocityY = PlayerResultsManager.globalPlayerSpeedY;

        if (velocityY > 0.01f)
            PlayIfNotPlaying("Player_Rising");
        else if (velocityY < -0.01f)
            PlayIfNotPlaying("Player_Falling");
    }

    public void PlayIfNotPlaying(string stateName)
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);

        if (!info.IsName(stateName))
        {
            animator.Play(stateName);
        }
    }

    private void PlayIdle()
    {
        animator.Play("Player_Idle");
    }

    public void PlayRolling()
    {
        StartCoroutine(PlayAndLockRoutine("Player_Rolling"));
        //animator.Play("Player_Rolling");
    }
    public void PlayTakeHit()
    {
        StartCoroutine(PlayAndLockRoutine("Player_Take_Hit"));
        //animator.Play("Player_Take_Hit");
    }

    public void PlayDeath()
    {
        animator.Play("Player_Death");
    }

    public void PlayAttack()
    {
        throw new System.Exception("Attack animation not implemented in Animator.");
    }
}
