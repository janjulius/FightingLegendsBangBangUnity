using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoomCanvas : MonoBehaviour
{

    public InputField roomName;

    public InputField maxPlayers;

    public void OnClickCreateRoom()
    {
        string rName = roomName.text;
        byte mPlayers;
        if (!byte.TryParse(maxPlayers.text, out mPlayers))
            mPlayers = 2;

        if (rName == "")
            rName = "ROOM#" + Random.Range(1000, 9999);

        RoomOptions options = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = mPlayers };

        if (PhotonNetwork.CreateRoom(rName, options, TypedLobby.Default))
        {
            print("create room send succesfully: " + rName);
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
        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();

        properties["Owner"] = PhotonNetwork.player.NickName;

        PhotonNetwork.room.SetCustomProperties(properties);

        print(PhotonNetwork.room.CustomProperties["Owner"]);
    }

    public void OnClickCancel()
    {
        MainCanvasManager.Instance.lobbyCanvas.transform.SetAsLastSibling();
    }

    public void OnMaxChange()
    {
        int mPlayers = 0;
        if (int.TryParse(maxPlayers.text,out mPlayers))
        {

            mPlayers = Mathf.Clamp(mPlayers, 2, 8);

            maxPlayers.text = mPlayers.ToString();
        }
    }
}
