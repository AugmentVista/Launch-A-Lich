using UnityEngine;

public class Item_World : MonoBehaviour
{
    public TreatPickUp treat;

    public bool isDead = false;
    private float moveSpeed;
    private float baseSpeed;

    public int supportValue;
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
            float relativeMult = Random.Range(0.5f, 0.8f);
            moveSpeed = Mathf.Max(baseSpeed, PlayerResultsManager.globalPlayerSpeedX * relativeMult);
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
        }
        if (isDead)
        {
            Die();
        }
    }

    public void TierMovementPatterns()
    {
        switch (tier)
        {
            case Tier.Candy:

                break;
            case Tier.Donuts:

                break;
            case Tier.Cupcake:

                break;
            case Tier.Cake:

                break;
            case Tier.CheeseCake:

                break;
            case Tier.CakeDeluxe:

                break;
            case Tier.Pie:

                break;
            case Tier.Chocolate:

                break;
            case Tier.Jelly:

                break;
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