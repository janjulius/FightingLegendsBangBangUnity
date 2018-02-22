using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocky : Character
{
    private int ultHitDamage = 20;
    private int ultLastHitDamage = 25;

    public Rocky()
    {
        name = "Rocky";
        nameAfter = "the Raccoon";
        AttackCooldown = 0.5f;
        SwingCooldown = 0.1f;
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
        PlayerBase ultTarget = GetTarget();

        if (!ultTarget)
            return;


        StartCoroutine(Leaping(ultTarget));

        print(ultTarget.transform.position.z < transform.position.z ? "left" : "right");

        print(ultTarget.netPlayer.NickName);

    }

    IEnumerator Leaping(PlayerBase t)
    {
        PlayerBase ultTarget = t;
        pb.photonViewer.RPC("RPC_AddSpecial", PhotonTargets.All, 0);
        IsStunned = true;
        pb.stunDuration = 10;
        bool left = ultTarget.transform.position.z < transform.position.z;

        Vector3 prepPos = new Vector3();
        prepPos.z = transform.position.z + (left ? -3 : 3);
        prepPos.y = transform.position.y + 3;

        print(Vector3.Distance(transform.position, prepPos));
        while (Vector3.Distance(transform.position, prepPos) > 0.5f)
        {
            transform.position = Vector3.Lerp(transform.position, prepPos, 5 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        var pos = transform.position;
        pos.x = 50000;

        transform.position = pos;
        yield return new WaitForSeconds(1f);
        pos.x = 0;
        transform.position = pos;

        ultTarget = GetTarget();

        if (!ultTarget)
        {
            pb.stunDuration = 0;
            yield break;
        }

        ultTarget.photonViewer.RPC("RPC_ChangePosition", ultTarget.netPlayer, ultTarget.transform.position);
        ultTarget.photonViewer.RPC("RPC_Stun", ultTarget.netPlayer, 100f);

        var targetpos = ultTarget.transform.position;
        targetpos.z += left ? (ultTarget.GetComponent<CapsuleCollider>().radius + pb.playerController.capsule.radius) :
            -(ultTarget.GetComponent<CapsuleCollider>().radius + pb.playerController.capsule.radius);

        transform.position = targetpos;
        yield return new WaitForSeconds(0.2f);

        Vector2 dir = new Vector2(left ? -1 : 1, 1);

        for (int i = 0; i < 2; i++)
        {
            ultTarget.photonViewer.RPC("RPC_GotAttacked", ultTarget.netPlayer, ultHitDamage, dir, 1, PhotonNetwork.player);
            print("lol");
            pb.animator.SetInteger("AttackState", 1);
            yield return new WaitForSeconds(0.4f);
            pb.animator.SetInteger("AttackState", -1);
            yield return new WaitForSeconds(0.1f);
        }
        ultTarget.photonViewer.RPC("RPC_Stun", ultTarget.netPlayer, 0f);

        ultTarget.photonViewer.RPC("RPC_GotAttacked", ultTarget.netPlayer, ultLastHitDamage, dir, 1, PhotonNetwork.player);


        pb.stunDuration = 0;
        yield break;
    }


    private PlayerBase GetTarget()
    {
        PlayerBase ultTarget = null;
        List<PlayerBase> ultTargets = GameManager.Instance.Players.FindAll(x =>
            x != null &&
            Vector3.Distance(x.transform.position, transform.position) < 500 &&
            !x.healthController.death &&
            x.netPlayer != PhotonNetwork.player);

        if (ultTargets.Count == 0)
            return null;

        ScoreManager.Instance.view.RPC("RPC_AddUltsUsed", PhotonTargets.MasterClient, pb.netPlayer, 1);

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

        return ultTarget;
    }
}
