using UnityEngine;

public class SpawnPod : MonoBehaviour
{
    public PodData data;

    [Header("Debug")]
    public string debugName;

    [HideInInspector] public float weight;

    [HideInInspector]
    public float cooldownRemaining = 0f;

    [SerializeField]private bool touchingGrass = false;

    public bool IsAvailable => cooldownRemaining <= 0f;

    private void Awake()
    {
        weight = data.baseWeight;
    }

    public void TriggerCooldown()
    {
        cooldownRemaining = data.cooldown;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ceiling") || collision.CompareTag("Ground"))
        {
            touchingGrass = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ceiling") || collision.CompareTag("Ground"))
        {
            touchingGrass = false;
        }
    }


    void Update()
    {
        if (touchingGrass)
        {
            cooldownRemaining = data.cooldown;
            return;
        }

        if (cooldownRemaining > 0f)
            cooldownRemaining -= Time.deltaTime;
    }
}
