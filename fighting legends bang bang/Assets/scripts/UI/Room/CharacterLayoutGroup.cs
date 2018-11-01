using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class CharacterLayoutGroup : MonoBehaviour
{
    public GameObject charHead;
    public Text HoverText;

    // Use this for initialization
    void Start()
    {
        int i = 0;
        foreach (var character in GameManager.Instance.CharacterData)
        {
            if (i != 0)
            {
                GameObject obj = Instantiate(charHead, transform, false);
                obj.GetComponent<Image>().sprite = character.CharacterHead;
                obj.GetComponent<CharacterListing>().charId = i;
                obj.GetComponent<CharacterListing>().HoverText = HoverText;
                if (character.CharacterDisabled)
                {
                    obj.GetComponent<Button>().interactable = false;
                }
            }

            i++;
        }

    }
}
