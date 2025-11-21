using System;
using UnityEngine;

public class PlayerInteractionHandler : PlayerBase
{
    #region Variable Declarations
    [SerializeField] Player_Anim_Manager playerAnim;

    public int forceMult;
    public int enemyImpactVelocityGain;

    private float groundedTimer = 0f;

    private float bouncyUpgradesActive = 0;
    public float bouncyUpgradeValue;
    public float bouncyUpgradesCount;

    [SerializeField] private bool grounded = false;
    private Ground ground;

    public delegate void EnemyDefeated(int goldValue);
    public static event EnemyDefeated OnFlyingEnemyDefeated;
    public static event EnemyDefeated OnGroundEnemyDefeated;

    public delegate void ItemCollected(int goldValue, Enum tier);
    public static event ItemCollected OnFlyingItemCollected;
    public static event ItemCollected OnGroundItemCollected;

    #endregion
    private void Awake()
    {
        if (!playerRb)
            playerRb = GetComponent<Rigidbody2D>();

        ResetHealth();
    }

    #region State Subcriptions

    private void OnEnable()
    {
        PlayerStateMachine.OnInactive += Inactive;
        PlayerStateMachine.OnGrounded += Grounded;
        PlayerStateMachine.OnFlying += Flying;
        PlayerStateMachine.OnStopped += Stopped;
        PlayerStateMachine.OnReadyToLaunch += ReadyToLaunch;
    }

    private void OnDisable()
    {
        PlayerStateMachine.OnInactive -= Inactive;
        PlayerStateMachine.OnGrounded -= Grounded;
        PlayerStateMachine.OnFlying -= Flying;
        PlayerStateMachine.OnStopped -= Stopped;
        PlayerStateMachine.OnReadyToLaunch -= ReadyToLaunch;
    }
    void Inactive() { }
    void Grounded() { }
    void Flying() { }
    void Stopped() { TakeDamage(MaxHealth); }
    void ReadyToLaunch() 
    {
        stopCalled = false;
        ResetHealth();
    }

    #endregion

    private void Update()
    {
        ApplyTouchingGroundDamage();
    }

    #region Upgrade Section
    public void UpgradeBounce(float improvementMod, float purchaseCount)
    {
        bouncyUpgradesCount = purchaseCount;
        bouncyUpgradeValue = improvementMod;
    }

    [SerializeField]private float ApplyBounceyUpgrade()
    {
        if (bouncyUpgradesCount > 0)
            bouncyUpgradesActive = bouncyUpgradesCount * bouncyUpgradeValue;
        else
            bouncyUpgradesActive = 0;

        return bouncyUpgradesActive;
    }

    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Respawn"))
        {
            ResetHealth();
        }

        if (collision.CompareTag("Item"))
        {
            if (!healthPositive) { return; }
            Item_World item = collision.GetComponent<Item_World>();

            LogarithmicBounce(4, item.supportValue);

            if (item.type == Item_World.Type.Flying)
            {
                OnFlyingItemCollected?.Invoke(item.moneyValue, item.tier);
            }
            else if (item.type == Item_World.Type.Grounded)
            { 
                OnGroundItemCollected?.Invoke(item.moneyValue, item.tier);
            }

            if (item != null) item.isDead = true;
        }

        #region Enemy Interactions

        if (collision.CompareTag("Enemy"))
        {
            if (!healthPositive) { return; }
            Enemy enemy = collision.GetComponent<Enemy>();

            TakeDamage(enemy.damageValue);

            if (Health < MaxHealth * 0.5f && enemy.type == Enemy.Type.Flying)
            {
                {
                    LogarithmicBounce(4f, enemyImpactVelocityGain * forceMult);
                }
                
                OnFlyingEnemyDefeated?.Invoke(enemy.moneyValue);
            }
            else if (Health >= MaxHealth * 0.5f && Health > 0 && enemy.type == Enemy.Type.Flying)
            {
                {
                    LogarithmicBounce(2f, enemyImpactVelocityGain * forceMult);
                }

                OnFlyingEnemyDefeated?.Invoke(enemy.moneyValue);
            }
            else if (Health < MaxHealth * 0.5f && enemy.type == Enemy.Type.Grounded)
            {
                LogarithmicBounce(4f, enemyImpactVelocityGain * forceMult);
                OnGroundEnemyDefeated?.Invoke(enemy.moneyValue);
            }
            else if (Health >= MaxHealth * 0.5f && Health > 0 && enemy.type == Enemy.Type.Grounded)
            {
                LogarithmicBounce(4f, enemyImpactVelocityGain * forceMult);
                OnGroundEnemyDefeated?.Invoke(enemy.moneyValue);
            }

            if (Health - enemy.damageValue > 0) { GetComponent<Player_Anim_Manager>()?.PlayRolling(); }


            if (Health <= 0) { NegativeLogarithmicBounce(2f, 100f); }

            if (enemy != null) enemy.isDead = true;
        }

