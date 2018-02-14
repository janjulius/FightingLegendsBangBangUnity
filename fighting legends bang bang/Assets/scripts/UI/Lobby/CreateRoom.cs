using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoom : MonoBehaviour
{
    [SerializeField] private Text roomName;

    public void OnClickCreateRoom()
    {
        RoomOptions options = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 4 };

        string nam = "ROOM#" + Random.Range(1000, 9999);
        if (PhotonNetwork.CreateRoom(nam, options, TypedLobby.Default))
        {
            print("create room send succesfully: " + nam);
        }
        else
        {
            print("Create room send failed");
        }
    }

    private void OnPhotonCreateRoomFailed(object[] codeAndMessage)
    {
        print("create room failed: " + codeAndMessage[1]);
    }

    private void OnCreatedRoom()
    {
        print("Room created successfully");
    }

}
