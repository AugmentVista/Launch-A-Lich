using UnityEngine;

public class PlayerBounce : MonoBehaviour
{
    private AudioSource bounceSound;

    void Start()
    {
        // Get the AudioSource component from the player
        bounceSound = GetComponent<AudioSource>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Play the bounce sound whenever the player collides with something
        if (collision.gameObject.CompareTag("Enemy"))
        { 
            if (bounceSound != null)
            {
                bounceSound.Play();
            }
        }
        
    }
}