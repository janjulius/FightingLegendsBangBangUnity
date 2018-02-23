using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    public List<PlayerBase> Players = new List<PlayerBase>();
    public GameObject GameUI;
    public Camera GameCamera;
    private string[] SceneList;
    internal List<string> Levels = new List<string>();
    public Sprite[] LevelsImage;
    [Space(10)]
    [Header("Character Data")]
    public Sprite[] CharacterHeads;
    public string[] charNames;
    public string[] charPrefabs;
    public int[] charsNotImplemented;


    private void Awake()
    {
        Instance = this;

        int count = SceneManager.sceneCountInBuildSettings;

        print(count);
        SceneList = new string[count];

        for (int i = 0; i < count; i++)
        {
            SceneList[i] = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));

            if (SceneList[i].Substring(0, 5) == "level")
            {
                print(SceneList[i].Substring(6));
                Levels.Add(SceneList[i]);
            }
        }
    }


    public int GetSceneId(string n)
    {
        return Array.IndexOf(SceneList, n);
    }
}
