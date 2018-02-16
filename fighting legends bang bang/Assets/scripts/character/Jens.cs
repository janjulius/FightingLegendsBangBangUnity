using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jens : Character {

    public Jens()
    {
        name = "Jens";
        nameAfter = "the Pirate Panda";
        AttackCooldown = 0.5;
        SwingCooldown = 0.1;
        BasicAttackDamage = 10;
    }
    public override void Attack(int dir)
    {
        base.Attack(dir);
    }

    public override void SpecialAttack()
    {
        base.SpecialAttack();
    }
}
