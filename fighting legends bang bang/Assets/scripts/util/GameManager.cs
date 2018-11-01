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

    public LevelInfo[] LevelData;
    public CharacterInfo[] CharacterData;


    private void Awake()
    {
        Instance = this;

        int count = SceneManager.sceneCountInBuildSettings;

        print(count);
        SceneList = new string[count];

        for (int i = 0; i < count; i++)
        {
            SceneList[i] = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
        }
    }


    public int GetSceneId(string n)
    {
        return Array.IndexOf(SceneList, n);
    }
}
