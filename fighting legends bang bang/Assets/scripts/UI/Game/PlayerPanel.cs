﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour
{


    public PhotonPlayer photonPlayer;
    public PlayerBase playerBase;

    [SerializeField] private Text playerName;
    [SerializeField] private GameObject LifesContainer;
    [SerializeField] private GameObject Hearth;
    [SerializeField] private Image charImage;
    [SerializeField] private Image borderImage;
    [SerializeField] private Image innerImage;
    [SerializeField] private Text damageText;
    [SerializeField] private Text charText;
    [SerializeField] private Slider specSlider;
    [SerializeField] private Text specText;

    public void ApplyPhotonPlayer(PhotonPlayer phoPlayer)
    {
        photonPlayer = phoPlayer;
        playerName.text = phoPlayer.NickName;
        damageText.text = string.Format("{0}%", 0);

        //borderImage.color = PhotonNetwork.player == phoPlayer ? Color.green : Color.black;

        Color c = new Color((float)photonPlayer.CustomProperties["pColorR"], (float)photonPlayer.CustomProperties["pColorG"], (float)photonPlayer.CustomProperties["pColorB"]);
        borderImage.color = c;
    }

    public void UpdateUI()
    {
        damageText.text = string.Format("{0}%", playerBase.healthController.Damage);

        specSlider.value = playerBase.currentCharacter.SpecialCounter;
        specText.text = playerBase.currentCharacter.SpecialCounter + "%";

        float ratio = Mathf.Clamp((float)playerBase.healthController.Damage / 300, 0, 1);

        damageText.color = Color.Lerp(Color.white, new Color(0.5f, 0, 0), ratio);

        charText.text = GameManager.Instance.CharacterData[(int)photonPlayer.CustomProperties["charId"]].CharacterName.ToUpper();
        charImage.sprite = GameManager.Instance.CharacterData[(int)photonPlayer.CustomProperties["charId"]].CharacterHead;

        foreach (Transform child in LifesContainer.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < playerBase.healthController.Lives; i++)
            Instantiate(Hearth, LifesContainer.transform, false);

    }
}
