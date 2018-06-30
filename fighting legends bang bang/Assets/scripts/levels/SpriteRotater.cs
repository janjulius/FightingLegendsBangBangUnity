using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRotater : MonoBehaviour
{

    public float speed = 100;

    void Update()
    {
        transform.Rotate(0, 0, Time.deltaTime * speed);
    }
}
