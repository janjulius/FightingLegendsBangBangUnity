using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fred : Character {

    public Fred()
    {
        name = "Fred";
        nameAfter = "der Goblin";
        AttackCooldown = 0.5f;
        SwingCooldown = 0.1f;
        BasicAttackDamage = 10;
        rangeModifier = 1;
        specialCounterThreshHold = 100;
    }
}
