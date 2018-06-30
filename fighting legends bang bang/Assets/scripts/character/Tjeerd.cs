using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Tjeerd : Character
{
    private List<PlayerBase> alreadyHit = new List<PlayerBase>();
    private int damage = 100;
    private bool isUlting = false;
    private float impactDistance = 2;
    private List<PlayerBase> ultTargets = new List<PlayerBase>();
    private List<PlayerBase> players;

    public Tjeerd()
    {
        name = "Tjeerd";
        nameAfter = "the Rugby God";
        AttackCooldown = 0.5f;
        SwingCooldown = 0.1f;
        BasicAttackDamage = 10;
        SpecialCounter = 100;
        specialCounterThreshHold = 100;
        rangeModifier = 1;
    }

    public override void Attack(Vector2 dir)
    {
        base.Attack(dir);
    }

    public void Update()
    {
        players = GameManager.Instance.Players;
        if (isUlting)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].netPlayer != PhotonNetwork.player
                    && Vector3.Distance(players[i].transform.position, transform.position) < impactDistance
                    && !ultTargets.Contains(players[i]))
                {
                    ultTargets.Add(players[i]);
                    print("added target " + players[i].name);
                }
            }
        }
    }

    public override void SpecialAttack()
    {
        if (isUlting)
        {
            StopCoroutine(StartSpecial());
        }
        else
        {
            StartCoroutine(StartSpecial());
        }
    }

    IEnumerator StartSpecial()
    {
        ScoreManager.Instance.view.RPC("RPC_AddUltsUsed", PhotonTargets.MasterClient, pb.netPlayer, 1);
        isUlting = true;
        CanJump = false;
        speed = speed * 2;



        print("Tjeerd special");

        yield return new WaitForSeconds(2.5f);


    }
}
