using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySystem : MonoBehaviour
{


    void Awake()
    {
        DontDestroyOnLoad(this);
        PhotonNetwork.LoadLevel(1);
    }

    // Use this for initialization
    void Start()
    {
        if (!PhotonNetwork.connected)
        {
            print("connecting to server...");
            PhotonNetwork.ConnectUsingSettings("1");
            PhotonNetwork.sendRate = 20;
            PhotonNetwork.sendRateOnSerialize = 20;
        }
    }

    private void OnConnectedToMaster()
    {
        print("Connected to master.");
        PhotonNetwork.automaticallySyncScene = false;
        PhotonNetwork.playerName = PlayerNetwork.Instance.PlayerName;

        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    private void OnJoinedLobby()
    {
        print("Joined lobby.");
        GameManager.Instance.Players.Clear();


        if (!PhotonNetwork.inRoom)
            MainCanvasManager.Instance.lobbyCanvas.transform.SetAsLastSibling();
    }
}
