using UnityEngine;

public class PlayerInteractionHandler : PlayerBase
{
    public int health;

    private void Awake()
    {
        if (!playerRb) { playerRb = GetComponent<Rigidbody2D>(); }
        Health = MaxHealth;
        health = MaxHealth;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if (Health < MaxHealth * 0.8f && enemy.type == Enemy.Type.Flying)
            {
                ApplyLog2Force(4f, enemy.damageValue);
            }
            if (Health >= MaxHealth * 0.8f && enemy.type == Enemy.Type.Flying)
            {
                ApplyLog2Force(4f, enemy.damageValue);
            }
            if (Health < MaxHealth * 0.8f && enemy.type == Enemy.Type.Grounded)
            {
                ApplyExp2Force(4f, enemy.damageValue);
            }
            if (Health >= MaxHealth * 0.8f && enemy.type == Enemy.Type.Grounded) 
            {
                ApplyExp2Force(4f, enemy.damageValue);
            }
            
            Debug.LogWarning("Player hit an enemy");
            TakeDamage(enemy.damageValue); health = Health;

            if (enemy != null)
                enemy.isDead = true;
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            Ground ground = collision.gameObject.GetComponent<Ground>();
            switch (Health)
            {
                case int i when (i == MaxHealth):
                    {
                        ApplyExp2Force(4f, Mathf.Abs(playerRb.linearVelocityY) * 0.95f);
                    }
                    break;
                case int i when (i < MaxHealth && Mathf.Round(i) >= MaxHealth * 0.75f):
                    {
                        ApplyExp2Force(4f, Mathf.Abs(playerRb.linearVelocityY) * 0.9f);
                    }
                    break;
                case int i when (Mathf.Round(i) < MaxHealth * 0.75f && Mathf.Round(i) >= MaxHealth * 0.5f):
                    {
                        ApplyExp2Force(4f, Mathf.Abs(playerRb.linearVelocityY) * 0.85f);
                    }
                    break;
                case int i when (Mathf.Round(i) < MaxHealth * 0.5f):
                    {
                        ApplyExp2Force(4f, Mathf.Abs(playerRb.linearVelocityY) * 0.75f);
                    }
                    break;
            }
            Debug.LogWarning("Player hit the ground");
            TakeDamage(ground.damageValue); health = Health;
        }

    }

}