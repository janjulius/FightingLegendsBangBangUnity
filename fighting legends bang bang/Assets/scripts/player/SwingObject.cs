﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingObject : MonoBehaviour
{
    private Collider col;
    private int dmg;
    public Vector2 dir;

    public void Start()
    {
        col = GetComponent<Collider>();
    }

    public void Setup(int dmg, CapsuleCollider pCol)
    {
        this.dmg = dmg;

        transform.localScale = new Vector3(1, pCol.height * 0.7f, pCol.radius * 2);

    }

    public void OnTriggerEnter(Collider other)
    {
        PlayerBase pb = other.gameObject.GetComponent<PlayerBase>();

        if (pb)
        {
            GameManager.Instance.Players.Find(x => x.netPlayer == PhotonNetwork.player).photonViewer.RPC("RPC_AddSpecial", PhotonTargets.All, dmg);
            pb.photonViewer.RPC("RPC_GotAttacked", pb.netPlayer, dmg, dir, 0, PhotonNetwork.player);
        }

        //other.gameObject.GetComponent<Health>().DealDamage(dmg, dir, 0, PhotonNetwork.player);
    }
}
