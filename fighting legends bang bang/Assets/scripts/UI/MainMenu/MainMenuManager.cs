using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{

    public Text playerText;
    public InputField input;

    public Transform optionsMenu;

    public void Awake()
    {
        string name = PlayerPrefs.GetString("playerName", "");
        DiscordController.discord.InMenus();
        input.text = name;
    }


    public void OnClickSinglePlayer()
    {

    }

    public void OnClickMultiPlayer()
    {
        string name = playerText.text;

        if (name == "" || name.Length < 3)
            name = "Player#" + Random.Range(1000, 9999);

        PlayerPrefs.SetString("playerName", name);

        PlayerNetwork.Instance.PlayerName = name;

        PhotonNetwork.playerName = name;

        PhotonNetwork.LoadLevel(2);
    }

    public void OnClickOptions()
    {
        optionsMenu.SetAsLastSibling();
    }

    public void OnClickExit()
    {
        Application.Quit();
    }
}
