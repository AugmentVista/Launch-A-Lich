using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 5f;
    public bool isDead = false;

    public GameObject deathPrefab;

    void Update()
    {
        if (!isDead)
        {
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
        }
        else if (isDead)
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