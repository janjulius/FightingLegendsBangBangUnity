using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoom : MonoBehaviour
{
    [SerializeField] private Text roomName;

    public void OnClickCreateRoom()
    {
        MainCanvasManager.Instance.CreateRoomCanvas.transform.SetAsLastSibling();
    }

    

}
