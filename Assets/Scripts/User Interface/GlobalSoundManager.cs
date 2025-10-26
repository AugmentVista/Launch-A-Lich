using UnityEngine;

public class GlobalSoundManager : MonoBehaviour
{
    [SerializeField] private PlayerAudioManager playerAudio;

    [SerializeField] private AudioSource playerAudioSource;

    [SerializeField] private AudioSource backgroundAudioSource;


    [Header("Volume Controls")]
    [SerializeField][Range(0f, 1f)] float playerVolume;
    [SerializeField][Range(0f, 1f)] float backgroundVolume;


    void Update()
    {




        backgroundAudioSource.volume = backgroundVolume;
        playerAudioSource.volume = playerVolume;
    }
}
