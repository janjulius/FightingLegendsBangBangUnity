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
        foreach (Sprite head in GameManager.Instance.CharacterHeads)
        {
            if (i != 0)
            {
                GameObject obj = Instantiate(charHead, transform, false);
                obj.GetComponent<Image>().sprite = head;
                obj.GetComponent<CharacterListing>().charId = i;
                obj.GetComponent<CharacterListing>().HoverText = HoverText;
                if (GameManager.Instance.charsNotImplemented.Contains(i))
                {
                    obj.GetComponent<Button>().interactable = false;
                }
            }

            i++;
        }

    }
}
