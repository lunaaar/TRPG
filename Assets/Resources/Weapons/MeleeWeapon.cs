using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MeleeWeapon : Weapon
{
    public override void Attack(Stats stats, Character target)
    {
        base.Attack(stats, target);
    }
}
