using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    public List<PlayerBase> Players = new List<PlayerBase>();
    public Sprite[] CharacterHeads;


    private void Awake()
    {
        Instance = this;
    }



}
