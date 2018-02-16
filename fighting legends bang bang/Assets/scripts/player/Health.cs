using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private int dmg = 0;

    private int hp = 250;
    private int lives = 3;

    public bool death = false;

    private PlayerBase pb;


    public int Damage
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

    public void DealDamage(int dmg, Vector2 dir)
    {
        Debug.Log("damage");

        GetComponent<PhotonView>().RPC("RPC_DealDamage", PhotonTargets.All, dmg, dir);
    }

    public void OnDeath()
    {
        death = true;
        transform.position = new Vector3(0, 5, 0);
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


        this.death = false;
    }

    [PunRPC]
    public void RPC_DealDamage(int dmg, Vector2 dir)
    {
        this.dmg = this.dmg + dmg;

        if (GetComponent<PhotonView>().isMine)
        {

            pb.AddKnockBack(dir, this.dmg);

            Debug.Log(dir);
            Debug.Log(pb.playerController.KnockBack.x);
        }
        Debug.Log("new health " + this.dmg);
        pb.gpc.playerPanels.Find(x => x.photonPlayer == pb.netPlayer).UpdateUI();
    }
}