        #endregion

        #region Ground & Ceiling Damaging details
        if (collision.CompareTag("Ground"))
        {
            ground = collision.GetComponent<Ground>();
            TakeDamage(ground.damageValue);
            if (!healthPositive) { return; }

            switch (Health)
            {
                case int i when (i < MaxHealth && i >= MaxHealth * 0.75f):
                    ApplyExp2Force(5f, ground.damageValue + Mathf.Abs(playerRb.linearVelocityY)/2  + ApplyBounceyUpgrade());
                    break;

                case int i when (i < MaxHealth && i >= MaxHealth * 0.5f):
                    ApplyExp2Force(6f, ground.damageValue + Mathf.Abs(playerRb.linearVelocityY)/2  + ApplyBounceyUpgrade());
                    break;

                case int i when (i < MaxHealth * 0.5f && i >= MaxHealth * 0.25f):
                    ApplyExp2Force(7f, ground.damageValue + Mathf.Abs(playerRb.linearVelocityY)/2  + ApplyBounceyUpgrade());
                    break;

                case int i when (i < MaxHealth * 0.25f && i >= 0f):
                    ApplyExp2Force(8f, ground.damageValue + Mathf.Abs(playerRb.linearVelocityY)/2  + ApplyBounceyUpgrade());
                    break;
            }
            if (Health <= 0) { playerAnim.PlayDeath(); }
            else { playerAnim.PlayTakeHit(); }

        }

        if (collision.CompareTag("Celling"))
        {
            ground = collision.GetComponent<Ground>();
            TakeDamage(ground.damageValue);
            if (!healthPositive) { return; }
            switch (Health)
            {
                case int i when (i < MaxHealth && i >= MaxHealth * 0.75f):
                    NegativeLogarithmicBounce(2f , Mathf.Abs(playerRb.linearVelocityY) * 0.95f + ApplyBounceyUpgrade());
                    break;

                case int i when (i < MaxHealth && i >= MaxHealth * 0.5f):
                    NegativeLogarithmicBounce(2f , Mathf.Abs(playerRb.linearVelocityY) * 0.90f + ApplyBounceyUpgrade());
                    break;

                case int i when (i < MaxHealth * 0.5f && i >= MaxHealth * 0.25f):
                    NegativeLogarithmicBounce(2f , Mathf.Abs(playerRb.linearVelocityY) * 0.85f + ApplyBounceyUpgrade());
                    break;

                case int i when (i < MaxHealth * 0.25f && i >= 0f):
                    NegativeLogarithmicBounce(2f , Mathf.Abs(playerRb.linearVelocityY) * 0.80f + ApplyBounceyUpgrade());
                    break;
            }
            if (Health <= 0) { playerAnim.PlayDeath(); }
            else { playerAnim.PlayTakeHit(); }

        }
#endregion
    }

    #region Prevent player from skidding across ground

    private void ApplyTouchingGroundDamage()
    {
        if (grounded && PlayerResultsManager.globalPlayerSpeedX > 4f)
        {
            groundedTimer += Time.deltaTime;

            if (groundedTimer > 0.5f)
            {
                groundedTimer = 0f;
                GetComponent<Player_Anim_Manager>()?.PlayTakeHit();
                TakeDamage(ground.damageValue - 10);
                ApplyExp2Force(2f, ground.damageValue * 2 + ApplyBounceyUpgrade());
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            ground = collision.GetComponent<Ground>();
            grounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            grounded = false;
        }
    }
    #endregion
}