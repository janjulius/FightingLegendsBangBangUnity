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
    public GameObject mark;
    private float timeChecker;

    public Tjeerd()
    {
        name = "Tjeerd";
        nameAfter = "the Rugby God";
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

    public void Update()
    {
        players = GameManager.Instance.Players;
        if (isUlting)
        {
            timeChecker += Time.deltaTime;
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].netPlayer != PhotonNetwork.player
                    && Vector3.Distance(players[i].transform.position, transform.position) < impactDistance
                    && !ultTargets.Contains(players[i]))
                {
                    ultTargets.Add(players[i]);
                    //players[i].netPlayer.ID
                    pb.photonViewer.RPC("RPC_MarkEnemy", PhotonTargets.All, 2.5f - timeChecker,
                        players[i].netPlayer.ID);
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
        timeChecker = 0;
        yield return new WaitForSeconds(2.5f);

        for (int i = 0; i < ultTargets.Count; i++)
        {
            bool left = ultTargets[i].transform.position.z < transform.position.z;
            Vector2 dir = new Vector2(left ? -1 : 1, 1);
            ultTargets[i].photonViewer.RPC("RPC_GotAttacked", ultTargets[i].netPlayer, damage, dir, 1, PhotonNetwork.player);
        }

        speed = speed / 2;
        CanJump = true;
        isUlting = false;
        ultTargets.Clear();
    }

    [PunRPC]
    public void RPC_MarkEnemy(float timer, int enemy)
    {
        PlayerBase target = null;
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].netPlayer.ID == enemy)
            {
                target = players[i];
                break;
            }
        }
        if (target != null)
        {
            GameObject m = Instantiate(mark, target.transform.position, Quaternion.identity);
            m.transform.parent = target.transform;
            Destroy(m, timer);
        }
    }
}
