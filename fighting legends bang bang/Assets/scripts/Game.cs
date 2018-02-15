using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    private PlayerBase[] players;
    private Canvas myCanvas;
    private GamePanelContainer gpc;

    public GameObject[] characterModels;

    [SerializeField]public Vector3[] characterPositionOffsets = new Vector3[]
    {
        new Vector3(0, -1, 0), 
    };

    [SerializeField] public Vector3[] characterRotationOffsets = new Vector3[]
    {
        new Vector3(0, 0, 0), 
    };

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
        gpc = FindObjectOfType<GamePanelContainer>();
        PhotonPlayer[] photonPlayers = PhotonNetwork.playerList;
        for(int i = 0; i < photonPlayers.Length; i++)
        {
            gpc.PlayerJoinedInterface(photonPlayers[i]);
        }
    }
}
