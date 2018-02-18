using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.UI;

public class CurrentRoomCanvas : MonoBehaviour
{
    public PlayerLayoutGroup PlayerLayoutGroup;
    public GameObject startButton;
    public GameObject lockButton;
    public Text playerNameText;

    public void OnClickStartMatch()
    {
        MainCanvasManager.Instance.LevelSelectCanvas.transform.SetAsLastSibling();
    }

}
