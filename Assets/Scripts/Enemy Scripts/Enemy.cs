using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 10f;
    public bool isDead = false;

    public int damageValue;
    public enum Type {Flying, Grounded };
    public Type type;

    public GameObject deathPrefab;

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