using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    private PlayerBase[] players;
    private Canvas myCanvas;

    [SerializeField] private GameObject gamePlayerListingObject;

    //game properties
    private bool gamePaused = false;
    private bool gameStartup = false;

    //game rules
    private bool blockingAllowed = true;

    private double damageRatio = 1.0;
    private double[] playerDamageRatio = new []{ 1.0, 1.0,1.0,1.0};
    

    public void Pause()
    {
        gamePaused = !gamePaused;
    }

    public void UpdateInterface()
    {
        
    }

    public void LoadInterface()
    {
        myCanvas = GetComponent<Canvas>();
        PhotonPlayer[] photonPlayers = PhotonNetwork.playerList;
        for(int i = 0; i < photonPlayers.Length; i++)
        {
            PlayerJoinedUpdateInterface(photonPlayers[i]);
        }
    }

    public void PlayerJoinedUpdateInterface(PhotonPlayer player)
    {
        if(player == null)
            return;

        GameObject playerListingObj = Instantiate(gamePlayerListingObject);
        playerListingObj.transform.SetParent(transform, false);

        PlayerListing playerlisting = playerListingObj.GetComponent<PlayerListing>();
        playerlisting.ApplyPhotonPlayer(player);
        
    }
}
