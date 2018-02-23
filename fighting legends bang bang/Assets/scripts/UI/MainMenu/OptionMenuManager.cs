using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionMenuManager : MonoBehaviour
{
    public Transform MainMenu;
    public Transform optionsVideo;
    public Transform optionsSound;
    public Transform optionsControls;

    public Transform pressButtonOverlay;

    [Header("video")]
    public Toggle toggleFullscreen;
    public Toggle toggleVsync;
    public Dropdown dropdownResses;
    private Resolution[] resses;

    //soundOptions
    [Header("sound options")]
    public Slider musicSound;
    public Slider sfxSound;
    private float maxMusic = 0.1f;
    private float maxSfx = 0.2f;

    [Header("controls")]
    public Text TmoveL;
    public Text TmoveR;
    public Text TmoveU;
    public Text TmoveD;
    public Text TBattack;
    public Text TSattack;
    public Text TBlock;
    public Text TJump;
    private KeyCode pressedButton = KeyCode.None;
    private bool waitingForKey = false;
    private int keyIndex = -1;
    private KeyCode[] keyBindings = new KeyCode[8];

    void Awake()
    {
        //sound
        var music = PlayerPrefs.GetFloat("optionsSMusic", 1);
        var sound = PlayerPrefs.GetFloat("optionsSSound", 1);
        musicSound.value = music;
        sfxSound.value = sound;
        AudioManager.Instance.volumeMusic = maxMusic * musicSound.value;
        AudioManager.Instance.volumeSound = maxSfx * sfxSound.value;

        //keybindings
        pressButtonOverlay.gameObject.SetActive(false);
        SetKeyBindings();

        //video
        dropdownResses.options.Clear();
        print(PlayerPrefs.GetInt("vfull", 1));
        toggleFullscreen.isOn = PlayerPrefs.GetInt("vfull", 1) == 1;
        toggleVsync.isOn = PlayerPrefs.GetInt("VVsync", 1) == 1;
        resses = GetResolutions();
        foreach (Resolution res in resses)
        {
            string ress = res.width + "x" + res.height;
            print(ress);
            dropdownResses.options.Add(new Dropdown.OptionData(ress));
        }
        print(resses.Length);
        print(dropdownResses.options.Count);
        dropdownResses.value = PlayerPrefs.GetInt("vRes", 0);
        OnClickApplyVideoSettings();

    }

    #region Buttons
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

    public void OnClickOptionsControls()
    {
        optionsControls.SetAsLastSibling();
    }
    #endregion

    #region Video

    public void OnClickApplyVideoSettings()
    {
        Resolution res = resses[dropdownResses.value];
        bool full = toggleFullscreen.isOn;
        bool vsync = toggleVsync.isOn;
        PlayerPrefs.SetInt("vfull", toggleFullscreen.isOn ? 1 : 0);
        PlayerPrefs.SetInt("VVsync", toggleVsync.isOn ? 1 : 0);
        PlayerPrefs.SetInt("vRes", dropdownResses.value);
        Screen.SetResolution(res.width, res.height, full);
        QualitySettings.vSyncCount = vsync ? 1 : 0;
    }

    private Resolution[] GetResolutions()
    {
        List<Resolution> resolutions = new List<Resolution>();

        int lastWidth = 0;
        int lastHeight = 0;

        foreach (Resolution res in Screen.resolutions)
        {
            if (res.width != lastWidth || res.height != lastHeight)
            {
                lastHeight = res.height;
                lastWidth = res.width;
                resolutions.Add(res);
            }
        }

        return resolutions.ToArray();
    }

    #endregion

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

    #region Controls

    void OnGUI()
    {
        if (waitingForKey)
        {
            Event e = Event.current;
            if (e.isKey)
            {
                if (e.keyCode != KeyCode.None)
                {
                    Debug.Log("Detected key code: " + e.keyCode);
                    string s = e.keyCode.ToString();
                    PlayerPrefs.SetString("k" + keyIndex, s);
                    SetKeyBindings();
                    waitingForKey = false;
                    keyIndex = -1;
                    pressButtonOverlay.gameObject.SetActive(false);
                }
            }
        }
    }

    public void OnClickTmoveL()
    {
        pressedButton = KeyCode.None;
        waitingForKey = true;
        keyIndex = 0;
        pressButtonOverlay.gameObject.SetActive(true);
        pressButtonOverlay.SetAsLastSibling();
    }
    public void OnClickTmoveR()
    {
        pressedButton = KeyCode.None;
        waitingForKey = true;
        keyIndex = 1;
        pressButtonOverlay.gameObject.SetActive(true);
        pressButtonOverlay.SetAsLastSibling();
    }
    public void OnClickTmoveU()
    {
        pressedButton = KeyCode.None;
        waitingForKey = true;
        keyIndex = 2;
        pressButtonOverlay.gameObject.SetActive(true);
        pressButtonOverlay.SetAsLastSibling();
    }
    public void OnClickTmoveD()
    {
        pressedButton = KeyCode.None;
        waitingForKey = true;
        keyIndex = 3;
        pressButtonOverlay.gameObject.SetActive(true);
        pressButtonOverlay.SetAsLastSibling();
    }
    public void OnClickTBattack()
    {
        pressedButton = KeyCode.None;
        waitingForKey = true;
        keyIndex = 4;
        pressButtonOverlay.gameObject.SetActive(true);
        pressButtonOverlay.SetAsLastSibling();
    }
    public void OnClickTSattack()
    {
        pressedButton = KeyCode.None;
        waitingForKey = true;
        keyIndex = 5;
        pressButtonOverlay.gameObject.SetActive(true);
        pressButtonOverlay.SetAsLastSibling();
    }
    public void OnClickTBlock()
    {
        pressedButton = KeyCode.None;
        waitingForKey = true;
        keyIndex = 6;
        pressButtonOverlay.gameObject.SetActive(true);
        pressButtonOverlay.SetAsLastSibling();
    }
    public void OnClickTJump()
    {
        pressedButton = KeyCode.None;
        waitingForKey = true;
        keyIndex = 7;
        pressButtonOverlay.gameObject.SetActive(true);
        pressButtonOverlay.SetAsLastSibling();
    }

    public void SetKeyBindings()
    {
        var kMoveL = PlayerPrefs.GetString("k0", KeyCode.LeftArrow.ToString());
        var kMoveR = PlayerPrefs.GetString("k1", KeyCode.RightArrow.ToString());
        var kMoveU = PlayerPrefs.GetString("k2", KeyCode.UpArrow.ToString());
        var kMoveD = PlayerPrefs.GetString("k3", KeyCode.DownArrow.ToString());
        var kBattack = PlayerPrefs.GetString("k4", KeyCode.X.ToString());
        var kSattack = PlayerPrefs.GetString("k5", KeyCode.C.ToString());
        var kblock = PlayerPrefs.GetString("k6", KeyCode.Z.ToString());
        var kJump = PlayerPrefs.GetString("k7", KeyCode.Space.ToString());

        TmoveL.text = "move left: " + kMoveL;
        TmoveR.text = "move right: " + kMoveR;
        TmoveU.text = "look up: " + kMoveU;
        TmoveD.text = "look down: " + kMoveD;
        TBattack.text = "light attack: " + kBattack;
        TSattack.text = "special attack: " + kSattack;
        TBlock.text = "block: " + kblock;
        TJump.text = "jump: " + kJump;

        keyBindings[0] = (KeyCode)System.Enum.Parse(typeof(KeyCode), kMoveL);
        keyBindings[1] = (KeyCode)System.Enum.Parse(typeof(KeyCode), kMoveR);
        keyBindings[2] = (KeyCode)System.Enum.Parse(typeof(KeyCode), kMoveU);
        keyBindings[3] = (KeyCode)System.Enum.Parse(typeof(KeyCode), kMoveD);
        keyBindings[4] = (KeyCode)System.Enum.Parse(typeof(KeyCode), kBattack);
        keyBindings[5] = (KeyCode)System.Enum.Parse(typeof(KeyCode), kSattack);
        keyBindings[6] = (KeyCode)System.Enum.Parse(typeof(KeyCode), kblock);
        keyBindings[7] = (KeyCode)System.Enum.Parse(typeof(KeyCode), kJump);

        PlayerNetwork.Instance.keyBindings = keyBindings;
    }

    #endregion

}
