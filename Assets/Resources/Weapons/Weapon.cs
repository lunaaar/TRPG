using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new_base_weapon", menuName = "ScriptableObjects/Weapons/BaseWeapon")]
public class Weapon : ScriptableObject
{
    public new string name;
    public int attackRange;
    public int weaponDamage;

    public virtual void Attack(Stats stats, Character target)
    {
        //Current Health = Current Health - weaponDamage * Attack
        target.characterStats.SetStats("currentHealth", target.characterStats.contains("currentHealth")
                                       - weaponDamage * stats.contains("Attack"));
        target.updateHealthBar();
    }
}
