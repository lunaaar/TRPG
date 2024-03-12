using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "new_single_enemy_spell", menuName = "Actions/Spell/SingleEnemySpell")]
public class SingleEnemySpell : Spell
{
    /*.
        Single Enemy Spell:
        -------------------
        This will  
     */

    List<GridTile> attackTiles = new List<GridTile>();
    List<GridTile> surroundingTiles = new List<GridTile>();

    [Tooltip("How close can a target be for you to hit them with this weapon in tiles")]
    public int closeRange;

    public SingleEnemySpell()
    {
        name = "New Single Enemy Spell";
        range = 3;
        uses = 10;
        actionTargets = ActionTargets.SingleEnemy;
        damageType = DamageType.Necrotic;
    }

    public override void performAction(Character caster, Character target)
    {
        //TODO: DO THIS 
        
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
