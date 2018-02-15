using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public abstract class Character : MonoBehaviour
{

    public string name = "", nameBefore = "", nameAfter = "";
    private string specialAttackName;

    private int id;
    private int cid;

    //attack range and speed
    private double speed;
    private double attackRange;
    private double attackWidth;

    //jumping
    private int totalJump;
    private int jumpsLeft;
    private double jumpForce;

    //special attacking
    private int specialCounter;
    private int specialCounterThreshHold;
    private bool specialExists;

    //damage
    private int basicAttackDamage;
    private int specialIncrease;

    //other properties
    private bool blocking;
    private bool alive;
    private bool stunned;
    private Vector3 knockBack;
    private double maxGravityVelocity;
    private double gravityVelocity;
    private int[] touchingWalls = new[] {-1, -1, -1, -1};

    //timer properties
    private double blockTime;
    private double blockTimer;
    private double blockCoolDownTimer;
    private double blockCooldownTime;
    private bool canBlock;
    private double swingDelay;
    private double swingRemoveCooldown;
    private double attackRemoveCooldown;
    private double attackDelay;
    private double respawnDelay;
    private double respawnTimer;
    private double specialTimer;
    private bool chargeAttack;

    //tracking data
    private int Tplace = -1;
    // TODO SET TO PLAYER OBJECT private double TLastPerson = -1;
    private double TDamageDone = 0;
    private double TDamageTaken = 0;
    private double TDamageHealed = 0;
    private double THighestDamageSurvived = 0;
    private int TKills = 0;
    private int TDeaths = 0;
    private double TDamageBlocked = 0;
    private double TDamageDoneWithUlt = 0;
    private int TotalUltsUsed = 0;

    private SwingObject swingobject;

    public void Start()
    {
        swingobject = GetComponentInChildren<SwingObject>();
        swingobject.Setup(basicAttackDamage);
        swingobject.gameObject.SetActive(false);
    }

    public void Update()
    {
        if (attackDelay >= 0)
        {
            attackDelay -= 1 * Time.deltaTime;
        }
        if (swingDelay >= 0)
        {
            swingDelay -= 1 * Time.deltaTime;
        }
        else
        {
            swingobject.gameObject.SetActive(false);
        }
    }

    #region overridable methods

    public virtual void Attack(int dir)
    {
        swingobject.gameObject.SetActive(true);
        switch (dir)
        {
            case -1:
                swingobject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z - 1);
                break;
            case 0:
                swingobject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z + 1);
                break;
            case 1:
                swingobject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1, gameObject.transform.position.z);
                break;
            case 2:
                swingobject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 1, gameObject.transform.position.z);
                break;
        }
        swingDelay = swingRemoveCooldown;
        attackDelay = attackRemoveCooldown;
    }

    public virtual void SpecialAttack()
    {
        
    }

    public virtual void Jump()
    {
        
    }

    public virtual void Block()
    {
        
    }

    #endregion

    public int BasicAttackDamage
    {
        get { return basicAttackDamage; }
        set { basicAttackDamage = value; }
    }

    public int SpecialCounter
    {
        get { return this.specialCounter; }
        set { this.specialCounter = value; }
    }

    public bool isBlocking {
        get { return blocking; }
        set { blocking = value; }
    }

    public double AttackCooldown
    {
        get { return attackRemoveCooldown; }
        set { attackRemoveCooldown = value; }
    }

    public double AttackDelay
    {
        get { return attackDelay; }
    }

    public double SwingCooldown {
        get { return swingRemoveCooldown; }
        set { swingRemoveCooldown = value; }
    } 

    public bool SpecialReady()
    {
        return specialCounter >= specialCounterThreshHold;
    }

    public bool CanAttack()
    {
        return attackDelay <= 0;
    }

    public string GetFullName()
    {
        if (nameBefore != "")
        {
            if (nameAfter != "")
            {
                return nameBefore + " " + name + " " + nameAfter;
            }
            return nameBefore + " " + name;
        }
        if (nameAfter != "")
        {
            return name + " " + nameAfter;
        }
        return name;
    }
}
