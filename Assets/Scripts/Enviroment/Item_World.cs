using UnityEngine;

public class Item_World : MonoBehaviour
{
    public bool isDead = false;

    public int healValue;
    public int moneyValue;
    public enum Type { Flying, Grounded };
    public Type type;

    public GameObject deathPrefab;

    private void Start()
    {
    }

    void Update()
    {
        if (!isDead)
        {

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