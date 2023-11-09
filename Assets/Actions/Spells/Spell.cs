using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Spell : Action
{
    [Header("Spell Specific Settings")]
    
    public int uses;
    public DamageType damageType;

    public Spell()
    {
        name = "New Spell";
        range = 3;
        uses = 5;

        actionType = ActionType.Spell;
    }

    public override void performAction(Stats stats, Character target)
    {
        base.performAction(stats, target);
    }

    public override List<GridTile> showActionRange(List<GridTile> movementTiles, GridTile start, int movementRange)
    {
        return base.showActionRange(movementTiles, start, movementRange);
    }

}