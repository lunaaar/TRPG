using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWeapon : Weapon
{
    public new int attackRange;

    public void SetUp()
    {

    }

    public override void Attack(Stats stats, Character target)
    {
        base.Attack(stats, target);
    }
}
