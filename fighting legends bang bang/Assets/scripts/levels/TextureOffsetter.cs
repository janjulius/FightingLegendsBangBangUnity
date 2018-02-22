using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureOffsetter : MonoBehaviour {

    public float scrollSpeedX = 0.5f;
    public float scrollSpeedY = 0;
    public string name = "_MainTex";
    private Material r;

    void Start()
    {
        r = GetComponent<SkinnedMeshRenderer>().materials[0];
    }

    void Update()
    {
        r.SetTextureOffset(name, new Vector2(Time.time * scrollSpeedX, Time.time * scrollSpeedY));
    }
}
