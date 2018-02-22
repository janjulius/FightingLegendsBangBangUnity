using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusic : MonoBehaviour
{

    public AudioClip MusicClip;

    void Start()
    {
        AudioManager.Instance.PlayMusic(MusicClip);
    }

    void OnDestroy()
    {
        AudioManager.Instance.StopMusic();
    }
}
