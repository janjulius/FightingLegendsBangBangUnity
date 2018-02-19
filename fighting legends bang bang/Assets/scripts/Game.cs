using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    private PlayerBase[] players;
    private Canvas myCanvas;
    private GamePanelContainer gpc;

    //game properties
    private bool gamePaused = false;
    private bool gameStartup = false;

    //game rules
    private bool blockingAllowed = true;

    private double damageRatio = 1.0;
    

    public void Pause()
    {
        gamePaused = !gamePaused;
    }

    public void UpdateInterface()
    {
        
    }

    public void LoadInterface()
    {
        gpc = FindObjectOfType<GamePanelContainer>();
        PhotonPlayer[] photonPlayers = PhotonNetwork.playerList;
        for(int i = 0; i < photonPlayers.Length; i++)
        {
            gpc.PlayerJoinedInterface(photonPlayers[i]);
        }
    }
}
