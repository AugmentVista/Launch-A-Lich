using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [Header("Sound Effects")]

    [SerializeField] private AudioClip groundHitSound;
    [SerializeField] private AudioClip enemyHitSound;
    [SerializeField] private AudioClip abilityHitSound;


    [Header("Volume Control")]
    [Range(0f, 1f)] public float volume;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            PlaySFX(groundHitSound);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            PlaySFX(enemyHitSound);
        }
        else if (collision.gameObject.CompareTag("P_Ability"))
        {
            PlaySFX(abilityHitSound);
        }
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }
}