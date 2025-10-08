using UnityEngine;

public class PlayerInteractionHandler : PlayerBase
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if (Health < MaxHealth * 0.8f && enemy.type == Enemy.Type.Flying)
            {
                YBiasNegative(enemy.damageValue);
            }
            if (Health < MaxHealth * 0.8f && enemy.type == Enemy.Type.Grounded)
            {
                XBiasNegative(enemy.damageValue);
            }
            else if (Health > MaxHealth * 0.8f) { Forward(enemy.damageValue); }

            TakeDamage(enemy.damageValue);
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            Ground ground = collision.gameObject.GetComponent<Ground>();

            switch (Health)
            {
                case int i when (i == MaxHealth):
                    {
                        YBiasPositive(ground.damageValue); // A large amount of upward force, small amount of forward force
                                                           // No penalty, first one is free
                    }
                    break;
                case int i when (i < MaxHealth && Mathf.Round(i) >= MaxHealth * 0.75f):
                    {
                        XBiasPositive(ground.damageValue / 2); // Small amount of upward force, medium amount of forward force
                                                               // half penalty
                        Forward(-ground.damageValue);                                        

                    }
                    break;
                case int i when (Mathf.Round(i) < MaxHealth * 0.75f && Mathf.Round(i) >= MaxHealth * 0.5f):
                    {
                        YBiasPositive(ground.damageValue / 2); // Small amount of upward force, very small amount of forward force
                                                               // Major Penalty
                        Forward(-ground.damageValue * 1.25f);

                    }
                    break;
                case int i when (Mathf.Round(i) < MaxHealth * 0.5f):
                    {
                        YBiasPositive(ground.damageValue / 2); // Small amount of upward force, very small amount of forward force
                                                               // Maximum Penalty
                        Forward(-ground.damageValue * 1.5f);
                    }
                    break;
            }

            TakeDamage(ground.damageValue);
        }

    }

}
