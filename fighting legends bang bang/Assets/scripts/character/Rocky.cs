using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocky : Character
{

    public Rocky()
    {
        name = "Rocky";
        nameAfter = "the Raccoon";
        AttackCooldown = 0.5;
        SwingCooldown = 0.1;
        BasicAttackDamage = 10;
        specialCounterThreshHold = 100;
        rangeModifier = 1;
    }
    public override void Attack(Vector2 dir)
    {
        base.Attack(dir);
    }

    public override void SpecialAttack()
    {
        print("special pogchamp");
        PlayerBase ultTarget = null;
        List<PlayerBase> ultTargets = GameManager.Instance.Players.FindAll(x =>
            x.gameObject != null &&
            Vector3.Distance(x.transform.position, transform.position) < 500 &&
            !x.healthController.death &&
            x.netPlayer != PhotonNetwork.player);

        if (ultTargets.Count == 0)
            return;

        if (ultTargets.Count == 1)
            ultTarget = ultTargets[0];

        if (ultTarget == null)
        {
            float dist = float.MaxValue;
            foreach (PlayerBase pbb in ultTargets)
            {
                float between = Vector3.Distance(pbb.transform.position, transform.position);
                if (between < dist)
                {
                    dist = between;
                    ultTarget = pbb;
                }
            }
        }

        //StartCoroutine(Leaping(ultTarget));

        print(ultTarget.transform.position.z < transform.position.z ? "left" : "right");

        print(ultTarget.netPlayer.NickName);

    }

    IEnumerator Leaping(PlayerBase ultTarget)
    {
        IsStunned = true;

        Vector3 prepPos = new Vector3();
        //prepPos.y = transform.position.y + ()



        ultTarget.photonViewer.RPC("RPC_Stun", ultTarget.netPlayer, 2);
        yield break;
    }
}
