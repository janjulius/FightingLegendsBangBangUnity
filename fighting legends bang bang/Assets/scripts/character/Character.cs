using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.UI;

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
    public float rangeModifier;

    //other properties
    private bool blocking;
    private bool alive;
    private bool stunned;
    private bool invulnerable;
    private bool knockbackImmume;
    private Vector3 knockBack;
    private double maxGravityVelocity;
    private double gravityVelocity;
    private int[] touchingWalls = new[] { -1, -1, -1, -1 };

    //timer properties
    private double blockTime;
    private double blockTimer;
    private double blockDelay;
    private double blockRemoveCooldown;
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
    private BlockObject blockobject;
    private PlayerBase pb;

    public void Start()
    {
        pb = GetComponent<PlayerBase>();
        swingobject = GetComponentInChildren<SwingObject>();
        swingobject.Setup(basicAttackDamage, GetComponent<CapsuleCollider>());
        swingobject.gameObject.SetActive(false);

        blockobject = GetComponentInChildren<BlockObject>();
        blockobject.gameObject.SetActive(false);
        pb.RPC_DoPunch(-1, Vector2.zero);
        blockTime = 0.1;

    }

    public void Update()
    {
        if (!pb.photonViewer.isMine)
            return;

        if (attackDelay >= 0)
        {
            attackDelay -= 1 * Time.deltaTime;
        }
        if (swingDelay >= 0)
        {
            swingDelay -= 1 * Time.deltaTime;
        }
        else if(swingobject.gameObject.activeSelf)
        {
            swingobject.gameObject.SetActive(false);
            pb.RPC_DoPunch(-1, Vector2.zero);
            GetComponent<PhotonView>().RPC("RPC_DoPunch", PhotonTargets.Others, -1, Vector2.zero);

        }

        //need to set timers
        if (blockDelay >= 0)
        {
            blockDelay -= 1 * Time.deltaTime;
            blocking = true;
            blockobject.gameObject.SetActive(true);
            if (blockDelay <= blockTime)
            {
                blocking = false;
                blockobject.gameObject.SetActive(false);
            }
        }
        else
        {
            blockobject.gameObject.SetActive(false);
        }
    }

    #region overridable methods

    public virtual void Attack(Vector2 dir)
    {
        print("attack started");
        CapsuleCollider coll = pb.playerController.capsule;

        pb.RPC_DoPunch(1, dir);
        GetComponent<PhotonView>().RPC("RPC_DoPunch", PhotonTargets.Others, 1, dir);

        float rangeside = coll.radius;
        float rangeUpDown = coll.height / 3;

        swingobject.gameObject.SetActive(true);
        swingobject.dir = dir;
        swingobject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + dir.y * rangeUpDown * rangeModifier, gameObject.transform.position.z + dir.x * rangeside * rangeModifier);
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
        blockobject.gameObject.SetActive(true);
        blockDelay = blockRemoveCooldown;
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

    public bool isBlocking
    {
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

    public double SwingCooldown
    {
        get { return swingRemoveCooldown; }
        set { swingRemoveCooldown = value; }
    }

    public bool IsKnockBackImmume
    {
        get { return knockbackImmume; }
        set { knockbackImmume = value; }
    }

    public bool IsInvulnerable
    {
        get { return invulnerable; }
        set { invulnerable = value; }
    }

    public bool SpecialReady()
    {
        return specialCounter >= specialCounterThreshHold;
    }

    public bool CanAttack()
    {
        return attackDelay <= 0;
    }

    public bool CanBlock()
    {
        return blockDelay <= 0;
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
