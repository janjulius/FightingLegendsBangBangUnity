﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterLayoutGroup : MonoBehaviour
{
    public GameObject charHead;

    // Use this for initialization
    void Start()
    {
        int i = 0;
        foreach (Sprite head in GameManager.Instance.CharacterHeads)
        {
            GameObject obj = Instantiate(charHead, transform, false);
            obj.GetComponent<Image>().sprite = head;
            obj.GetComponent<CharacterListing>().charId = i;
            i++;
        }

    }

}
