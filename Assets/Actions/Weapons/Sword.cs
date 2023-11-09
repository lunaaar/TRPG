using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "new_sword", menuName = "Actions/Weapons/Melee Weapons/Sword")]
public class Sword : MeleeWeapon
{
    public override void performAction(Stats stats, Character target)
    {
        //TODO: Work out how specifically we want to calculate dmg and how Swords specifically should do it.

        /*.
            * Current Formula is just doing as much dmg as the actionCharacter has attack.
            * Target.Health = Target.Health - ActionCharacter.Attack;
         */
        
        target.characterStats.SetStats("currentHealth", target.characterStats.contains("currentHealth")
                                       - damage * stats.contains("Attack"));
        target.updateHealthBar();
    }

    public override List<GridTile> showActionRange(List<GridTile> movementTiles, GridTile start, int movementRange)
    {
        return base.showActionRange(movementTiles, start, movementRange);
    }
}
