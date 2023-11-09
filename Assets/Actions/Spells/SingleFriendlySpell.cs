using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SingleFriendlySpell : Spell
{
    /**
     * Single Friendly Spell:
     * -------------------
     * This will 
     * 
     */

    List<GridTile> attackTiles = new List<GridTile>();
    List<GridTile> surroundingTiles = new List<GridTile>();

    [Tooltip("How close can a target be for you to hit them with this weapon in tiles")]
    public int closeRange;

    public SingleFriendlySpell()
    {
        name = "New Single Friendly Spell";
        range = 3;
        closeRange = 2;
        uses = 10;
        actionTargets = ActionTargets.SingleAlly;
        damageType = DamageType.Necrotic;
    }

    public override void performAction(Stats stats, Character target)
    {
        base.performAction(stats, target);
    }

    public override List<GridTile> showActionRange(List<GridTile> movementTiles, GridTile start, int movementRange)
    {
        PathFinder pathFinder = new PathFinder();

        attackTiles.Clear();

        int step = 0;

        List<GridTile> attackTilesToCheck = new List<GridTile>(movementTiles);

        while (step < range)
        {
            surroundingTiles.Clear();

            foreach (var tile in attackTilesToCheck)
            {
                surroundingTiles.AddRange(pathFinder.getNeighbourAttackTiles(tile));
            }

            surroundingTiles = surroundingTiles.Distinct().ToList();

            foreach (var tile in surroundingTiles)
            {
                if (tile.status.Equals("Friendly"))
                {
                    attackTiles.Add(tile);
                }
                else
                {
                    attackTilesToCheck.Add(tile);
                }

                var path = pathFinder.findPath(start, tile);
                if (path.Sum(t => t.movementPenalty) >= closeRange + movementRange
                       && path.Sum(t => t.movementPenalty) <= range + movementRange)
                {
                    attackTiles.Add(tile);
                }

            }

            step++;
        }

        attackTiles = attackTiles.Distinct().ToList();
        attackTiles.Remove(start);

        foreach (var tile in attackTiles)
        {
            if (tile.status.Equals("Friendly"))
            {
                CursorMovement.instance.attackRangeTilemap.SetTile(tile.gridPosition, CursorMovement.instance.friendlyTile);
            }
        }

        return attackTiles;
    }
}
