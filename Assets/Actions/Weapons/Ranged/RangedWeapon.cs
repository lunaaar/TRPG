using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//TODO: Remove this.
[CreateAssetMenu(fileName = "new_ranged_weapon", menuName = "Actions/Weapons/RangedWeapon")]
public class RangedWeapon : Weapon
{
    List<GridTile> attackTiles = new List<GridTile>();
    List<GridTile> surroundingTiles = new List<GridTile>();


    [Tooltip("How close can the enemy be for you to hit them with this weapon in tiles")]
    public int closestAttackRange;

    public RangedWeapon()
    {
        name = "New Ranged Weapon";
        range = 2;
        damage = 1;
        closestAttackRange = 2;
        actionTargets = ActionTargets.SingleEnemy;
    }

    public override void performAction(Character caster, Character target)
    {
        base.performAction(caster, target);
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
                if (tile.status.Equals("Friendly") && !tile.Equals(start))
                {
                    continue;
                }
                else if (tile.status.Equals("Enemy"))
                {
                    attackTiles.Add(tile);
                }
                else
                {
                    attackTilesToCheck.Add(tile);
                }

                var path = pathFinder.findPath(start, tile);
                if (path.Sum(t => t.movementPenalty) >= closestAttackRange + movementRange
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
            if (tile.status.Equals("Enemy"))
            {
                //CursorMovement.instance.attackRangeTilemap.SetTile(tile.gridPosition, CursorMovement.instance.attackTileActive);
                MapManager.instance.floorTilemaps[tile.gridPosition.z].SetColor(tile.gridPosition, GameManager.instance.attackFullColor);
            }
            else if (pathFinder.findPath(start, tile).Sum(t => t.movementPenalty) == range + movementRange)
            {
                //CursorMovement.instance.attackRangeTilemap.SetTile(tile.gridPosition, CursorMovement.instance.attackTileEmpty);
                MapManager.instance.floorTilemaps[tile.gridPosition.z].SetColor(tile.gridPosition, GameManager.instance.attackEmptyColor);
            }
        }

        return attackTiles;
    }


}
