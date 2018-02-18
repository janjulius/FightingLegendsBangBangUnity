using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelListing : MonoBehaviour
{

    internal int lid;

    public void OnClick()
    {
        MainCanvasManager.Instance.LevelSelectCanvas.StartMatch(lid);
    }
}
