using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySystem : MonoBehaviour
{
    [Header("DEBUG SETTINGS")]
    public bool DEBUGMODE;
    public int DEBUGCHARACTER;
    public int DEBUGLEVEL;
    public string DEBUGCHARACTERNAME;

    void Awake()
    {
        DontDestroyOnLoad(this);
        if (!DEBUGMODE)
            PhotonNetwork.LoadLevel(1);
    }

    // Use this for initialization
    void Start()
    {
        if (!PhotonNetwork.connected)
        {
            print("connecting to server...");
            PhotonNetwork.ConnectUsingSettings("1");
            PhotonNetwork.sendRate = 60;
            PhotonNetwork.sendRateOnSerialize = 60;
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

        if (!PhotonNetwork.inRoom && MainCanvasManager.Instance)
            MainCanvasManager.Instance.lobbyCanvas.transform.SetAsLastSibling();


        if (DEBUGMODE)
        {
            string rName = "DEBUG ROOM#" + Random.Range(1000, 9999);

            RoomOptions options = new RoomOptions() { IsVisible = true, IsOpen = false, MaxPlayers = 1 };

            PhotonNetwork.CreateRoom(rName, options, TypedLobby.Default);
        }
    }

    private void OnCreatedRoom()
    {
        if (DEBUGMODE)
        {
            KeyCode[] keyBindings = new KeyCode[8];

            var kMoveL = PlayerPrefs.GetString("k0", KeyCode.LeftArrow.ToString());
            var kMoveR = PlayerPrefs.GetString("k1", KeyCode.RightArrow.ToString());
            var kMoveU = PlayerPrefs.GetString("k2", KeyCode.UpArrow.ToString());
            var kMoveD = PlayerPrefs.GetString("k3", KeyCode.DownArrow.ToString());
            var kBattack = PlayerPrefs.GetString("k4", KeyCode.X.ToString());
            var kSattack = PlayerPrefs.GetString("k5", KeyCode.C.ToString());
            var kblock = PlayerPrefs.GetString("k6", KeyCode.Z.ToString());
            var kJump = PlayerPrefs.GetString("k7", KeyCode.Space.ToString());

            keyBindings[0] = (KeyCode)System.Enum.Parse(typeof(KeyCode), kMoveL);
            keyBindings[1] = (KeyCode)System.Enum.Parse(typeof(KeyCode), kMoveR);
            keyBindings[2] = (KeyCode)System.Enum.Parse(typeof(KeyCode), kMoveU);
            keyBindings[3] = (KeyCode)System.Enum.Parse(typeof(KeyCode), kMoveD);
            keyBindings[4] = (KeyCode)System.Enum.Parse(typeof(KeyCode), kBattack);
            keyBindings[5] = (KeyCode)System.Enum.Parse(typeof(KeyCode), kSattack);
            keyBindings[6] = (KeyCode)System.Enum.Parse(typeof(KeyCode), kblock);
            keyBindings[7] = (KeyCode)System.Enum.Parse(typeof(KeyCode), kJump);

            PlayerNetwork.Instance.keyBindings = keyBindings;

            PlayerNetwork.Instance.PlayerName = DEBUGCHARACTERNAME;

            PhotonNetwork.playerName = DEBUGCHARACTERNAME;

            Color randomc = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

            PlayerNetwork.Instance.properties["pColorR"] = randomc.r;
            PlayerNetwork.Instance.properties["pColorG"] = randomc.g;
            PlayerNetwork.Instance.properties["pColorB"] = randomc.b;
            PlayerNetwork.Instance.properties["charId"] = DEBUGCHARACTER;
            PhotonNetwork.player.SetCustomProperties(PlayerNetwork.Instance.properties);

            int roomid = GameManager.Instance.GetSceneId(GameManager.Instance.LevelData[DEBUGLEVEL].LevelFileName);

            PhotonNetwork.LoadLevel(roomid);
        }
    }
}
