using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{

    void Start()
    {
        var pho = GetComponent<PhotonView>();
        PhotonPlayer p = pho.owner;
        var col = new Color((float)p.CustomProperties["pColorR"], (float)p.CustomProperties["pColorG"], (float)p.CustomProperties["pColorB"]);
        GetComponentInChildren<Renderer>().material.color = col;
    }
}
