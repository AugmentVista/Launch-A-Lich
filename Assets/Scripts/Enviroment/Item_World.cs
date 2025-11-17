using UnityEngine;

public class Item_World : MonoBehaviour
{
    public TreatPickUp treat;

    public bool isDead = false;

    public int healValue;
    public int moneyValue;
    public enum Type { Flying, Grounded };
    public Type type;

    public enum Tier 
    {
        Candy, // 1
        Donuts,
        Cupcake,
        Cake,
        CheeseCake,
        CakeDeluxe,
        Pie,
        Chocolate,
        Jelly // 9
    };
    public Tier tier;

    public GameObject deathPrefab;

    private void Start()
    {
        int treatIndex = (int)treat.treatType;

        tier = (Tier)treatIndex;
        
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
            // something was supposed to go here?
        }
        Destroy(gameObject);
    }
}