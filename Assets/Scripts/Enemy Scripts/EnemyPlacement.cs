using UnityEngine;

public class EnemyPlacement : MonoBehaviour
{
    [SerializeField] EnemySpawner spawner;

    public Transform[] spawnPods;

    [SerializeField] private Transform target;

    void Update()
    {
        if (target != null)
        {
            transform.position = target.position;
            transform.rotation = Quaternion.identity;
        }
    }
}
