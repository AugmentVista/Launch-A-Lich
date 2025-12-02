using System.Collections;
using UnityEngine;

public class Player_DarkWizard_Anim_Manager : MonoBehaviour
{
    private Animator animator;
    private bool dead = true;

    private bool animationLocked = false;
    private float lockTimer = 0f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
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
        PlayerStateMachine.OnFlying += Alive;
        PlayerStateMachine.OnGrounded += EvaluateFlyingState;
        PlayerStateMachine.OnGrounded += Alive;
    }

    private void OnDisable()
    {
        PlayerStateMachine.OnReadyToLaunch -= PlayIdle;
        PlayerStateMachine.OnStopped -= PlayDeath;
        PlayerStateMachine.OnFlying -= EvaluateFlyingState;
        PlayerStateMachine.OnFlying -= Alive;
        PlayerStateMachine.OnGrounded -= EvaluateFlyingState;
        PlayerStateMachine.OnGrounded -= Alive;
    }

    public void Alive()
    {
        dead = false;
    }

    private void Update()
    {
        if (animationLocked)
        {
            lockTimer -= Time.deltaTime;
            if (lockTimer <= 0f)
                animationLocked = false;

            return; // skip flight animation when locked
        }

        // Only run flying logic if NOT attacking or hit-stunned
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
            PlayIfNotPlaying("WizardDark_Jump_Anim");
        else if (velocityY < -0.01f)
            PlayIfNotPlaying("WizardDark_Fall_Anim");
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
        animator.Play("WizardDark_Idle_Anim");
    }

    public void PlayRolling()
    {
        StartCoroutine(PlayAndLockRoutine("WizardDark_Jump_Anim"));
    }

    public void PlayTakeHitBig()
    {
        if (dead || Time.timeScale == 0) return;

        LockAnimationNow();
        StartCoroutine(PlayPriorityAnimation("WizardDark_TakeHitBig_Anim"));
    }

    public void PlayTakeHitSmall()
    {
        if (dead || Time.timeScale == 0) return;

        LockAnimationNow();
        StartCoroutine(PlayPriorityAnimation("WizardDark_TakeHitSmall_Anim"));
    }

    public void PlayDeath()
    {
        dead = true;
        StartCoroutine(PlayAndLockRoutine("WizardDark_Death_Anim"));
    }

    public void PlayAttackDown()
    {
        if (dead || Time.timeScale == 0) return;

        LockAnimationNow();
        StartCoroutine(PlayPriorityAnimation("WizardDark_AttackDOWN_Anim"));
    }

    public void PlayAttackUp()
    {
        if (dead || Time.timeScale == 0) return;

        LockAnimationNow();
        StartCoroutine(PlayPriorityAnimation("WizardDark_AttackUP_Anim"));
    }

    private void LockAnimationNow()
    {
        animationLocked = true;
        lockTimer = 999f;  // placeholder, will be corrected in coroutine
    }

    private IEnumerator PlayPriorityAnimation(string anim)
    {
        animator.Play(anim);

        yield return null; // let animator switch states this frame

        float clip = animator.GetCurrentAnimatorStateInfo(0).length;

        lockTimer = clip;
    }
}