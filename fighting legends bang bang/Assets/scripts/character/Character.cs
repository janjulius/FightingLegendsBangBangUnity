using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{

    private string name, nameBefore, nameAfter;
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
    private bool isAlive;
    private bool isStunned;
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
    private double swingTimer;
    private double swingCooldown;
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


    public virtual void Attack()
    {
        
    }

    public virtual void SpecialAttack()
    {
        
    }
}
