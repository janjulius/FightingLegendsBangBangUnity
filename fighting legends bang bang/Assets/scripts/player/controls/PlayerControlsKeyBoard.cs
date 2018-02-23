using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlsKeyBoard : PlayerControls
{

    public override void ControlUpdate()
    {
        float horAxis = (Input.GetKey(PlayerNetwork.Instance.keyBindings[0]) ? -1 : 0) +
                        (Input.GetKey(PlayerNetwork.Instance.keyBindings[1]) ? 1 : 0);
        float verAxis = (Input.GetKey(PlayerNetwork.Instance.keyBindings[2]) ? 1 : 0) +
                        (Input.GetKey(PlayerNetwork.Instance.keyBindings[3]) ? -1 : 0);

        controlsNew.horizontal = horAxis;
        controlsNew.vertical = verAxis;
        controlsNew.attackButton = Input.GetKey(PlayerNetwork.Instance.keyBindings[4]);
        controlsNew.specialAttackButton = Input.GetKey(PlayerNetwork.Instance.keyBindings[5]);
        controlsNew.jumpButton = Input.GetKey(PlayerNetwork.Instance.keyBindings[7]);
        controlsNew.blockButton = Input.GetKey(PlayerNetwork.Instance.keyBindings[6]);

        base.ControlUpdate();
    }
}