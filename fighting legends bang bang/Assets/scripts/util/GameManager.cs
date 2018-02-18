using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    public List<PlayerBase> Players = new List<PlayerBase>();
    public string[] SceneList;
    public string[] Levels;
    public Sprite[] LevelsImage;
    public Sprite[] CharacterHeads;
    public string[] charNames;
    public string[] charPrefabs;
    public int[] charsNotImplemented;


    private void Awake()
    {
        Instance = this;
    }


    public int GetSceneId(string n)
    {
        print(n);
        print(Array.IndexOf(SceneList, n));
        return Array.IndexOf(SceneList, n);
    }
}
