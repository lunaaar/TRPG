using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "new_single_friendly_spell", menuName = "Actions/Spell/SingleFriendySpell")]
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

    public override int performAction(Character caster, Character target, bool justCalculate)
    {
        return base.performAction(caster, target, justCalculate);
    }

    public override List<GridTile> showActionRange(List<GridTile> movementTiles, GridTile start, int movementRange, string casterAlignment, bool justCalculate)
    {
        attackTiles.Clear();

        int step = 0;

        List<GridTile> attackTilesToCheck = new List<GridTile>(movementTiles);

        while (step < range)
        {
            surroundingTiles.Clear();

            foreach (var tile in attackTilesToCheck)
            {
                surroundingTiles.AddRange(GameManager.instance.pathFinder.getNeighbourAttackTiles(tile));
            }

            surroundingTiles = surroundingTiles.Distinct().ToList();

            foreach (var tile in surroundingTiles)
            {
                if (tile.status.Equals("Enemy") && !tile.Equals(start))
                {
                    continue;
                }
                else if (tile.status.Equals("Friendly"))
                {
                    attackTiles.Add(tile);
                }
                else
                {
                    attackTilesToCheck.Add(tile);
                }

                var path = GameManager.instance.pathFinder.findPath(start, tile);
                
                //TODO: This needs to be fixed
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

        if (justCalculate)
        {
            return attackTiles;
        }

        foreach (var tile in attackTiles)
        {
            if (tile.status.Equals("Friendly"))
            {
                //CursorMovement.instance.attackRangeTilemap.SetTile(tile.gridPosition, CursorMovement.instance.friendlyTileActive);
                MapManager.instance.floorTilemaps[tile.gridPosition.z].SetColor(tile.gridPosition, GameManager.instance.friendlyFullColor);
            }
            else if (GameManager.instance.pathFinder.findPath(start, tile).Sum(t => t.movementPenalty) == range + movementRange)
            {
                //CursorMovement.instance.attackRangeTilemap.SetTile(tile.gridPosition, CursorMovement.instance.friendlyTileEmpty);
                MapManager.instance.floorTilemaps[tile.gridPosition.z].SetColor(tile.gridPosition, GameManager.instance.friendlyEmptyColor);
            }
        }

        return attackTiles;
    }
}
