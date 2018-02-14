using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListing : MonoBehaviour
{

    [SerializeField] private Text roomNameText;
    [SerializeField] private Text playerNameText;
    [SerializeField] private Text totalPlayersText;
    public bool Updated;

    public string RoomName;

    private void Start()
    {
        GameObject lobbyCanvasObj = MainCanvasManager.Instance.lobbyCanvas.gameObject;
        if (lobbyCanvasObj == null)
            return;

        LobbyCanvas lobbyCanvas = lobbyCanvasObj.GetComponent<LobbyCanvas>();

        Button button = GetComponent<Button>();
        button.onClick.AddListener(() => lobbyCanvas.OnClickJoinRoom(roomNameText.text));

    }

    private void OnDestroy()
    {
        Button button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();

    }

    public void SetRoomNameText(RoomInfo room)
    {
        RoomName = room.Name;
        roomNameText.text = RoomName;

        string roomOwner = "";
        playerNameText.text = string.Format("owner: {0}", roomOwner);

        totalPlayersText.text = string.Format("players: {1}/{0}", room.MaxPlayers, room.PlayerCount);

    }
}
