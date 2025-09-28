using UnityEngine;

public class AbilityEffect : MonoBehaviour
{
    private Animator animator;
    public float Force = 50f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        float clipLength = animator.GetCurrentAnimatorStateInfo(0).length;
        Destroy(gameObject, clipLength);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                // Combine upward and rightward force into one vector
                Vector2 force = (Vector2.up * Force*2) + (Vector2.right * Force);

                // Apply the force
                playerRb.AddForce(force, ForceMode2D.Impulse);

                // Destroy this ability prefab after applying effect
                Destroy(gameObject);
            }
        }
    }
}
