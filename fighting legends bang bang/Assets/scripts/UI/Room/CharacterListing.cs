using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterListing : MonoBehaviour
{
    public int charId;
    public Text HoverText;

    public void OnClick()
    {
        print("id: " + charId);


        PlayerNetwork.Instance.properties["charId"] = charId;
        PhotonNetwork.player.SetCustomProperties(PlayerNetwork.Instance.properties);


        PlayerNetwork.Instance.photonView.RPC("RPC_UpdateSelection", PhotonTargets.All, PhotonNetwork.player);
    }

    public void OnHover()
    {
        HoverText.text = GameManager.Instance.CharacterData[charId].CharacterName + "\n" +
            GameManager.Instance.GetComponent<Info>().GetCharacterInformation(charId) +"\n"+
                         GameManager.Instance.GetComponent<Info>().GetSpecialAttackInformation(charId);
    }

    public void OnHoverExit()
    {
        HoverText.text = "";
    }
}
