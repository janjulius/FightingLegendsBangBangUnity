using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterListing : MonoBehaviour
{
    public int charId;

    public void OnClick()
    {
        print("id: " + charId);


        PlayerNetwork.Instance.properties["charId"] = charId;
        PhotonNetwork.player.SetCustomProperties(PlayerNetwork.Instance.properties);


        PlayerNetwork.Instance.photonView.RPC("RPC_UpdateSelection", PhotonTargets.All, PhotonNetwork.player);
    }
}
