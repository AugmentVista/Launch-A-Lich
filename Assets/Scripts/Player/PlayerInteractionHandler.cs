using UnityEngine;

public class PlayerInteractionHandler : PlayerBase
{
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

    public delegate void ItemCollected(int goldValue);
    public static event ItemCollected OnFlyingItemCollected;
    public static event ItemCollected OnGroundItemCollected;

    private void Awake()
    {
        if (!playerRb)
            playerRb = GetComponent<Rigidbody2D>();

        ResetHealth();
    }

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

    private void Update()
    {
        ApplyTouchingGroundDamage();
    }

    public void UpgradeBounce(float improvementMod, float purchaseCount)
    {
        bouncyUpgradesCount = purchaseCount;
        bouncyUpgradeValue = improvementMod;
    }

    [SerializeField] private float ApplyBounceyUpgrade()
    {
        if (bouncyUpgradesCount > 0)
            bouncyUpgradesActive = bouncyUpgradesCount * bouncyUpgradeValue;
        else
            bouncyUpgradesActive = 0;

        return bouncyUpgradesActive;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Respawn"))
        {
            ResetHealth();
        }

        if (collision.CompareTag("Item"))
        {
            Item_World item = collision.GetComponent<Item_World>();

            LogarithmicBounce(4, item.healValue);

            if (item.type == Item_World.Type.Flying)
                OnFlyingItemCollected?.Invoke(item.moneyValue);
            else if (item.type == Item_World.Type.Grounded)
                OnGroundItemCollected?.Invoke(item.moneyValue);

            if (item != null) item.isDead = true;
        }

        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            TakeDamage(enemy.damageValue);

            if (Health < MaxHealth * 0.5f && enemy.type == Enemy.Type.Flying)
            {
                //if (PlayerResultsManager.globalPlayerSpeedY > 0.01 && transform.position.y > 20)
                //{
                //    NegativeLogarithmicBounce(15f, enemyImpactVelocityGain * forceMult);
                //}
                //else
                {
                    LogarithmicBounce(4f, enemyImpactVelocityGain * forceMult);
                }
                
                OnFlyingEnemyDefeated?.Invoke(enemy.moneyValue);
            }
            else if (Health >= MaxHealth * 0.5f && Health > 0 && enemy.type == Enemy.Type.Flying)
            {
                //if (PlayerResultsManager.globalPlayerSpeedY > 0.01)
                //{
                //    NegativeLogarithmicBounce(2f, enemyImpactVelocityGain * forceMult);
                //}
                //else if (PlayerResultsManager.globalPlayerSpeedY < -0.01)
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

        if (collision.CompareTag("Ground"))
        {
            ground = collision.GetComponent<Ground>();
            TakeDamage(ground.damageValue);

            switch (Health)
            {
                case int i when (i < MaxHealth && i >= MaxHealth * 0.75f):
                    ApplyExp2Force(5f, ground.damageValue + Mathf.Abs(playerRb.linearVelocityY)  + ApplyBounceyUpgrade());
                    break;

                case int i when (i < MaxHealth && i >= MaxHealth * 0.5f):
                    ApplyExp2Force(6f, ground.damageValue + Mathf.Abs(playerRb.linearVelocityY)  + ApplyBounceyUpgrade());
                    break;

                case int i when (i < MaxHealth * 0.5f && i >= MaxHealth * 0.25f):
                    ApplyExp2Force(7f, ground.damageValue + Mathf.Abs(playerRb.linearVelocityY)  + ApplyBounceyUpgrade());
                    break;

                case int i when (i < MaxHealth * 0.25f && i >= 0f):
                    ApplyExp2Force(8f, ground.damageValue + Mathf.Abs(playerRb.linearVelocityY)  + ApplyBounceyUpgrade());
                    break;
            }
            if (Health <= 0) { playerAnim.PlayDeath(); }
            else { playerAnim.PlayTakeHit(); }

        }

        if (collision.CompareTag("Celling"))
        {
            ground = collision.GetComponent<Ground>();
            TakeDamage(ground.damageValue);

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
    }

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
}