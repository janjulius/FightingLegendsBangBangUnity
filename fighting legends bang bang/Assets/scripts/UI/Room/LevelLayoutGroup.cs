using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelLayoutGroup : MonoBehaviour
{

    public GameObject levelObj;

    // Use this for initialization
    void Start()
    {
        
        int i = 0;
        foreach (var level in GameManager.Instance.LevelData)
        {


            GameObject obj = Instantiate(levelObj, transform, false);
            obj.GetComponent<Image>().sprite = level.LevelImage;
            obj.GetComponent<LevelListing>().lid = i;

            i++;
        }
        
    }
}
