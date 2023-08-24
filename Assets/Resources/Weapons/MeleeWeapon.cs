using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new_base_melee_weapon", menuName = "ScriptableObjects/Weapons/MeleeWeapon")]
public class MeleeWeapon : Weapon
{
    public override void Attack(Stats stats, Character target)
    {
        base.Attack(stats, target);
    }
}
