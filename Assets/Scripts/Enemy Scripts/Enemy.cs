using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject deathPrefab;
    public GameObject explodedPrefab;

    public bool hitByPlayerAbility = false;
    public bool isDead = false;

    [SerializeField]private float moveSpeed = 15f;

    public int damageValue;
    public int moneyValue;

    public enum Type {Flying, Grounded };
    public Type type;


    void Update()
    {
        if (!isDead)
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime, Space.World);
        }
        else if (isDead)
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