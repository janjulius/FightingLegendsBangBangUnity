using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomstronk : Character {

    public Boomstronk()
    {
        name = "Boomstronk";
        nameAfter = "the Treestump";
        AttackCooldown = 0.5f;
        SwingCooldown = 0.1f;
        BasicAttackDamage = 10;
        rangeModifier = 1;
        specialCounterThreshHold = 100;
        SpecialCounter = 100;
    }

    public override void SpecialAttack()
    {
        if (pb.playerController.CheckSide(PlayerController.Direction.Bottom)) //is grounded?
        {
            if (SpecialReady()) //can acutally use special attack
            {
                pb.photonViewer.RPC("RPC_AddSpecial", PhotonTargets.All, 0);

                List<PlayerBase> ultTargets = GameManager.Instance.Players.FindAll(x =>
                    x != null &&
                    Vector3.Distance(x.transform.position, transform.position) < 500 &&
                    !x.healthController.death && 
                    x.playerController.CheckSide(PlayerController.Direction.Bottom) &&
                    x.netPlayer != PhotonNetwork.player);

                pb.photonViewer.RPC("RPC_Stun", pb.netPlayer, 1000f);
            }
        }
    }
}
