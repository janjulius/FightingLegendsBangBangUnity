using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Willem : Character {

    public Willem()
    {
        name = "Willem";
        nameAfter = "the Snowman";
        AttackCooldown = 0.5;
        SwingCooldown = 0.1;
        BasicAttackDamage = 10;
        rangeModifier = 1;
    }
    public override void SpecialAttack()
    {
        base.SpecialAttack();
    }
}
