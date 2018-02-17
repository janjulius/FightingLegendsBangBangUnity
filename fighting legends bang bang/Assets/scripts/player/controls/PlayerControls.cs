using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerControls : MonoBehaviour
{
    protected struct Controls
    {
        public float horizontal;
        public float vertical;

        public bool attackButton;
        public bool attackButtonDown;

        public bool specialAttackButton;
        public bool specialAttackButtonDown;

        public bool jumpButton;
        public bool jumpButtonDown;

        public bool blockButton;
        public bool blockButtonDown;
    }

    private PlayerBase pb;

    protected Controls controls = new Controls();
    protected Controls controlsNew = new Controls();
    protected Controls controlsOld = new Controls();

    void Start()
    {
        pb = GetComponent<PlayerBase>();
    }

    public virtual void ControlUpdate()
    {
        controls.horizontal = controlsNew.horizontal;
        controls.vertical = controlsNew.vertical;
        controls.attackButton = controlsNew.attackButton;
        controls.specialAttackButton = controlsNew.specialAttackButton;
        controls.jumpButton = controlsNew.jumpButton;
        controls.blockButton = controlsNew.blockButtonDown;
        controls.attackButtonDown = controlsNew.attackButton && (controlsNew.attackButton != controlsOld.attackButton);
        controls.specialAttackButtonDown = controlsNew.specialAttackButton && (controlsNew.specialAttackButton != controlsOld.specialAttackButton);
        controls.jumpButtonDown = controlsNew.jumpButton && (controlsNew.jumpButton != controlsOld.jumpButton);
        controls.blockButtonDown = controlsNew.blockButton && (controlsNew.blockButton != controlsOld.blockButton);


        controlsOld.horizontal = controlsNew.horizontal;
        controlsOld.vertical = controlsNew.vertical;
        controlsOld.attackButton = controlsNew.attackButton;
        controlsOld.specialAttackButton = controlsNew.specialAttackButton;
        controlsOld.jumpButton = controlsNew.jumpButton;
        controlsOld.blockButton = controlsNew.blockButton;
    }

    public float Horizontal()
    {
        return Mathf.Clamp(controls.horizontal, -1f, 1f);
    }

    public float Vertical()
    {
        return Mathf.Clamp(controls.vertical, -1f, 1f);
    }

    public bool AttackButton()
    {
        return controls.attackButton;
    }

    public bool AttackButtonDown()
    {
        return controls.attackButtonDown;
    }

    public bool SpecialAttackButton()
    {
        return controls.specialAttackButton;
    }

    public bool SpecialAttackButtonDown()
    {
        return controls.specialAttackButtonDown;
    }

    public bool JumpButton()
    {
        return controls.jumpButton;
    }

    public bool JumpButtonDown()
    {
        return controls.jumpButtonDown;
    }

    public bool BlockButton()
    {
        return controls.blockButton;
    }

    public bool BlockButtonDown()
    {
        return controls.blockButtonDown;
    }
}
