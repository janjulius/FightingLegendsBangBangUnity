﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    internal Character currentCharacter;
    internal Health healthController;
    internal PlayerControls Keys;
    internal PlayerController playerController;
    internal PhotonPlayer netPlayer;
    internal GamePanelContainer gpc;
    public GameObject attackParticles;
    public GameObject swingObject;
    public GameObject blockObject;
    public GameObject PlayerNameObject;
    public GameObject PlayerHitParticle;
    public GameObject PlayerHealParticle;
    internal Animator animator;
    internal GameObject playerBody;
    internal PhotonView photonViewer;
    internal Vector3 SpawnPoint;

    internal Color myColor;
    internal bool CanNotMove;
    internal bool InMenus;
    internal bool frozen;
    internal ParticleSystem HitParticleSystem;
    internal ParticleSystem HealParticleSystem;

    internal float stunDuration;


    private void Awake()
    {
        gameObject.AddComponent<PlayerControlsKeyBoard>();
        photonViewer = GetComponent<PhotonView>();
        Keys = GetComponent<PlayerControls>();
        currentCharacter = GetComponent<Character>();
        healthController = GetComponent<Health>();
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();

        myColor = new Color((float)photonViewer.owner.CustomProperties["pColorR"], (float)photonViewer.owner.CustomProperties["pColorG"], (float)photonViewer.owner.CustomProperties["pColorB"]);
        netPlayer = photonViewer.owner;
        gpc = FindObjectOfType<GamePanelContainer>();
        gpc.playerPanels.Find(x => x.photonPlayer == netPlayer).playerBase = this;
        gpc.playerPanels.Find(x => x.photonPlayer == netPlayer).UpdateUI();
        playerBody = transform.Find("pivot").gameObject;
        GameManager.Instance.Players.Add(this);
        if (PhotonNetwork.isMasterClient)
            ScoreManager.Instance.AddPlayer(netPlayer, this);

        attackParticles = Instantiate(attackParticles, playerBody.transform, false);
        swingObject = Instantiate(swingObject, playerBody.transform, false);
        blockObject = Instantiate(blockObject, playerBody.transform, false);
        PlayerHitParticle = Instantiate(PlayerHitParticle, playerBody.transform, false);
        HitParticleSystem = PlayerHitParticle.GetComponent<ParticleSystem>();

        if (PlayerHealParticle != null)
        {
            PlayerHealParticle = Instantiate(PlayerHealParticle, playerBody.transform, false);
            HealParticleSystem = PlayerHealParticle.GetComponent<ParticleSystem>();
        }

        PlayerNameObject = Instantiate(PlayerNameObject);

        PlayerNameObject.GetComponent<PlayerName>().SetTarget(this, myColor);
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
        if (!photonViewer.isMine)
            return;

        CanNotMove = currentCharacter.IsStunned || InMenus || healthController.death || frozen;

        if (stunDuration > 0 && currentCharacter.IsStunned)
            stunDuration -= Time.deltaTime;

        if (stunDuration <= 0 && currentCharacter.IsStunned)
            currentCharacter.IsStunned = false;

        Keys.ControlUpdate();

        playerController.PlayerUpdate();
    }

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
        Debug.Log("Blocking playerbase");
        if (currentCharacter.CanBlock())
            currentCharacter.Block();
    }

    public void AddKnockBack(Vector2 dir, float force)
    {
        force /= 2;

        playerController.KnockBack = dir * force;
    }

    public void OnDestroy()
    {
        if (PhotonNetwork.isMasterClient)
            ScoreManager.Instance.PlayerLost(netPlayer);
    }

    #region playerRPCS
    [PunRPC]
    public void RPC_DoPunch(int a, Vector2 dir)
    {
        if (a > -1 && a <= 3)
        {
            Vector3 vec = new Vector3(0, dir.y, dir.x);
            ParticleSystem sys = attackParticles.GetComponent<ParticleSystem>();
            var mainModule = sys.main;
            mainModule.startSpeedMultiplier = (playerController.capsule.height * playerController.capsule.radius) * 0.8f * 7f;

            attackParticles.transform.rotation = Quaternion.LookRotation(vec);
            sys.Play();

        }
    }

    [PunRPC]
    public void RPC_DoBlock(bool a)
    {
        blockObject.SetActive(a);
    }

    [PunRPC]
    public void RPC_DoHeal(bool a)
    {
        PlayerHealParticle.SetActive(a);
    }

    [PunRPC]
    public void RPC_ChangePosition(Vector3 pos)
    {
        transform.position = pos;
    }

    [PunRPC]
    public void RPC_UpdateDirection(bool dir)
    {
        playerController.right = dir;
        playerBody.transform.eulerAngles = new Vector3(0, dir ? 0 : 180, 0);
    }

    [PunRPC]
    public void RPC_Stun(float dur)
    {
        print("getstunnend");
        stunDuration = dur;
        currentCharacter.IsStunned = true;
    }
    #endregion

}
