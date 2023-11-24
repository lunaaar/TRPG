using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOESpell : Spell
{
    List<GridTile> attackTiles = new List<GridTile>();

    [Tooltip("How many tiles in radius is the AOE spell?")]
    public int aoeRadius;

    public AOESpell()
    {
        name = "New AOE Spell";
        range = 0;
        uses = 3;
        actionTargets = ActionTargets.AOE;
        damageType = DamageType.Holy;

        aoeRadius = 3;
    }

    public override List<GridTile> showActionRange(List<GridTile> movementTiles, GridTile start, int movementRange)
    {
        //TODO: write this lol.
        
        return base.showActionRange(movementTiles, start, movementRange);
    }
}
