using UnityEngine;

public class PillarFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 4f, -10f);

    public bool playerIsDead = false;

    void Update()
    {
        transform.position = target.position + offset;
    }

}
