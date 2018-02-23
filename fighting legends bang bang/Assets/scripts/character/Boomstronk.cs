using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomstronk : Character {

    public Boomstronk()
    {
        name = "Boomstronk";
        nameAfter = "the Treestump";
        AttackCooldown = 0.5f;
        SwingCooldown = 0.1f;
        BasicAttackDamage = 10;
        rangeModifier = 1;
        specialCounterThreshHold = 100;
    }
}
