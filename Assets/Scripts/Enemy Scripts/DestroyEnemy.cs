using UnityEngine;

public class DestroyEnemy : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Item"))
        {
            Destroy(other.gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Item"))
        {
            Destroy(other.gameObject);
        }
    }
}
