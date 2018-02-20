using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Jens : Character
{

    public GameObject CannonBallGameObject;

    public Jens()
    {
        name = "Jens";
        nameAfter = "the Pirate Panda";
        AttackCooldown = 0.5;
        SwingCooldown = 0.1;
        BasicAttackDamage = 10;
        rangeModifier = 1.3f;
        SpecialCounter = 1000;
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
            List<PlayerBase> possibleTargets = GameManager.Instance.Players;
            Random r = new Random();

            PlayerBase target = GameManager.Instance.Players[r.Next(0, GameManager.Instance.Players.Count)];
            Vector3 targetPos = target.gameObject.transform.position;
            PhotonView.Instantiate(CannonBallGameObject, 
                new Vector3(target.gameObject.transform.position.x, target.gameObject.transform.position.y + 1200, target.gameObject.transform.position.z), Quaternion.identity).
                GetComponent<CannonBall>().TargetPos = target.gameObject.transform.position;

        }
    }
}
