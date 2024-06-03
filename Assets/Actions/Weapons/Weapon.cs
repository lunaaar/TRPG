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

        uses = 100000;

        actionType = ActionType.Weapon;
    }

    public override int performAction(Character caster, Character target, bool justCalculate)
    {
        //? Perform Action for a weapon is to do an Attack.

        //? Target.Health -= (Weapon.damage + Character.Attack) - Target.Defense;

        var damageTaken = (caster.characterStats.contains("Attack") + damage) - target.characterStats.contains("Defense");

        if (!justCalculate)
        {   
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
