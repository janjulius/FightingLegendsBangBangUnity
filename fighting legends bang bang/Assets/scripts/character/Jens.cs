using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Jens : Character
{

    public GameObject CannonBallGameObject;
    public int ultDamage = 50;
    public int ultImpactDamage = 100;

    public Jens()
    {
        name = "Jens";
        nameAfter = "the Pirate Panda";
        AttackCooldown = 0.5f;
        SwingCooldown = 0.1f;
        BasicAttackDamage = 10;
        rangeModifier = 1.3f;
        specialCounterThreshHold = 100;
        //SpecialCounter = 1000;
    }
    public override void Attack(Vector2 dir)
    {
        base.Attack(dir);
    }

    public override void SpecialAttack()
    {
        Debug.Log("Jens ulting");
        if (SpecialReady())
        {
            List<PlayerBase> possibleTargets = GameManager.Instance.Players.FindAll(x =>
                x != null &&
                Vector3.Distance(x.transform.position, transform.position) < 500 &&
                !x.healthController.death &&
                x.netPlayer != PhotonNetwork.player);

            if (possibleTargets.Count == 0)
                return;

            ScoreManager.Instance.view.RPC("RPC_AddUltsUsed", PhotonTargets.MasterClient, pb.netPlayer, 1);

            Random r = new Random();

            PlayerBase target = GameManager.Instance.Players[r.Next(0, GameManager.Instance.Players.Count)];
            Vector3 targetPos = target.gameObject.transform.position;
            PhotonNetwork.Instantiate("Cannonball",
                    new Vector3(target.gameObject.transform.position.x, target.gameObject.transform.position.y + 20,
                        target.gameObject.transform.position.z), Quaternion.identity, 0)
                .GetComponent<CannonBall>().Setup(target.gameObject.transform.position, 50, 100, 7, 10, 0.1f);
            pb.photonViewer.RPC("RPC_AddSpecial", PhotonTargets.All, 0);
        }
    }
}
