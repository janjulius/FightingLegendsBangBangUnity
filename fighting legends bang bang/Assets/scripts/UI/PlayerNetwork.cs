using System.Collections;
using System.Collections.Generic;
using System.IO;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerNetwork : MonoBehaviour
{

    public static PlayerNetwork Instance;
    public string PlayerName { get; private set; }
    public PhotonView photonView;
    private int playersInGame = 0;
    public ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();

    private void Awake()
    {
        Instance = this;
        photonView = GetComponent<PhotonView>();
        PlayerName = "Player#" + Random.Range(1000, 9999);

        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "test")
        {
            playersInGame = 0;
            if (PhotonNetwork.isMasterClient)
                MasterLoadedGame();
            else
                NonMasterLoadedGame();
        }
    }

    private void MasterLoadedGame()
    {
        photonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient);
        photonView.RPC("RPC_LoadGameOthers", PhotonTargets.Others);
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
        PhotonNetwork.LoadLevel(1);
    }


    [PunRPC]
    private void RPC_LoadGameOthers()
    {
        PhotonNetwork.LoadLevel(2);
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
        GetComponent<Game>().LoadInterface();

        Debug.Log("!char id: "+ p.CustomProperties["charId"]);

        string pre = GameManager.Instance.charPrefabs[(int) p.CustomProperties["charId"]];
        Debug.Log(pre);

        Vector3 spawn = new Vector3(0, 5, 0);
        GameObject obj = PhotonNetwork.Instantiate("TestPlayer", spawn, Quaternion.identity, 0);
        PlayerBase pBase = obj.GetComponent<PlayerBase>();
    }

    [PunRPC]
    public void RPC_UpdateSelection(PhotonPlayer plr)
    {
        PlayerListing pl = MainCanvasManager.Instance.CurrentRoomCanvas.PlayerLayoutGroup.playerListings.Find(x => x.photonPlayer == plr);
        if(pl)
            pl.UpdateUI();
    }
}
