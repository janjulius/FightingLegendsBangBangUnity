using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    internal int ghostAudio = 3;

    void Start()
    {
        PlayerNetwork.Instance.photonView.RPC("PlaySound", PhotonTargets.All, ghostAudio);
        var pho = GetComponent<PhotonView>();
        PhotonPlayer p = pho.owner;
        var col = new Color((float)p.CustomProperties["pColorR"], (float)p.CustomProperties["pColorG"], (float)p.CustomProperties["pColorB"]);
        GetComponentInChildren<Renderer>().material.color = col;
    }
}
