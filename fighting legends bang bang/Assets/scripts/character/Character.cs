using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.UI;

public abstract class Character : MonoBehaviour
{

    public string name = "", nameBefore = "", nameAfter = "";
    protected string specialAttackName;

    private int id;
    private int cid;

    //attack range and speed
    internal float speed = 13f;
    protected double attackRange;
    protected double attackWidth;

    //jumping
    internal int maxJumps = 1;
    internal float jumpForce = 15f;

    //special attacking
    private int specialCounter;
    protected int specialCounterThreshHold;
    private bool specialExists;

    //damage
    private int basicAttackDamage;
    private int specialIncrease;
    public float rangeModifier;
    internal float armor = 1;

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
    protected const float blockCooldownTimer = 0.7f; //time for the block to be able to be used again
    protected float blockDelay;
    protected float blockCooldownTime;
    protected const float blockRemoveCooldown = 0.3f; //how long a block lasts for
    protected bool canBlock;
    protected float swingDelay;
    protected float swingRemoveCooldown;
    protected float attackRemoveCooldown;
    protected float attackDelay;
    protected float respawnDelay;
    protected float respawnTimer;
    protected float specialTimer;
    protected bool chargeAttack;



    protected SwingObject swingobject;
    protected BlockObject blockobject;
    protected PlayerBase pb;

    public void Start()
    {
        pb = GetComponent<PlayerBase>();
        swingobject = GetComponentInChildren<SwingObject>();
        swingobject.Setup(basicAttackDamage, GetComponent<CapsuleCollider>(), pb);
        swingobject.gameObject.SetActive(false);

        blockobject = GetComponentInChildren<BlockObject>();
        blockobject.gameObject.SetActive(false);

        pb.animator.SetInteger("AttackState", -1);

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
        else if (swingobject.gameObject.activeSelf)
        {
            swingobject.gameObject.SetActive(false);
            pb.animator.SetInteger("AttackState", -1);

        }

        if (blockCooldownTime > 0)
        {
            blockCooldownTime -= 1 * Time.deltaTime;
            canBlock = false;
        }

        if (blockCooldownTime <= 0)
        {
            canBlock = true;
            blockobject.gameObject.SetActive(false);
        }

        if (blockDelay > 0)
        {
            blockDelay -= 1 * Time.deltaTime;
        }
        if (blockDelay <= 0)
        {
            if (blocking)
            {
                blockobject.gameObject.SetActive(false);
                blocking = false;
                GetComponent<PhotonView>().RPC("RPC_DoBlock", PhotonTargets.Others, false);
            }
        }
        charUpdate();
    }

    #region overridable methods

    public virtual void charUpdate()
    {
    }

    public virtual void Attack(Vector2 dir)
    {
        CapsuleCollider coll = pb.playerController.capsule;

        int punchType = 1;

        if (dir.y == 1)
            punchType = 3;
        if (dir.y == -1)
            punchType = 2;

        pb.animator.SetInteger("AttackState", punchType);
        pb.RPC_DoPunch(punchType, dir);

        GetComponent<PhotonView>().RPC("RPC_DoPunch", PhotonTargets.Others, punchType, dir);

        float rangeside = coll.radius;
        float rangeUpDown = coll.height / 3;

        swingobject.gameObject.SetActive(true);
        swingobject.dir = dir;
        swingobject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + dir.y * rangeUpDown * rangeModifier, gameObject.transform.position.z + dir.x * rangeside * rangeModifier);
        swingobject.dmg = basicAttackDamage;
        swingobject.type = 0;
        swingDelay = swingRemoveCooldown;
        attackDelay = attackRemoveCooldown;
    }


    public virtual void Block()
    {
        blockobject.gameObject.SetActive(true);
        blockDelay = blockRemoveCooldown;
        blockCooldownTime = blockCooldownTimer;
        blocking = true;
        GetComponent<PhotonView>().RPC("RPC_DoBlock", PhotonTargets.Others, true);
    }

    public virtual void SpecialAttack()
    {

    }

    public virtual void Jump()
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
        set { this.specialCounter = Mathf.Clamp(value, 0, 150); }
    }

    public bool isBlocking
    {
        get { return blocking; }
        set { blocking = value; }
    }

    public float AttackCooldown
    {
        get { return attackRemoveCooldown; }
        set { attackRemoveCooldown = value; }
    }

    public float AttackDelay
    {
        get { return attackDelay; }
    }

    public float SwingCooldown
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

    public bool IsStunned
    {
        get { return stunned; }
        set { stunned = value; }
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
        return canBlock;
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
