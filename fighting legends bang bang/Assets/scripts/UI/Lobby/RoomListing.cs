using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListing : MonoBehaviour
{

    [SerializeField] private Text roomNameText;
    public bool Updated;

    public string RoomName;

    private void Start()
    {
        GameObject lobbyCanvasObj = MainCanvasManager.Instance.lobbyCanvas.gameObject;
        if(lobbyCanvasObj == null)
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

    public void SetRoomNameText(string text)
    {
        RoomName = text;
        roomNameText.text = RoomName;
    }
}
