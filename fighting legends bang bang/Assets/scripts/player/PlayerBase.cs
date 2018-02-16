using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    public Character currentCharacter;
    public Health healthController;
    public PlayerController playerController;
    public PhotonPlayer netPlayer;
    public GamePanelContainer gpc;
    private GameObject pc;

    private void Awake()
    {
        GameManager.Instance.Players.Add(this);
        PhotonView phov = GetComponent<PhotonView>();
        currentCharacter = GetComponent<Character>();
        netPlayer = phov.owner;
        gpc = FindObjectOfType<GamePanelContainer>();
        gpc.playerPanels.Find(x => x.photonPlayer == netPlayer).playerBase = this;
        playerController = GetComponent<PlayerController>();

        gpc.playerPanels.Find(x => x.photonPlayer == netPlayer).UpdateUI();
        PhotonNetwork.player.TagObject = gameObject;
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

    public void AddKnockBack(Vector3 dir, double power)
    {

    }

}
