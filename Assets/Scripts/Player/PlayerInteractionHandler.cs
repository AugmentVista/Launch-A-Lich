using UnityEngine;

public class PlayerInteractionHandler : PlayerBase
{
    public int health;

    public int forceMult;

    public int enemyImpactVelocityGain;

    private float groundedTimer = 0f;

    [SerializeField] private bool grounded = false;

    private Ground ground;

    private void Awake()
    {
        if (!playerRb) { playerRb = GetComponent<Rigidbody2D>(); }
        Health = MaxHealth;
        health = MaxHealth;
    }

    /// <summary>
    /// A delegate event that other classes can subcribe to
    /// </summary>
    public delegate void EnemyDefeated(int goldValue);

    public static event EnemyDefeated OnFlyingEnemyDefeated;
    public static event EnemyDefeated OnGroundEnemyDefeated;

    public delegate void ItemCollected(int goldValue);

    public static event ItemCollected OnFlyingItemCollected;
    public static event ItemCollected OnGroundItemCollected;


    private void Update()
    {
        health = Health;
        ApplyTouchingGroundDamage();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Respawn"))
        {
            health = MaxHealth;
        }

        if (collision.gameObject.CompareTag("Item"))
        {
            Item_World item = collision.gameObject.GetComponent<Item_World>();

            if (Health < MaxHealth) { TakeDamage(-item.healValue); }
            LogarithmicBounce(4, item.healValue);

            if (item.type == Item_World.Type.Flying)
            {
                OnFlyingItemCollected?.Invoke(item.moneyValue);
            }
            if (item.type == Item_World.Type.Grounded)
            {
                OnGroundItemCollected?.Invoke(item.moneyValue);
            }

            if (item != null) { item.isDead = true; }
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            TakeDamage(enemy.damageValue);

            if (Health < MaxHealth * 0.5f && enemy.type == Enemy.Type.Flying)
            {
                ApplyLog2Force(2f, enemyImpactVelocityGain * forceMult);
                OnFlyingEnemyDefeated?.Invoke(enemy.moneyValue);
            }
            if (Health >= MaxHealth * 0.5f && enemy.type == Enemy.Type.Flying)
            {
                ApplyLog2Force(8f, enemyImpactVelocityGain * (forceMult -1));
                OnFlyingEnemyDefeated?.Invoke(enemy.moneyValue);
            }
            if (Health < MaxHealth * 0.5f && enemy.type == Enemy.Type.Grounded)
            {
                LogarithmicBounce(8f, enemyImpactVelocityGain * forceMult);
                OnGroundEnemyDefeated?.Invoke(enemy.moneyValue);
            }
            if (Health >= MaxHealth * 0.5f && enemy.type == Enemy.Type.Grounded) 
            {
                LogarithmicBounce(4f, enemyImpactVelocityGain * forceMult);
                OnGroundEnemyDefeated?.Invoke(enemy.moneyValue);
            }
            if (Health >= 0)
            {
                LogarithmicBounce(1f, enemyImpactVelocityGain * 0f);
            }
            if (Health - enemy.damageValue > 0) { GetComponent<Player_Anim_Manager>()?.PlayRolling(); }
            Debug.LogWarning("Player hit an enemy");
            

            if (enemy != null) { enemy.isDead = true; }
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            ground = collision.gameObject.GetComponent<Ground>();

            TakeDamage(ground.damageValue);

            switch (Health)
            {
                case int i when (i < MaxHealth && Mathf.Round(i) >= MaxHealth * 0.75f):
                    {
                        ApplyExp2Force(4f, Mathf.Abs(playerRb.linearVelocityY) * 0.95f);
                    }
                    break;
                case int i when (i < MaxHealth && Mathf.Round(i) >= MaxHealth * 0.5f):
                    {
                        ApplyExp2Force(4f, Mathf.Abs(playerRb.linearVelocityY) * 0.9f);
                    }
                    break;
                case int i when (Mathf.Round(i) < MaxHealth * 0.5f && Mathf.Round(i) >= MaxHealth * 0.25f):
                    {
                        LogarithmicBounce(15f, Mathf.Abs(ground.damageValue));
                    }
                    break;
                case int i when (Mathf.Round(i) < MaxHealth * 0.25f && Mathf.Round(i) >= 0f):
                    {
                        ApplyExp2Force(2f, Mathf.Abs(playerRb.linearVelocityY) * 0.8f);
                    }
                    break;
                case int i when (Mathf.Round(i) >= 0):
                    {
                    
                    }
                    break;
            }
            if (Health - ground.damageValue > 0) { GetComponent<Player_Anim_Manager>()?.PlayTakeHit(); }
            Debug.LogWarning("Player hit the ground");
            
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
                TakeDamage(ground.damageValue / 2);
                ApplyExp2Force(2f, ground.damageValue * 2);
            }
        }
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            ground = collision.gameObject.GetComponent<Ground>();

            grounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            ground = collision.gameObject.GetComponent<Ground>();
            grounded = false;
        }
    }

}