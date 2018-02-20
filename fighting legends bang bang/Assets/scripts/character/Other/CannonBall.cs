using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour


{
    private Vector3 targetpos;
    public AudioClip audio;

    void Update()
    {
        Debug.Log("Canjnonballl spawned");
        if (gameObject.transform.position.y > targetpos.y)
        {
            Debug.Log("Falling");
            gameObject.transform.position += Vector3.down * 1 * Time.deltaTime;
        }

        if (gameObject.transform.position.y <= targetpos.y)
            Destroy(gameObject);
    }

    public Vector3 TargetPos
    {
        set { targetpos = value; }
    }
}
