using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    private PlayerBase[] players;

    //game properties
    private bool gamePaused = false;
    private bool gameStartup = false;

    //game rules
    private bool blockingAllowed = true;

    private double damageRatio = 1.0;
    private double[] playerDamageRatio = new []{ 1.0, 1.0,1.0,1.0};


    public void Pause()
    {
        gamePaused = !gamePaused;
    }

}
