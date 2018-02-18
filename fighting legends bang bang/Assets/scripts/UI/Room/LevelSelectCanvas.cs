using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectCanvas : MonoBehaviour
{

    public void OnClickCancel()
    {
        MainCanvasManager.Instance.CurrentRoomCanvas.transform.SetAsLastSibling();
    }

    public void StartMatch(int i)
    {
        bool Ready = true;

        int roomid = GameManager.Instance.GetSceneId(GameManager.Instance.Levels[i]);
        PlayerNetwork.Instance.currentLevel = roomid;

        foreach (PhotonPlayer plr in PhotonNetwork.playerList)
        {
            print(plr.CustomProperties["charId"]);
            if ((int)plr.CustomProperties["charId"] == 0)
                Ready = false;
        }
        print(Ready);

        if (!PhotonNetwork.isMasterClient || !Ready)
            return;

        PhotonNetwork.room.IsOpen = false;
        PhotonNetwork.room.IsVisible = false;
        PhotonNetwork.LoadLevel(roomid);
    }

}
