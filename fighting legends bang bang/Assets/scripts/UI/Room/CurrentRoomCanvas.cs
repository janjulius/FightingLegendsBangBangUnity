using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.UI;

public class CurrentRoomCanvas : MonoBehaviour
{
    public PlayerLayoutGroup PlayerLayoutGroup;
    public GameObject startButton;
    public GameObject lockButton;
    public Text playerNameText;

    public void OnClickStartMatch()
    {
        bool Ready = true;

        foreach (PhotonPlayer plr in PhotonNetwork.playerList)
        {
            print(plr.CustomProperties["charId"]);
            if ((int) plr.CustomProperties["charId"] == 0)
                Ready = false;
        }
        print(Ready);

        if (!PhotonNetwork.isMasterClient || !Ready)
            return;

        print("wtf");
        PhotonNetwork.room.IsOpen = false;
        PhotonNetwork.room.IsVisible = false;
        PhotonNetwork.LoadLevel(2);
    }

}
