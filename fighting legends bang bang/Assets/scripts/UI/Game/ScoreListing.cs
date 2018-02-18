using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreListing : MonoBehaviour
{

    public GameObject group;
    public GameObject text;
    public Text name;
    public Text character;
    public Text ready;
    public int id;

	
    public void SetText(string n,string c,Color col)
    {
        name.text = n;
        character.text = c;
        GetComponent<Image>().color = col;
    }

    public void SetReady()
    {
        ready.text = "READY";
        ready.color = Color.green;
    }

    public void AddData(string t)
    {
        var obj = Instantiate(text, group.transform, false);
        obj.GetComponent<Text>().text = t;
    }

}
