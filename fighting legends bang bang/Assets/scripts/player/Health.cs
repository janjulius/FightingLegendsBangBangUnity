using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private float dmg = 0;

    private int hp = 250;
    private int lives = 3;

    public bool death = false;

    private PlayerBase pb;
    public PhotonPlayer LastHitBy;

    private int Cheering1Audio = 12;

    public float Damage
    {
        get { return dmg; }
        set { dmg = value; }
    }

    public int HealthPoints
    {
        get { return hp; }
        set { hp = value; }
    }

    public int Lives
    {
        get { return lives; }
        set { lives = value; }
    }

    private void Awake()
    {
        pb = GetComponent<PlayerBase>();
    }

    public void DealDamage(int dmg, Vector2 dir, int t, PhotonPlayer other)
    {
        Debug.Log("damage");
    }

    public void OnDeath()
    {
        death = true;
        PlayerNetwork.Instance.photonView.RPC("PlaySound", PhotonTargets.All, Cheering1Audio);
        StartCoroutine(CurrentGameManager.Instance.RespawnPlayer(gameObject, pb.SpawnPoint));
        pb.playerController.KnockBack = Vector2.zero;
        GetComponent<Rigidbody>().velocity = Vector3.zero;

        if (pb.currentCharacter.SpecialCounter > 50)
        {
            int spec = 50 + (pb.currentCharacter.SpecialCounter - 50) / 2;
            pb.photonViewer.RPC("RPC_AddSpecial", PhotonTargets.All, spec);
        }

        ScoreManager.Instance.view.RPC("RPC_AddDeath", PhotonTargets.MasterClient, pb.netPlayer, LastHitBy);
        GetComponent<PhotonView>().RPC("RPC_OnDeath", PhotonTargets.All);
    }

    [PunRPC]
    public void RPC_OnDeath()
    {
        this.death = true;
        this.lives--;
        this.Damage = 0;


        pb.gpc.playerPanels.Find(x => x.photonPlayer == pb.netPlayer).UpdateUI();
        if (this.lives <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            pb.photonViewer.RPC("RPC_Stun", PhotonTargets.All, 0f);
        }
    }

    [PunRPC]
    public void RPC_alliveAgain()
    {
        this.death = false;
        pb.playerController.KnockBack = Vector2.zero;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        pb.playerController.VerticalVelocity = 0;
    }

    [PunRPC]
    public void RPC_AddSpecial(int spec)
    {
        pb.currentCharacter.SpecialCounter = spec;
        pb.gpc.playerPanels.Find(x => x.photonPlayer == pb.netPlayer).UpdateUI();

    }

    [PunRPC]
    public void RPC_GotAttacked(int dmg, Vector2 dir, int t, PhotonPlayer other)
    {
        bool isBlocking = pb.currentCharacter.isBlocking;
        bool isInvulnerable = pb.currentCharacter.IsInvulnerable;
        bool isKnockBackImmune = pb.currentCharacter.IsKnockBackImmume;
        LastHitBy = other;
        dmg = (int)(dmg / pb.currentCharacter.armor);

        if (!isInvulnerable)
        {
            if (!isBlocking)
            {
                PlayerNetwork.Instance.photonView.RPC("PlaySound", PhotonTargets.All, pb.currentCharacter.gotHitAudio);

                ScoreManager.Instance.view.RPC("RPC_AddDamageTaken", PhotonTargets.MasterClient, pb.netPlayer, other, dmg, t);
                pb.photonViewer.RPC("RPC_DealDamage", PhotonTargets.All, dmg);

                if (!isKnockBackImmune)
                {
                    pb.AddKnockBack(dir, 10 + this.dmg+this.dmg);
                }
            }
            else
            {
                PlayerNetwork.Instance.photonView.RPC("PlaySound", PhotonTargets.All, pb.currentCharacter.blockedAudio);
                ScoreManager.Instance.view.RPC("RPC_AddDamageBlocked", PhotonTargets.MasterClient, pb.netPlayer, dmg);
            }
        }
    }



    [PunRPC]
    public void RPC_DealDamage(int dmg)
    {

        this.dmg = this.dmg + dmg;

        pb.gpc.playerPanels.Find(x => x.photonPlayer == pb.netPlayer).UpdateUI();
        pb.HitParticleSystem.Play();
    }
}
