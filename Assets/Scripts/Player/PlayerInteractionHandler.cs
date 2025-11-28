using System;
using UnityEngine;

public class PlayerInteractionHandler : PlayerBase
{
    #region Variable Declarations
    [SerializeField] Player_DarkWizard_Anim_Manager playerAnim;

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
        DetectPlayerAttacking();
        ApplyTouchingGroundDamage();
    }

    public void DetectPlayerAttacking()
    {
        if (Input.GetMouseButtonDown(1))
        {
            playerAnim.PlayAttackDown();
        }
        else if (Input.GetMouseButtonDown(0))
        {
            playerAnim.PlayAttackUp();
        }
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

    public void EnemySlayed(Enemy enemy)
    {
        if (!healthPositive) { return; }

        OnFlyingEnemyDefeated?.Invoke(enemy.moneyValue);
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

            PlayerAbility playerAbility = GetComponentInChildren<PlayerAbility>();
            if (playerAbility != null) { playerAbility.AddMana(item.supportValue); }

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

            if (Health <= MaxHealth && Health > 0)
            {
                NegativeLogarithmicBounce(4f, enemy.damageValue * 2f);

                OnFlyingEnemyDefeated?.Invoke(enemy.moneyValue);
            }
            if (Health <= 0) // if this hit would kill player, downward spike
            {
                { NegativeLogarithmicBounce(2f, 50f); }
            }

            if (Health <= 0) { playerAnim.PlayDeath(); }
            else { playerAnim.PlayTakeHitSmall(); }

            Debug.LogWarning("HITTING ENEMY DAMAGE");

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
                    ApplyExp2Force(5f, ground.damageValue + ApplyBounceyUpgrade());
                    break;

                case int i when (i < MaxHealth && i >= MaxHealth * 0.5f):
                    ApplyExp2Force(6f, ground.damageValue + ApplyBounceyUpgrade());
                    break;

                case int i when (i < MaxHealth * 0.5f && i >= MaxHealth * 0.25f):
                    ApplyExp2Force(7f, ground.damageValue + ApplyBounceyUpgrade());
                    break;

                case int i when (i < MaxHealth * 0.25f && i >= 0f):
                    ApplyExp2Force(8f, ground.damageValue + ApplyBounceyUpgrade());
                    break;
            }
            if (Health <= 0) { playerAnim.PlayDeath(); }
            else { playerAnim.PlayTakeHitBig(); }
        }

        if (collision.CompareTag("Ceiling"))
        {
            ground = collision.GetComponent<Ground>();

            TakeDamage(ground.damageValue);

            if (!healthPositive) { return; }

            switch (Health)
            {
                case int i when (i < MaxHealth && i >= MaxHealth * 0.75f):
                    NegativeLogarithmicBounce(2f , Mathf.Min(Mathf.Abs(playerRb.linearVelocityY) * 0.60f, 10f));
                    break;

                case int i when (i < MaxHealth && i >= MaxHealth * 0.5f):
                    NegativeLogarithmicBounce(2f , Mathf.Min(Mathf.Abs(playerRb.linearVelocityY) * 0.70f, 15f));
                    break;

                case int i when (i < MaxHealth * 0.5f && i >= MaxHealth * 0.25f):
                    NegativeLogarithmicBounce(2f , Mathf.Min(Mathf.Abs(playerRb.linearVelocityY) * 0.80f, 20f));
                    break;

                case int i when (i < MaxHealth * 0.25f && i >= 0f):
                    NegativeLogarithmicBounce(2f , Mathf.Min(Mathf.Abs(playerRb.linearVelocityY) * 0.90f, 20f));
                    break;
            }
            if (Health <= 0) { playerAnim.PlayDeath(); }
            else { playerAnim.PlayTakeHitBig(); }

        }
#endregion
    }

    #region Prevent player from skidding across ground

    private void ApplyTouchingGroundDamage()
    {
        if (grounded && PlayerResultsManager.globalPlayerSpeedX > 4f && healthPositive)
        {
            groundedTimer += Time.deltaTime;

            if (groundedTimer > 0.5f)
            {
                groundedTimer = 0f;

                TakeDamage(ground.damageValue);

                ApplyExp2Force(2f, ground.damageValue + ApplyBounceyUpgrade());

                if (Health <= 0) { playerAnim.PlayDeath(); }
                else { playerAnim.PlayTakeHitBig(); }
                Debug.LogWarning("Touching ground damage");
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") || collision.CompareTag("Ceiling"))
        {
            ground = collision.GetComponent<Ground>();
            grounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") || collision.CompareTag("Ceiling"))
        {
            grounded = false;
        }
    }
    #endregion
}