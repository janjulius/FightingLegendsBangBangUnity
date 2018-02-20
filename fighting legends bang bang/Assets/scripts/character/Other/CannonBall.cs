using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public AudioClip audio;

    public void Send(Vector3 targetPosition)
    {

        while (gameObject.transform.position.y > targetPosition.y)
        {
            gameObject.transform.position += Vector3.down * 1;
        }

        Destroy(gameObject);
    }
}
