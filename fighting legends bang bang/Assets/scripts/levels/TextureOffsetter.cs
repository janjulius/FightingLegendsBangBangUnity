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
        if (GetComponent<SkinnedMeshRenderer>() != null)
        {
            r = GetComponent<SkinnedMeshRenderer>().materials[0];
        } 
        else if (GetComponent<MeshRenderer>() != null)
        {
            r = GetComponent<MeshRenderer>().materials[0];
        }
        else
        {
            Debug.LogError("There is no kind of mesh renedere attatched");
        }
    }

    void Update()
    {
        r.SetTextureOffset(name, new Vector2(Time.time * scrollSpeedX, Time.time * scrollSpeedY));
    }
}
