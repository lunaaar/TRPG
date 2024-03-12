using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//TODO: Remove this.
[CreateAssetMenu(fileName = "new_melee_weapon", menuName = "Actions/Weapons/MeleeWeapon")]
public class MeleeWeapon : Weapon
{
    /*.
        Melee Weapon:
        -------------
        Melee weapons function as one of the two core weapon types.
        They are extremely straight forward. They have an amount of damage, and can hit range tiles away.
        
        Examples of why range could be different count include:
        - Sword = 1 range
        - Spear = 2 range
     */
    
    
    List<GridTile> attackTiles = new List<GridTile>();
    List<GridTile> surroundingTiles = new List<GridTile>();

    public MeleeWeapon()
    {
        name = "New Melee Weapon";
        range = 1;
        damage = 1;

        actionTargets = ActionTargets.SingleEnemy;
    }

    public override void performAction(Character caster, Character target)
    {
        base.performAction(caster, target);
    }

    public override List<GridTile> showActionRange(List<GridTile> movementTiles, GridTile start, int movementRange)
    {
        PathFinder pathFinder = new PathFinder();

        //List<GridTile> attackTiles = new List<GridTile>();
        attackTiles.Clear();

        int step = 0;

        List<GridTile> attackTilesToCheck = new List<GridTile>(movementTiles);

        while (step < range)
        {
            //var surroundingTiles = new List<GridTile>();
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
                if ((path.Sum(t => t.movementPenalty) <= range + movementRange && path.Sum(t => t.movementPenalty) > movementRange) || path.Count == range + movementRange)
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
