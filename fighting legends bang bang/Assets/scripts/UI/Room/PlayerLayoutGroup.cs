using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLayoutGroup : MonoBehaviour
{
    [SerializeField] private GameObject playerListingGameObject;

    public List<PlayerListing> playerListings = new List<PlayerListing>();

    private void OnJoinedRoom()
    {

        MainCanvasManager.Instance.CurrentRoomCanvas.playerNameText.text = PhotonNetwork.player.NickName;

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        MainCanvasManager.Instance.CurrentRoomCanvas.transform.SetAsLastSibling();

        PhotonPlayer[] photonPlayers = PhotonNetwork.playerList;
        for (int i = 0; i < photonPlayers.Length; i++)
        {
            PlayerJoinedRoom(photonPlayers[i]);
        }

        print(PhotonNetwork.player);

        Color randomc = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

        print("i Joined the lobby");
        PlayerNetwork.Instance.properties["charId"] = 0;
        PlayerNetwork.Instance.properties["pColorR"] = randomc.r;
        PlayerNetwork.Instance.properties["pColorG"] = randomc.g;
        PlayerNetwork.Instance.properties["pColorB"] = randomc.b;
        PhotonNetwork.player.SetCustomProperties(PlayerNetwork.Instance.properties);
        PlayerNetwork.Instance.photonView.RPC("RPC_UpdateSelection", PhotonTargets.AllBuffered, PhotonNetwork.player);
    }

    private void OnPhotonPlayerConnected(PhotonPlayer phoPlayer)
    {
        PlayerJoinedRoom(phoPlayer);
    }

    private void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        PlayerLeftRoom(player);
    }


    internal void PlayerJoinedRoom(PhotonPlayer phoPlayer)
    {

        if (phoPlayer == null)
            return;
        PlayerLeftRoom(phoPlayer);

        GameObject playerListingObj = Instantiate(playerListingGameObject);
        playerListingObj.transform.SetParent(transform, false);

        PlayerListing playerlisting = playerListingObj.GetComponent<PlayerListing>();
        playerlisting.ApplyPhotonPlayer(phoPlayer);

        playerListings.Add(playerlisting);
    }

    private void PlayerLeftRoom(PhotonPlayer phoPlayer)
    {
        int index = playerListings.FindIndex(x => x.photonPlayer == phoPlayer);
        if (index != -1)
        {
            Destroy(playerListings[index].gameObject);
            playerListings.RemoveAt(index);
        }

    }

    public void OnClickRoomState()
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        PhotonNetwork.room.IsOpen = !PhotonNetwork.room.IsOpen;
        PhotonNetwork.room.IsVisible = !PhotonNetwork.room.IsVisible;

        if (!PhotonNetwork.room.IsOpen)
        {
            MainCanvasManager.Instance.CurrentRoomCanvas.lockButton.GetComponentInChildren<Text>().text = "Unlock room";
        }
        else
        {
            MainCanvasManager.Instance.CurrentRoomCanvas.lockButton.GetComponentInChildren<Text>().text = "Lock room";
        }
    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}
