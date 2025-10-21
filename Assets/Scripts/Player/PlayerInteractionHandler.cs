using UnityEngine;

public class PlayerInteractionHandler : PlayerBase
{
    public int health;

    public int enemyForceMult;

    private void Awake()
    {
        if (!playerRb) { playerRb = GetComponent<Rigidbody2D>(); }
        Health = MaxHealth;
        health = MaxHealth;
    }

    private void Update()
    {
        health = Health;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Respawn"))
        {
            health = MaxHealth;
        }


        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            //GameObject enemyGameObject = collision.gameObject;

            //BoxCollider2D enemyReboundCollider = enemyGameObject.GetComponent<BoxCollider2D>();

            TakeDamage(enemy.damageValue); health = Health;

            if (Health < MaxHealth * 0.5f && enemy.type == Enemy.Type.Flying)
            {
                ApplyLog2Force(8f, enemy.damageValue * enemyForceMult);
            }
            if (Health >= MaxHealth * 0.5f && enemy.type == Enemy.Type.Flying)
            {
                ApplyLog2Force(2f, enemy.damageValue * enemyForceMult -1);
            }
            if (Health < MaxHealth * 0.5f && enemy.type == Enemy.Type.Grounded)
            {
                ApplyExp2Force(4f, enemy.damageValue * enemyForceMult);
            }
            if (Health >= MaxHealth * 0.5f && enemy.type == Enemy.Type.Grounded) 
            {
                ApplyExp2Force(4f, enemy.damageValue * enemyForceMult -1);
            }

            if (Health - enemy.damageValue > 0) { GetComponent<Player_Anim_Manager>()?.PlayRolling(); }
            Debug.LogWarning("Player hit an enemy");
            

            if (enemy != null) { enemy.isDead = true; }
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            Ground ground = collision.gameObject.GetComponent<Ground>();

            TakeDamage(ground.damageValue); health = Health;

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
                        ApplyExp2Force(4f, Mathf.Abs(ground.damageValue));
                    }
                    break;
                case int i when (Mathf.Round(i) < MaxHealth * 0.25f):
                    {
                        ApplyExp2Force(4f, Mathf.Abs(playerRb.linearVelocityY) * 0.8f);
                    }
                    break;
            }
            if (Health - ground.damageValue > 0) { GetComponent<Player_Anim_Manager>()?.PlayTakeHit(); }
            Debug.LogWarning("Player hit the ground");
            
        }

    }

}