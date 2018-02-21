using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Willem : Character
{

    private bool Ulted;
    private bool InBallForm;

    private GameObject ultobj;

    public Willem()
    {
        name = "Willem";
        nameAfter = "the Snowman";
        AttackCooldown = 0.5;
        SwingCooldown = 0.1;
        BasicAttackDamage = 10;
        rangeModifier = 1;
        specialCounterThreshHold = 100;
    }
    public override void SpecialAttack()
    {
        print("willem ult");
        pb.frozen = true;


        if (!InBallForm && !Ulted)
        {
            StartCoroutine(StartSpecial());
        }
        if (InBallForm)
        {
            print("!!!");
            StopCoroutine(StartSpecial());
            StopUlt();
        }
    }

    IEnumerator StartSpecial()
    {
        Ulted = true;
        pb.RPC_DoPunch(4, Vector2.zero);
        pb.photonViewer.RPC("RPC_DoPunch", PhotonTargets.Others, 4, Vector2.zero);
        yield return new WaitForSeconds(0.7f);
        pb.RPC_DoPunch(-1, Vector2.zero);
        pb.photonViewer.RPC("RPC_DoPunch", PhotonTargets.Others, -1, Vector2.zero);

        Vector3 pos = transform.position;

        transform.position = new Vector3(50000, transform.position.y, transform.position.x);

        var obj = PhotonNetwork.Instantiate("SnowBall", pos, Quaternion.identity, 0);
        ultobj = obj;
        var rigd = obj.GetComponent<Rigidbody>();
        bool right = pb.playerController.right;


        rigd.velocity = new Vector3(0, 0, right ? 20 : -20);

        float timer = 2.5f;

        while (timer > 0)
        {
            if (pb.Keys.SpecialAttackButton())
            {
                SpecialAttack();
                yield break;
            }

            InBallForm = true;
            Ray rayLeft = new Ray(obj.transform.position, -Vector3.forward);
            Ray rayRight = new Ray(obj.transform.position, Vector3.forward);

            RaycastHit hitLeft;
            RaycastHit hitRight;

            if (Physics.Raycast(rayLeft, out hitLeft, 1.7f) && rigd.velocity.z < 0)
                if (hitLeft.transform.gameObject.layer == 9)
                {
                    print("right touch");
                    right = true;
                }
            if (Physics.Raycast(rayRight, out hitRight, 1.7f) && rigd.velocity.z > 0)
                if (hitRight.transform.gameObject.layer == 9)
                {
                    print("left touch");
                    right = false;
                }

            Debug.DrawLine(rayLeft.origin, rayLeft.origin + -Vector3.forward * 1.7f, Color.red);
            Debug.DrawLine(rayRight.origin, rayRight.origin + Vector3.forward * 1.7f, Color.blue);
            timer -= Time.deltaTime;
            rigd.velocity = new Vector3(0, rigd.velocity.y, right ? 20 : -20);
            var newpos = new Vector3(50000, rigd.transform.position.y, rigd.transform.position.z);
            transform.position = Vector3.Lerp(transform.position, newpos, 1 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        StopUlt();
    }

    private void StopUlt()
    {
        var spawn = ultobj.transform.position;
        PhotonNetwork.Destroy(ultobj);
        transform.position = spawn;
        pb.photonViewer.RPC("RPC_AddSpecial", PhotonTargets.All, 0);

        pb.playerController.VerticalVelocity = 0;
        pb.playerController.KnockBack = Vector2.zero;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        pb.frozen = false;
        Ulted = false;
        InBallForm = false;
    }
}
