using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : ScriptableObject
{
    public new string name;
    public int attackRange;
    public int weaponDamage;

    public virtual void Attack(Character target)
    {
        target.currentHealth -= weaponDamage;

        target.updateHealthBar();
    }
}
