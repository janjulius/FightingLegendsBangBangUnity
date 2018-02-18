using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListing : MonoBehaviour
{

    public PhotonPlayer photonPlayer;

    [SerializeField] private Text playerName;
    [SerializeField] private Text charName;
    [SerializeField] private Image backGround;
    [SerializeField] private Image charImage;
    [SerializeField] private GameObject kickButon;
    [SerializeField] private GameObject Crown;

    public void ApplyPhotonPlayer(PhotonPlayer phoPlayer)
    {
        photonPlayer = phoPlayer;
        if (phoPlayer.IsMasterClient)
            Crown.SetActive(true);

        playerName.text = string.Format("{0}", phoPlayer.NickName);

        if (photonPlayer == PhotonNetwork.player)
            playerName.text = "You";
    }

    public void UpdateUI()
    {

        if (PhotonNetwork.isMasterClient)
        {
            if (photonPlayer != PhotonNetwork.player)
                kickButon.SetActive(true);

            MainCanvasManager.Instance.CurrentRoomCanvas.lockButton.SetActive(true);
            bool Ready = true;

            foreach (PhotonPlayer plr in PhotonNetwork.playerList)
            {
                print(plr.CustomProperties["charId"]);
                if ((int)plr.CustomProperties["charId"] == 0)
                    Ready = false;
            }

            MainCanvasManager.Instance.CurrentRoomCanvas.startButton.SetActive(Ready);
        }

        charImage.sprite = GameManager.Instance.CharacterHeads[(int)photonPlayer.CustomProperties["charId"]];
        Color c = new Color((float)photonPlayer.CustomProperties["pColorR"], (float)photonPlayer.CustomProperties["pColorG"], (float)photonPlayer.CustomProperties["pColorB"]);
        backGround.color = c;
        charName.text = GameManager.Instance.charNames[(int)photonPlayer.CustomProperties["charId"]].ToUpper();
    }

    public void OnClick()
    {
        if (PhotonNetwork.player != photonPlayer)
            return;

        Color randomc = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

        PlayerNetwork.Instance.properties["pColorR"] = randomc.r;
        PlayerNetwork.Instance.properties["pColorG"] = randomc.g;
        PlayerNetwork.Instance.properties["pColorB"] = randomc.b;
        PhotonNetwork.player.SetCustomProperties(PlayerNetwork.Instance.properties);
        PlayerNetwork.Instance.photonView.RPC("RPC_UpdateSelection", PhotonTargets.AllBuffered, PhotonNetwork.player);
    }

    public void OnClickKick()
    {
        PlayerNetwork.Instance.photonView.RPC("Kicking", photonPlayer);
    }
}
