using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    public Character currentCharacter;
    public Health healthController;

    public void Update()
    {
        //Debug.Log(PhotonNetwork.playerList.Length);
        //
        //Debug.Log(PhotonNetwork.playerList[0].TagObject);

    }

    public void TakeDamage(int damage)
    {
        healthController.Damage += damage;
    }

    public void TakeHealth(int damage)
    {
        healthController.HealthPoints -= damage;
    }

    public void AddKnockBack(Vector3 dir, double power)
    {
        
    }
    
}
