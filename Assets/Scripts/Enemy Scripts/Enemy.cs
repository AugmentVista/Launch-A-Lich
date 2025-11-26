using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float moveSpeed;
    public bool isDead = false;
    public float baseSpeed;

    public bool customMovement;
    public bool hitByPlayerAbility = false;

    public int damageValue;
    public int moneyValue;
    public enum Type {Flying, Grounded };
    public Type type;

    public GameObject deathPrefab;
    public GameObject explodedPrefab;
    void FixedUpdate()
    {
        if (!isDead && !customMovement)
        {
            float relativeMult = Random.Range(0.5f, 0.8f);
            moveSpeed = (PlayerResultsManager.globalPlayerSpeedX * relativeMult);
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
        }
        if (isDead)
        {
            Die();
        }
    }

    public void Die()
    {
        if (deathPrefab != null && explodedPrefab != null)
        {
            if (hitByPlayerAbility) 
            { 
                Instantiate(explodedPrefab, transform.position, Quaternion.identity); 
            }
            else 
            {
                Instantiate(deathPrefab, transform.position, Quaternion.identity);
            }
        }
        Destroy(gameObject);
    }
}