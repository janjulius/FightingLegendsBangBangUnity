using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    public List<PlayerBase> Players = new List<PlayerBase>();


    private void Awake()
    {
        Instance = this;
    }



}
