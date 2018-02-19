﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private int dmg = 0;

    private int hp = 250;
    private int lives = 3;

    public bool death = false;

    private PlayerBase pb;
    public PhotonPlayer LastHitBy;


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

    public int Lives
    {
        get { return lives; }
        set { lives = value; }
    }

    private void Awake()
    {
        pb = GetComponent<PlayerBase>();
    }

    public void DealDamage(int dmg, Vector2 dir, int t, PhotonPlayer other)
    {
        Debug.Log("damage");

        GetComponent<PhotonView>().RPC("RPC_DealDamage", PhotonTargets.All, dmg, dir, t, other);
    }

    public void OnDeath()
    {
        death = true;
        StartCoroutine(CurrentGameManager.Instance.RespawnPlayer(gameObject, pb.SpawnPoint));
        pb.playerController.KnockBack = Vector2.zero;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        ScoreManager.Instance.view.RPC("RPC_AddDeath", PhotonTargets.MasterClient, pb.netPlayer, LastHitBy);
        GetComponent<PhotonView>().RPC("RPC_OnDeath", PhotonTargets.All);
    }

    [PunRPC]
    public void RPC_OnDeath()
    {
        this.death = true;
        this.lives--;
        this.Damage = 0;
        pb.gpc.playerPanels.Find(x => x.photonPlayer == pb.netPlayer).UpdateUI();
        if (this.lives <= 0)
        {
            Destroy(gameObject);
        }


        this.death = false;
    }

    [PunRPC]
    public void RPC_DealDamage(int dmg, Vector2 dir, int t, PhotonPlayer other)
    {
        LastHitBy = other;

        if (!pb.currentCharacter.IsInvulnerable)
        {
            if (!pb.currentCharacter.isBlocking)
            {
                if (PhotonNetwork.isMasterClient)
                    ScoreManager.Instance.view.RPC("RPC_AddDamageTaken", PhotonTargets.MasterClient, pb.netPlayer, other, dmg, t);
                this.dmg = this.dmg + dmg;
            }
            else
            {
                if (PhotonNetwork.isMasterClient)
                    ScoreManager.Instance.view.RPC("RPC_AddDamageBlocked", PhotonTargets.MasterClient, pb.netPlayer, dmg);

            }
        }

        if (GetComponent<PhotonView>().isMine)
        {
            if (!pb.currentCharacter.IsKnockBackImmume)
            {
                pb.AddKnockBack(dir, 10 + this.dmg);
            }
        }
        pb.gpc.playerPanels.Find(x => x.photonPlayer == pb.netPlayer).UpdateUI();
    }
}
