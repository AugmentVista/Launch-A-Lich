using UnityEngine;
using System.Collections;

public class Player_DarkWizard_Anim_Manager : MonoBehaviour
{
    private Animator animator;

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
        //animator.Play("Player_Rolling");
    }
    public void PlayTakeHit()
    {
        StartCoroutine(PlayAndLockRoutine("WizardDark_TakeDamage_Anim"));
        //animator.Play("Player_Take_Hit");
    }

    public void PlayDeath()
    {
        animator.Play("WizardDark_Death_Anim");
    }

    public void PlayAttack()
    {
        throw new System.Exception("Attack animation not implemented in Animator.");
    }
}