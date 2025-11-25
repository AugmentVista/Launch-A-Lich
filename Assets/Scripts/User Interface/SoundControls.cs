using UnityEngine;
using UnityEngine.UI;

public class SoundControls : MonoBehaviour
{
    public GlobalSoundManager soundManager;

    public Slider sliderMusic;
    public Slider sliderSFX;


    void Update()
    {
        soundManager.musicVolume = sliderMusic.value;
        soundManager.playerVolume = sliderSFX.value;
    }
}
