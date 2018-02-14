using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingObject : MonoBehaviour
{
    private Collider col;
    private int dmg;

    public void Start()
    {
        col = GetComponent<Collider>();
    }

    public void Setup(int dmg)
    {
        this.dmg = dmg;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PhotonView>() != null)
            other.gameObject.GetComponent<PhotonView>().RPC("DealDamage", PhotonTargets.All, dmg);
    }
}
