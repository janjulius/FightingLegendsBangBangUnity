using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomstronk : Character
{
    private int totalDamageThisUlt = 0;
    private float ultStunTime = 2f;
    private int ultDamage = 60;

    public Boomstronk()
    {
        name = "Boomstronk";
        nameAfter = "the Treestump";
        AttackCooldown = 0.5f;
        SwingCooldown = 0.1f;
        BasicAttackDamage = 10;
        rangeModifier = 1;
        specialCounterThreshHold = 100;
    }

    public override void SpecialAttack()
    {
        if (pb.playerController.CheckSide(PlayerController.Direction.Bottom)) //is grounded?
        {
            if (SpecialReady()) //can acutally use special attack
            {
                pb.photonViewer.RPC("RPC_AddSpecial", PhotonTargets.All, 0);
                ScoreManager.Instance.view.RPC("RPC_AddUltsUsed", PhotonTargets.MasterClient, pb.netPlayer, 1);

                List<PlayerBase> ultTargets = GameManager.Instance.Players.FindAll(x =>
                    x != null &&
                    !x.healthController.death && 
                    x.playerController.CheckSide(PlayerController.Direction.Bottom) &&
                    x.netPlayer != PhotonNetwork.player);

                Debug.Log("players amount found " + ultTargets.Count);
                
                foreach (PlayerBase t in ultTargets)
                {
                    int dmg = calcDamage(t);
                    totalDamageThisUlt += dmg;
                    bool left = t.transform.position.z < transform.position.z;
                    Vector2 dir = new Vector2(left ? -1 : 1, 1);
                    t.photonViewer.RPC("RPC_Stun", pb.netPlayer, ultStunTime);
                    t.photonViewer.RPC("RPC_GotAttacked", t.netPlayer, dmg, dir, 1, PhotonNetwork.player);
                }
                pb.photonViewer.RPC("RPC_Stun", pb.netPlayer, ultStunTime);
                pb.photonViewer.RPC("RPC_Heal", pb.netPlayer, totalDamageThisUlt / 1.5);
                pb.photonViewer.RPC("RPC_DoHeal", pb.netPlayer, true);

            }
        }
    }

    public int calcDamage(PlayerBase t)
    {
        float result = 0;

        result = 20 + ultDamage;

        return Convert.ToInt32(Mathf.Round(result));
    }
}
