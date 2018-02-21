using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingObject : MonoBehaviour
{
    private Collider col;
    public int dmg;
    public Vector2 dir;
    public int type = 0;
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
            pb.photonViewer.RPC("RPC_AddSpecial", PhotonTargets.All, pb.currentCharacter.SpecialCounter + dmg);
            opb.photonViewer.RPC("RPC_GotAttacked", opb.netPlayer, dmg, dir, type, PhotonNetwork.player);
        }

        //other.gameObject.GetComponent<Health>().DealDamage(dmg, dir, 0, PhotonNetwork.player);
    }
}