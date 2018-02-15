using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private int dmg = 0;

    private int hp = 250;
    private int lives = 3;

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
        get { return lives;}
        set { lives = value; }
    }

    private void Awake()
    {
        pb = GetComponent<PlayerBase>();
    }

    public void DealDamage(int dmg)
    {
        Debug.Log("damage");
        GetComponent<PhotonView>().RPC("RPC_DealDamage", PhotonTargets.All, dmg);
    }


    [PunRPC]
    public void RPC_DealDamage(int dmg)
    {
        this.dmg = this.dmg + dmg;
        Debug.Log("new health " + this.dmg);
        pb.gpc.playerPanels.Find(x => x.photonPlayer == pb.netPlayer).UpdateUI();
    }
}
