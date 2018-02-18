using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentGameManager : MonoBehaviour
{

    public static CurrentGameManager Instance;
    private PhotonView phoView;

    private void Awake()
    {
        Instance = this;
        phoView = GetComponent<PhotonView>();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!PhotonNetwork.isMasterClient)
            {
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.LoadLevel(2);
            }
            else
            {
                phoView.RPC("RPC_ReturnToRoom", PhotonTargets.All);
            }
        }
    }

    [PunRPC]
    public void RPC_ReturnToRoom()
    {
        PhotonNetwork.LoadLevel(2);
    }
}
