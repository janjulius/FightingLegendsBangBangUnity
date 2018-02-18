using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCanvas : MonoBehaviour
{

    public RoomLayoutGroup roomLayoutGroup;

    public void OnClickJoinRoom(string roomName)
    {
        print("joining");
        if (PhotonNetwork.JoinRoom(roomName))
        {

        }
        else
        {
            print("joining failed");
        }
    }

    public void OnClickLeaveLobby()
    {
        PhotonNetwork.LoadLevel(GameManager.Instance.GetSceneId("mainmenu"));
    }
}
