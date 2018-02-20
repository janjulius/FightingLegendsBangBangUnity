using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingObject : MonoBehaviour
{
    private Collider col;
    private int dmg;
    public Vector2 dir;
    private PlayerBase pb;

    public void Start()
    {
        col = GetComponent<Collider>();
    }

    public void Setup(int dmg, CapsuleCollider pCol, PlayerBase pb)
    {
        this.dmg = dmg;
        this.pb = pb;
        transform.localScale = new Vector3(1, pCol.height * 0.7f, pCol.radius * 2);

    }

    public void OnTriggerEnter(Collider other)
    {
        PlayerBase opb = other.gameObject.GetComponent<PlayerBase>();

        if (opb)
        {
            pb.photonViewer.RPC("RPC_AddSpecial", PhotonTargets.All, dmg);
            opb.photonViewer.RPC("RPC_GotAttacked", opb.netPlayer, dmg, dir, 0, PhotonNetwork.player);
        }

        //other.gameObject.GetComponent<Health>().DealDamage(dmg, dir, 0, PhotonNetwork.player);
    }
}
