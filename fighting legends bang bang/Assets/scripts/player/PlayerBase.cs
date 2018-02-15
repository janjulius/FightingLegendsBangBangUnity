﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    public Character currentCharacter;
    public Health healthController;
    public PhotonPlayer netPlayer;
    public GamePanelContainer gpc;

    private void Awake()
    {
        GameManager.Instance.Players.Add(this);
        PhotonView phov = GetComponent<PhotonView>();
        netPlayer = phov.owner;
        gpc = FindObjectOfType<GamePanelContainer>();
        gpc.playerPanels.Find(x => x.photonPlayer == netPlayer).playerBase = this;

        Game g = FindObjectOfType<Game>();
        GameObject pc = Instantiate(g.characterModels[0],
            new Vector3(transform.position.x, transform.position.y, transform.position.z) + g.characterPositionOffsets[0],
            Quaternion.identity);

        pc.transform.parent = gameObject.transform;
        GetComponent<PlayerController>().PlayerBody = pc;
        
        gpc.playerPanels.Find(x => x.photonPlayer == netPlayer).UpdateUI();
        PhotonNetwork.player.TagObject = gameObject;
    }

    //dir -1 = right
    //dir 0 = left
    //dir 1 = up
    //dir 2 = down
    public void RegularAttack(int dir)
    {
        if (!currentCharacter.isBlocking)
            if (currentCharacter.CanAttack())
                currentCharacter.Attack(dir);
    }

    public void SpecialAttack()
    {
        if (!currentCharacter.isBlocking)
            if (currentCharacter.SpecialReady())
                currentCharacter.SpecialAttack();
    }

    public void TakeDamage(int damage)
    {
        healthController.Damage += damage;
    }

    public void TakeHealth(int damage)
    {
        healthController.HealthPoints -= damage;
    }

    public void AddKnockBack(Vector3 dir, double power)
    {

    }

}
