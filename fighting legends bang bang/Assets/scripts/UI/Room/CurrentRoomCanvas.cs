using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class CurrentRoomCanvas : MonoBehaviour
{
    public void OnClickStartMatch()
    {
        if(!PhotonNetwork.isMasterClient)
            return;
        PhotonNetwork.room.IsOpen = false;
        PhotonNetwork.room.IsVisible = false;
        PhotonNetwork.LoadLevel(1);
    }

}
