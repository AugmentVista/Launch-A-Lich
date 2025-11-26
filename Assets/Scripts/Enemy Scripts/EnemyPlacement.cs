using UnityEngine;

public class EnemyPlacement : MonoBehaviour
{
    [SerializeField] EnemySpawner spawner;

    public Transform[] spawnPods;

    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(55f, 5f, 0f);

    void Update()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
            transform.rotation = Quaternion.identity;
        }
    }
}
