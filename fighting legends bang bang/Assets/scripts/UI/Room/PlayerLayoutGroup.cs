using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLayoutGroup : MonoBehaviour
{
    [SerializeField] private GameObject playerListingGameObject;

    private List<PlayerListing> playerListings = new List<PlayerListing>();

    private void OnJoinedRoom()
    {

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
    }

    private void OnPhotonPlayerConnected(PhotonPlayer phoPlayer)
    {
        PlayerJoinedRoom(phoPlayer);
    }

    private void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        PlayerLeftRoom(player);
    }


    private void PlayerJoinedRoom(PhotonPlayer phoPlayer)
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
    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}
