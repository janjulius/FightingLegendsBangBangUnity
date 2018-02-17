using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    public Character currentCharacter;
    public Health healthController;
    public PlayerControls Keys;
    public PlayerController playerController;
    public PhotonPlayer netPlayer;
    public GamePanelContainer gpc;
    public GameObject attackParticles;
    private GameObject pc;
    public Animator animator;
    public GameObject playerBody;
    public PhotonView photonViewer;


    private void Awake()
    {
        gameObject.AddComponent<PlayerControlsKeyBoard>();
        playerBody = transform.Find("pivot").gameObject;

        GameManager.Instance.Players.Add(this);
        photonViewer = GetComponent<PhotonView>();
        Keys = GetComponent<PlayerControls>();
        currentCharacter = GetComponent<Character>();
        netPlayer = photonViewer.owner;
        gpc = FindObjectOfType<GamePanelContainer>();
        gpc.playerPanels.Find(x => x.photonPlayer == netPlayer).playerBase = this;
        playerController = GetComponent<PlayerController>();
        animator = GetComponentInChildren<Animator>();
        gpc.playerPanels.Find(x => x.photonPlayer == netPlayer).UpdateUI();
        PhotonNetwork.player.TagObject = gameObject;

        attackParticles = Instantiate(attackParticles, playerBody.transform, false);

    }

    public void CheckWithinArena()
    {
        AreaManager manager = AreaManager.Instance;

        Vector3 arenaTopRight = new Vector3(manager.arenaBottemRight.x, manager.arenaTopLeft.y, manager.arenaBottemRight.z);
        Vector3 arenaBottemLeft = new Vector3(manager.arenaTopLeft.x, manager.arenaBottemRight.y, manager.arenaTopLeft.z);

        Rect arena = new Rect();
        arena.xMin = manager.arenaTopLeft.z;
        arena.yMin = manager.arenaTopLeft.y;
        arena.xMax = arenaTopRight.z;
        arena.yMax = arenaBottemLeft.y;

        var playerPoint = new Vector3(transform.position.z, transform.position.y, 0);

        if (!arena.Contains(playerPoint, true) && !healthController.death)
        {
            healthController.OnDeath();
        }
    }

    private void Update()
    {
        Keys.ControlUpdate();
        playerController.PlayerUpdate();
    }

    //dir -1 = right
    //dir 0 = left
    //dir 1 = up
    //dir 2 = down
    public void RegularAttack(Vector2 dir)
    {
        if (!currentCharacter.isBlocking)
            if (currentCharacter.CanAttack())
                currentCharacter.Attack(dir);
    }

    public void SpecialAttack()
    {
        if (!currentCharacter.isBlocking)
            if (currentCharacter.SpecialReady())
                currentCharacter.SpecialAttack();
    }

    public void Block()
    {
        if (currentCharacter.CanBlock())
            currentCharacter.Block();
    }

    public void AddKnockBack(Vector2 dir, float force)
    {
        force /= 2;

        playerController.KnockBack = dir * force;
    }

    public void TakeDamage(int damage)
    {
        healthController.Damage += damage;
    }

    public void TakeHealth(int damage)
    {
        healthController.HealthPoints -= damage;
    }

    #region playerRPCS




    [PunRPC]
    public void RPC_DoJump()
    {
        animator.SetTrigger("IsJumping");
    }

    [PunRPC]
    void RPC_DoRunning()
    {
        animator.SetBool("IsRunning", true);
    }

    [PunRPC]
    void RPC_StopRunning()
    {
        animator.SetBool("IsRunning", false);
    }

    [PunRPC]
    void RPC_IsGrounded(bool g)
    {
        animator.SetBool("IsGrounded", g);
    }

    [PunRPC]
    public void RPC_DoPunch(int a, Vector2 dir)
    {
        if (a > -1)
        {
            Vector3 vec = new Vector3(0, dir.y, dir.x);
            print(GetComponent<CapsuleCollider>().height * GetComponent<CapsuleCollider>().radius);
            ParticleSystem sys = attackParticles.GetComponent<ParticleSystem>();
            var mainModule = sys.main;
            mainModule.startSpeedMultiplier = (playerController.capsule.height * playerController.capsule.radius)*0.8f* 7f;

            attackParticles.transform.rotation = Quaternion.LookRotation(vec);
            sys.Play();

        }

        animator.SetInteger("AttackState", a);
    }

    [PunRPC]
    public void RPC_UpdateDirection(bool dir)
    {
        playerBody.transform.eulerAngles = new Vector3(0, dir ? 0 : 180, 0);
    }
    #endregion

}
