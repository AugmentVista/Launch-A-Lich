using UnityEngine;

public class GlobalSoundManager : MonoBehaviour
{
    [SerializeField] private PlayerAudioManager playerAudio;

    [SerializeField] private AudioSource playerAudioSource;

    [SerializeField] private AudioSource backgroundAudioSource;


    [Header("Volume Values")]
    public float playerVolume;
    public float musicVolume;


    void Update()
    {
        backgroundAudioSource.volume = musicVolume;
        playerAudioSource.volume = playerVolume;
    }
}
