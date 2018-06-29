using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tjeerd : Character
{
    public Tjeerd()
    {
        name = "Tjeerd";
        nameAfter = "the Rugby God";
        AttackCooldown = 0.5f;
        SwingCooldown = 0.1f;
        BasicAttackDamage = 10;
        specialCounterThreshHold = 100;
        rangeModifier = 1;
    }
    public override void Attack(Vector2 dir)
    {
        base.Attack(dir);
    }

    public override void SpecialAttack()
    {
        print("special pogchamp");
    }
}
