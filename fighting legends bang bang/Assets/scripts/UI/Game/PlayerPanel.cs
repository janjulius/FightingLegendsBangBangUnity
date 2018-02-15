using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour {


    public PhotonPlayer photonPlayer;
    public PlayerBase playerBase;

    [SerializeField] private Text playerName;
    [SerializeField] private Image charImage;
    [SerializeField] private Text damageText;
    [SerializeField] private Text charText;

    public void ApplyPhotonPlayer(PhotonPlayer phoPlayer)
    {
        photonPlayer = phoPlayer;
        playerName.text = phoPlayer.NickName;
        damageText.text = string.Format("{0}%", 0);
    }

    public void UpdateUI()
    {
        damageText.text = string.Format("{0}%", playerBase.healthController.Damage);
    }
}
