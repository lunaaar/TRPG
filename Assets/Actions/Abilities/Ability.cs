using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Ability : Action
{
    [Header("Ability Specific Settings")]
    public ActionTargets targets;

    public Ability()
    {
        name = "New Spell";
        range = 3;
        uses = 5;

        actionType = ActionType.Ability;
    }

    public override int performAction(Character caster, Character target, bool justCalculate)
    {
        return base.performAction(caster, target, justCalculate);
    }

    public override List<GridTile> showActionRange(List<GridTile> movementTiles, GridTile start, int movementRange, string casterAlignment, bool justCalculate)
    {
        return base.showActionRange(movementTiles, start, movementRange, casterAlignment, justCalculate);
    }
}
