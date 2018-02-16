using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocky : Character {

    public Rocky()
    {
        name = "Rocky";
        nameAfter = "the Raccoon";
        AttackCooldown = 0.5;
        SwingCooldown = 0.1;
        BasicAttackDamage = 10;
    }
    public override void Attack(Vector2 dir)
    {
        base.Attack(dir);
    }

    public override void SpecialAttack()
    {
        base.SpecialAttack();
    }
}
