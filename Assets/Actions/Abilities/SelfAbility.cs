using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new_self_ability", menuName = "Actions/Ability/SelfAbility")]
public class SelfAbility : Ability
{
    List<GridTile> attackTiles = new List<GridTile>();

    public SelfAbility()
    {
        name = "New Self Ability";
        range = 0;
        uses = 3;

        targets = ActionTargets.Self;
    }

    public override int performAction(Character caster, Character target, bool justCalculate)
    {
        return base.performAction(caster, target, justCalculate);
    }

    public override List<GridTile> showActionRange(List<GridTile> movementTiles, GridTile start, int movementRange, string casterAlignment, bool justCalculate)
    {
        attackTiles.Clear();
        attackTiles.Add(start);

        if (justCalculate)
        {
            return attackTiles;
        }

        //CursorMovement.instance.attackRangeTilemap.SetTile(start.gridPosition, CursorMovement.instance.friendlyTileActive);
        MapManager.instance.floorTilemaps[start.gridPosition.z].SetColor(start.gridPosition, GameManager.instance.friendlyFullColor);
        return attackTiles;
    }

}
