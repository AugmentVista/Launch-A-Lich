using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float moveSpeed;
    public bool isDead = false;
    public float baseSpeed;

    public int damageValue;
    public int moneyValue;
    public enum Type {Flying, Grounded };
    public Type type;

    public GameObject deathPrefab;

    private void Start()
    {
        moveSpeed = Random.Range(baseSpeed, PlayerResultsManager.globalPlayerSpeedX * 0.8f);
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