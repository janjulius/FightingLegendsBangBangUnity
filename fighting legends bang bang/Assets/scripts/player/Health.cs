﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private int dmg = 0;

    private int hp = 250;

    public int Damage
    {
        get { return dmg; }
        set { dmg = value; }
    }

    public int HealthPoints
    {
        get { return hp; }
        set { hp = value; }
    }

    public void DealDamage(int dmg)
    {
        Debug.Log("damage");
        GetComponent<PhotonView>().RPC("RPC_DealDamage", PhotonTargets.All, dmg);
    }


    [PunRPC]
    public void RPC_DealDamage(int dmg)
    {
        this.dmg = this.dmg + dmg;
        Debug.Log("new health " + this.dmg);
    }
}
