using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Weapon : Action
{   
    public Weapon()
    {
        name = "New Weapon";
        range = 1;
        damage = 1;

        actionType = ActionType.Weapon;
    }

    public override void performAction(Stats stats, Character target)
    {
        //Perform Action for a weapon is to do an Attack.
        
        //Current Health = Current Health - weaponDamage * Attack
        target.characterStats.SetStats("currentHealth", target.characterStats.contains("currentHealth")
                                       - damage * stats.contains("Attack"));
        target.updateHealthBar();
    }

    public override List<GridTile> showActionRange(List<GridTile> movementTiles, GridTile start, int movementRange)
    {
        return base.showActionRange(movementTiles, start, movementRange);
    }
}
