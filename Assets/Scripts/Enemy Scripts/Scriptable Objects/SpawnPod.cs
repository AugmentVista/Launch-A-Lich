using Unity.VisualScripting;
using UnityEngine;

public class SpawnPod : MonoBehaviour
{
    public PodData data;

    [Header("Debug")]
    public string debugName;

    [HideInInspector] public float weight;

    [HideInInspector]
    public float cooldownRemaining = 0f;

    public bool IsAvailable => cooldownRemaining <= 0f;

    private void Awake()
    {
        weight = data.baseWeight;
    }
    public void TriggerCooldown()
    {
        cooldownRemaining = data.cooldown;
    }

    void Update()
    {
        if (cooldownRemaining > 0f)
            cooldownRemaining -= Time.deltaTime;
    }
}
