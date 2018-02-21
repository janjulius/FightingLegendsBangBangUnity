using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBall : MonoBehaviour
{
    private PhotonView pho;
    private Rigidbody body;
    public int damage = 150;

    List<PlayerBase> alreadyHit = new List<PlayerBase>();


    private void Start()
    {
        pho = GetComponent<PhotonView>();
        body = GetComponent<Rigidbody>();
        //body.maxAngularVelocity = 1000000;


    }

    public void OnCollisionEnter(Collision other)
    {
        PlayerBase opb = other.gameObject.GetComponent<PlayerBase>();

        if (opb && pho.owner == PhotonNetwork.player)
        {

            if (opb.netPlayer != pho.owner && !alreadyHit.Contains(opb))
            {
                var norm = Vector3.Normalize(GetComponent<Rigidbody>().velocity);
                var dir = new Vector2(norm.z, 1);
                alreadyHit.Add(opb);

                opb.photonViewer.RPC("RPC_GotAttacked", opb.netPlayer, damage, dir, 1, PhotonNetwork.player);
            }
        }

        //other.gameObject.GetComponent<Health>().DealDamage(dmg, dir, 0, PhotonNetwork.player);
    }

}
