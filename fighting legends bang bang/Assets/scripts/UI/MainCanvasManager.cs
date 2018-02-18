using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvasManager : MonoBehaviour
{
    public LobbyCanvas lobbyCanvas;
    public CurrentRoomCanvas CurrentRoomCanvas;
    public CreateRoomCanvas CreateRoomCanvas;
    public LevelSelectCanvas LevelSelectCanvas;

    public static MainCanvasManager Instance;

    private void Awake()
    {
        Instance = this;
    }
}
