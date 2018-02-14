using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListing : MonoBehaviour
{

    public PhotonPlayer photonPlayer;

    [SerializeField] private Text playerName;

    public void ApplyPhotonPlayer(PhotonPlayer phoPlayer)
    {
        photonPlayer = phoPlayer;
        if (phoPlayer.IsMasterClient)
            playerName.text = string.Format("{0} - OWNER", phoPlayer.NickName);
        else
            playerName.text = string.Format("{0}", phoPlayer.NickName);
    }
}
