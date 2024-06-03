using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "new_sword", menuName = "Actions/Weapons/Melee Weapons/Sword")]
public class Sword : MeleeWeapon
{
    public override int performAction(Character caster, Character target, bool justCalculate)
    {
        //TODO: Work out how specifically we want to calculate dmg and how Swords specifically should do it.

        /*?
            Current Formula:
            ================
            Target.Health -= (Weapon.damage + Character.Attack) - Target.Defense;
         */

        int damageTaken = (caster.characterStats.contains("Attack") + damage) - target.characterStats.contains("Defense");

        if (!justCalculate)
        {
            //? Mathf.Max prevents the target from taking negative damage and actually healing.
            target.characterStats.SetStats("currentHealth", Mathf.Max(target.characterStats.contains("currentHealth") - damageTaken, 0));
            target.updateHealthBar();

            DamageDisplay.create(damageTaken, target.transform.position, Color.red);
        }

        return damageTaken;
    }

    public override List<GridTile> showActionRange(List<GridTile> movementTiles, GridTile start, int movementRange, string casterAlignment, bool justCalculate)
    {
        return base.showActionRange(movementTiles, start, movementRange, casterAlignment, justCalculate);
    }
}
