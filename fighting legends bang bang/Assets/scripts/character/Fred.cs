using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fred : Character
{

    private float impactDistance = 5;
    private int damage = 75;
    public GameObject UltExplosion;

    private int ultSound = 13;

    public Fred()
    {
        name = "Fred";
        nameAfter = "der Goblin";
        AttackCooldown = 0.5f;
        SwingCooldown = 0.1f;
        BasicAttackDamage = 10;
        rangeModifier = 1;
        specialCounterThreshHold = 100;
    }

    public override void SpecialAttack()
    {
        if (SpecialReady())
        {
            ScoreManager.Instance.view.RPC("RPC_AddUltsUsed", PhotonTargets.MasterClient, pb.netPlayer, 1);
            List<PlayerBase> ultTargets = GameManager.Instance.Players.FindAll(x =>
                x != null &&
                Vector3.Distance(x.transform.position, transform.position) < impactDistance &&
                !x.healthController.death &&
                x.netPlayer != PhotonNetwork.player);

            PlayerNetwork.Instance.photonView.RPC("PlaySound", PhotonTargets.All, ultSound);

            foreach (var t in ultTargets)
            {
                Debug.Log("target");
                t.photonViewer.RPC("RPC_GotAttacked", t.netPlayer, damage, FindBlastDirection(t.transform.position), 1, PhotonNetwork.player);
            }

            pb.photonViewer.RPC("RPC_AddSpecial", PhotonTargets.All, 0);
            pb.photonViewer.RPC("RPC_CreateExplosion", PhotonTargets.All);
        }
    }

    public Vector2 FindBlastDirection(Vector3 otherpos)
    {
        return new Vector2(otherpos.z < gameObject.transform.position.z ? -1 : 1, 1);
    }

    [PunRPC]
    public void RPC_CreateExplosion()
    {
        GameObject expl = Instantiate(UltExplosion, transform.position, Quaternion.identity);
        Destroy(expl, 1.5f);
    }
}
