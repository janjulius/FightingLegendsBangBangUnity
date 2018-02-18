using System.Collections;
using System.Collections.Generic;
using System.IO;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerNetwork : MonoBehaviour
{

    public static PlayerNetwork Instance;
    public string PlayerName;
    public int currentLevel;
    public PhotonView photonView;
    private int playersInGame = 0;
    public ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();

    private void Awake()
    {
        Instance = this;
        photonView = GetComponent<PhotonView>();

        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {

        switch (scene.name)
        {
            case "lobby":
                print("back to lobby");
                if (PhotonNetwork.inRoom)
                {
                    PhotonNetwork.room.IsOpen = true;
                    PhotonNetwork.room.IsVisible = true;

                    var CRC = MainCanvasManager.Instance.CurrentRoomCanvas;

                    MainCanvasManager.Instance.CurrentRoomCanvas.playerNameText.text = PhotonNetwork.player.NickName;

                    foreach (Transform child in CRC.PlayerLayoutGroup.transform)
                    {
                        Destroy(child.gameObject);
                    }

                    MainCanvasManager.Instance.CurrentRoomCanvas.transform.SetAsLastSibling();

                    PhotonPlayer[] photonPlayers = PhotonNetwork.playerList;
                    for (int i = 0; i < photonPlayers.Length; i++)
                    {
                        CRC.PlayerLayoutGroup.PlayerJoinedRoom(photonPlayers[i]);
                    }

                    PlayerNetwork.Instance.photonView.RPC("RPC_UpdateSelection", PhotonTargets.AllBuffered, PhotonNetwork.player);
                }
                
                MainCanvasManager.Instance.lobbyCanvas.roomLayoutGroup.OnReceivedRoomListUpdate();


                break;

            case "test":
                ScoreManager.Instance.players.Clear();
                GetComponent<Game>().LoadInterface();
                playersInGame = 0;
                if (PhotonNetwork.isMasterClient)
                    MasterLoadedGame();
                else
                    NonMasterLoadedGame();
                break;
        }
    }

    private void MasterLoadedGame()
    {
        photonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient);
        photonView.RPC("RPC_LoadGameOthers", PhotonTargets.Others,currentLevel);
    }

    private void NonMasterLoadedGame()
    {
        photonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient);
    }

    void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        Kicked();
    }


    [PunRPC]
    public void Kicking()
    {
        Kicked();
    }

    public void Kicked()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(GameManager.Instance.GetSceneId("lobby"));
    }


    [PunRPC]
    private void RPC_LoadGameOthers(int lvl)
    {
        PhotonNetwork.LoadLevel(lvl);
    }

    [PunRPC]
    private void RPC_LoadedGameScene()
    {
        playersInGame++;
        if (playersInGame == PhotonNetwork.playerList.Length)
        {
            print("all players are in the game");

            foreach (PhotonPlayer player in PhotonNetwork.playerList)
            {
                photonView.RPC("RPC_CreatePlayer", player, player);
            }

        }
    }

    [PunRPC]
    private void RPC_CreatePlayer(PhotonPlayer p)
    {

        Debug.Log("!char id: " + p.CustomProperties["charId"]);

        string pre = GameManager.Instance.charPrefabs[(int)p.CustomProperties["charId"]];
        Debug.Log(pre);

        Vector3 spawn = new Vector3(0, 5, 0);
        GameObject obj = PhotonNetwork.Instantiate(pre, spawn, Quaternion.identity, 0);
        PlayerBase pBase = obj.GetComponent<PlayerBase>();
    }

    [PunRPC]
    public void RPC_UpdateSelection(PhotonPlayer plr)
    {
        PlayerListing pl = MainCanvasManager.Instance.CurrentRoomCanvas.PlayerLayoutGroup.playerListings.Find(x => x.photonPlayer == plr);
        if (pl)
            pl.UpdateUI();
    }
}
