using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionMenuManager : MonoBehaviour
{
    public Transform MainMenu;
    public Transform optionsVideo;
    public Transform optionsSound;

    //soundOptions
    [Header("sound options")]
    public Slider musicSound;
    public Slider sfxSound;
    private float maxMusic = 0.1f;
    private float maxSfx = 0.2f;

    void Awake()
    {
        //sound
        var music = PlayerPrefs.GetFloat("optionsSMusic", 1);
        var sound = PlayerPrefs.GetFloat("optionsSSound", 1);
        musicSound.value = music;
        sfxSound.value = sound;
        AudioManager.Instance.volumeMusic = maxMusic * musicSound.value;
        AudioManager.Instance.volumeMusic = maxSfx * sfxSound.value;
    }

    public void OnClickLeave()
    {
        MainMenu.SetAsLastSibling();
    }

    public void OnClickOptionsVideo()
    {
        optionsVideo.SetAsLastSibling();
    }

    public void OnClickOptionsSound()
    {
        optionsSound.SetAsLastSibling();
    }

    #region sound

    public void OnMusicChange()
    {
        AudioManager.Instance.volumeMusic = maxMusic * musicSound.value;
        PlayerPrefs.SetFloat("optionsSMusic", musicSound.value);

    }

    public void OnSFXChange()
    {
        AudioManager.Instance.volumeSound = maxSfx * sfxSound.value;
        PlayerPrefs.SetFloat("optionsSSound", sfxSound.value);

    }


    #endregion
}
