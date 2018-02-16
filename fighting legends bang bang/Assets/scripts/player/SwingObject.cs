using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingObject : MonoBehaviour
{
    private Collider col;
    private int dmg;
    public Vector2 dir;

    public void Start()
    {
        col = GetComponent<Collider>();
    }

    public void Setup(int dmg, CapsuleCollider pCol)
    {
        this.dmg = dmg;

        transform.localScale = new Vector3(1, pCol.height * 0.7f, pCol.radius * 2);

    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("hit someone");

        if (other.gameObject.GetComponent<Health>() != null)
            other.gameObject.GetComponent<Health>().DealDamage(dmg, dir);
    }
}
