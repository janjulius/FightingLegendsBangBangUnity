using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Berend : Character
{

    private float standardSpeed;
    private float standardJumpForce;
    private float standardAttackCooldown;
    private int standardDamage;

    private bool inUlt;

    public Berend()
    {
        name = "Berend";
        nameAfter = "the Yeti";
        AttackCooldown = 0.5f;
        SwingCooldown = 0.1f;
        BasicAttackDamage = 10;
        specialCounterThreshHold = 100;

        standardSpeed = speed;
        standardJumpForce = jumpForce;
        standardAttackCooldown = AttackCooldown;
        standardDamage = BasicAttackDamage;
        specialCounterThreshHold = 100;
    }
    public override void Attack(Vector2 dir)
    {
        if (!inUlt)
        {
            base.Attack(dir);
        }
        else
        {
            CapsuleCollider coll = pb.playerController.capsule;

            int punchType = 1;

            if (dir.y == 1)
                punchType = 3;
            if (dir.y == -1)
                punchType = 2;

            pb.RPC_DoPunch(punchType, dir);

            GetComponent<PhotonView>().RPC("RPC_DoPunch", PhotonTargets.Others, punchType, dir);

            float rangeside = coll.radius;
            float rangeUpDown = coll.height / 3;

            swingobject.gameObject.SetActive(true);
            swingobject.dir = dir;
            swingobject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + dir.y * rangeUpDown * rangeModifier, gameObject.transform.position.z + dir.x * rangeside * rangeModifier);
            swingobject.type = 1;
            swingobject.dmg = BasicAttackDamage;
            swingDelay = swingRemoveCooldown;
            attackDelay = attackRemoveCooldown;
            pb.photonViewer.RPC("RPC_AddSpecial", PhotonTargets.All, 0);

        }
    }

    public override void SpecialAttack()
    {
        inUlt = true;
        pb.photonViewer.RPC("RPC_AddSpecial", PhotonTargets.All, 0);
        ScoreManager.Instance.view.RPC("RPC_AddUltsUsed", PhotonTargets.MasterClient, pb.netPlayer, 1);
        StartCoroutine(SpecialAttackIE());

    }

    IEnumerator SpecialAttackIE()
    {
        speed = 20;
        jumpForce = 20;
        IsKnockBackImmume = true;
        BasicAttackDamage = 20;
        AttackCooldown = 0.2f;
        armor = 0.5f;

        yield return new WaitForSeconds(4);

        speed = standardSpeed;
        jumpForce = standardJumpForce;
        IsKnockBackImmume = false;
        BasicAttackDamage = standardDamage;
        AttackCooldown = standardAttackCooldown;
        armor = 1;
        inUlt = false;
        pb.photonViewer.RPC("RPC_AddSpecial", PhotonTargets.All, 0);

    }
}
