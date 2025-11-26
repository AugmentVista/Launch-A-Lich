using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float moveSpeed;
    public bool isDead = false;
    public float baseSpeed;

    public bool hitByPlayerAbility = false;

    public int damageValue;
    public int moneyValue;
    public enum Type {Flying, Grounded };
    public Type type;

    public GameObject deathPrefab;
    public GameObject explodedPrefab;

    public float targetOffset = 8f;
    public float followStrength = 2f;
    public float maxSpeed = 12f;

    private float smoothSpeed = 0;
    void FixedUpdate()
    {
        if (!isDead )
        {
            RelativeMovement();
        }
        if (isDead)
        {
            Die();
        }
    }

    public void RelativeMovement()
    {
        float playerX = PlayerResultsManager.currentDistance;
        float enemyX = transform.position.x;

        float playerSpeed = Mathf.Abs(PlayerResultsManager.globalPlayerSpeedX);

        float currentOffset = enemyX - playerX;
        float offsetError = targetOffset - currentOffset;

        float desiredSpeed = playerSpeed + offsetError * followStrength;
        desiredSpeed = Mathf.Clamp(desiredSpeed, 0, maxSpeed);

        smoothSpeed = Mathf.Lerp(smoothSpeed, desiredSpeed, Time.deltaTime * 5f);

        transform.Translate(Vector2.right * smoothSpeed * Time.deltaTime);
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