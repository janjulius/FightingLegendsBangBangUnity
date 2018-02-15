using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListing : MonoBehaviour
{

    public PhotonPlayer photonPlayer;

    [SerializeField] private Text playerName;
    [SerializeField] private Image backGround;
    [SerializeField] private Image charImage;

    public void ApplyPhotonPlayer(PhotonPlayer phoPlayer)
    {
        photonPlayer = phoPlayer;
        if (phoPlayer.IsMasterClient)
            playerName.text = string.Format("{0} - HOST", phoPlayer.NickName);
        else
            playerName.text = string.Format("{0}", phoPlayer.NickName);
    }

    public void UpdateUI()
    {
        print("uhhhh");

        Debug.Log(photonPlayer.CustomProperties["charId"]);
        charImage.sprite = GameManager.Instance.CharacterHeads[(int)photonPlayer.CustomProperties["charId"]];
        Color c = new Color((float)photonPlayer.CustomProperties["pColorR"], (float)photonPlayer.CustomProperties["pColorG"], (float)photonPlayer.CustomProperties["pColorB"]);
        backGround.color = c;
    }
}
