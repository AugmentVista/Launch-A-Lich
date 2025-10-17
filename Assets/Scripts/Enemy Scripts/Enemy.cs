using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float moveSpeed;
    public bool isDead = false;

    public int damageValue;
    public enum Type {Flying, Grounded };
    public Type type;

    public GameObject deathPrefab;

    private void Start()
    {
        moveSpeed = Random.Range(5f, 15f);
    }

    void Update()
    {
        if (!isDead)
        {
            
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
        }
        if (isDead)
        {
            Die();
        }
    }

    public void Die()
    {
        if (deathPrefab != null)
        {
            Instantiate(deathPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}