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
    void Update()
    {
        if (!isDead)
        {
            float relativeMult = Random.Range(0.5f, 0.8f);
            moveSpeed = Mathf.Max(baseSpeed, PlayerResultsManager.globalPlayerSpeedX * relativeMult);
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