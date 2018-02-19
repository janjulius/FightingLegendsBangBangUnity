using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlsKeyBoard : PlayerControls
{

    public override void ControlUpdate()
    {
        controlsNew.horizontal = Input.GetAxis("Horizontal");
        controlsNew.vertical = Input.GetAxis("Vertical");
        controlsNew.attackButton = Input.GetButton("RegularAttack");
        controlsNew.specialAttackButton = Input.GetButton("SpecialAttack");
        controlsNew.jumpButton = Input.GetButton("Jump");
        controlsNew.blockButton = Input.GetButton("Block");

        base.ControlUpdate();
    }
}