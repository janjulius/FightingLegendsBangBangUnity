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
        foreach (Sprite image in GameManager.Instance.LevelsImage)
        {


            GameObject obj = Instantiate(levelObj, transform, false);
            obj.GetComponent<Image>().sprite = image;
            obj.GetComponent<LevelListing>().lid = i;

            i++;
        }

    }
}
