using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance;
    public AudioSource soundSource;
    public AudioSource musicSource;


    public AudioClip[] sounds;


    [Range(0, 1)]
    public float volumeSound;
    [Range(0, 1)]
    public float volumeMusic;

    [PunRPC]
    public void PlaySound(int clip)
    {
        soundSource.volume = volumeSound;
        soundSource.PlayOneShot(sounds[clip]);
    }

    [PunRPC]
    public void PlayMusic(AudioClip clip)
    {
        musicSource.volume = volumeMusic;
        musicSource.clip = clip;
        musicSource.Play();
    }

    [PunRPC]
    public void StopMusic()
    {
        if (musicSource)
            musicSource.Stop();
    }


    // Use this for initialization
    void Awake()
    {
        Instance = this;
    }
}
