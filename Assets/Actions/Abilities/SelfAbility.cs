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

    public override void performAction(Stats stats, Character target)
    {
        base.performAction(stats, target);
    }

    public override List<GridTile> showActionRange(List<GridTile> movementTiles, GridTile start, int movementRange)
    {
        attackTiles.Clear();
        attackTiles.Add(start);

        CursorMovement.instance.attackRangeTilemap.SetTile(start.gridPosition, CursorMovement.instance.friendlyTileActive);
        return attackTiles;
    }

}
