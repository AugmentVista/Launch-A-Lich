using UnityEngine;

public enum PodZone
{
    Red,
    Green,
    Yellow,
    Blue
}


[CreateAssetMenu(fileName = "NewPod", menuName = "Enemy Spawning/Pod Data")]
public class PodData : ScriptableObject
{
    public PodZone zone;
    public float baseWeight = 1f;
    public float cooldown = 0.5f;
}