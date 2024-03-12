using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "new_aoe_spell", menuName = "Actions/Spell/AOE")]
public class AOESpell : Spell
{
    List<GridTile> attackTiles = new List<GridTile>();
    PathFinder pathFinder = new PathFinder();

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

    public override void performAction(Character caster, Character target)
    {
        foreach(GridTile tile in attackTiles.Distinct())
        {
            if(MapManager.instance.map[tile.gridPosition].status == "Enemy")
            {
                target = GameManager.instance.getCharacterAt(tile.gridPosition);

                if (target == null) continue;
                
                var damageTaken = (caster.characterStats.contains("Magic") + damage) - target.characterStats.contains("Resist");

                //? Mathf.Max prevents the target from taking negative damage and actually healing.
                target.characterStats.SetStats("currentHealth", Mathf.Max(target.characterStats.contains("currentHealth") - damageTaken, 0));
                target.updateHealthBar();
            }
        }
    }

    public override List<GridTile> showActionRange(List<GridTile> movementTiles, GridTile end, int movementRange)
    {
        //CursorMovement.instance.attackRangeTilemap.ClearAllTiles();

        var path = pathFinder.findTruePath(MapManager.instance.map[CursorMovement.instance.selectedCharacter.gridPosition], end);

        if (path.Count <= 0)
        {
            return new List<GridTile>();
        }

        /**
        //? CODE FOR SHOWING DEBUG ARROW RANGE.
        foreach (GridTile gt in path)
        {
            CursorMovement.instance.debug_arrow_tilemap.SetTile(gt.gridPosition, debug_arrow);
        }

        //test.transform.position = CursorMovement.instance.tilemap.GetCellCenterWorld(path[Mathf.Max(path.Count - (range + 1), 0)].gridPosition);

        //var status = path[Mathf.Max(path.Count - range, 0)].status;

        if (!status.Equals("NotOccupied") || !status.Equals("Objective"))
        {
            Debug.Log(status);
            return new List<GridTile>();
        }*/

        //MapManager.instance.resetAttackTiles();

        if (path.Count <= range + CursorMovement.instance.selectedCharacter.movementRange)
        {
            attackTiles = pathFinder.getTilesInRange(end, aoeRadius);

            foreach (GridTile tile in attackTiles.Distinct())
            {
                if (tile.status.Equals("Water") || tile.status.Equals("Obstacle"))
                {
                    continue;
                }
                
                if (tile.status.Equals("Enemy"))
                {
                    //CursorMovement.instance.attackRangeTilemap.SetTile(tile.gridPosition, CursorMovement.instance.attackTileActive);
                    MapManager.instance.floorTilemaps[tile.gridPosition.z].SetColor(tile.gridPosition, GameManager.instance.attackFullColor);
                }
                else
                {
                    //CursorMovement.instance.attackRangeTilemap.SetTile(tile.gridPosition, CursorMovement.instance.attackTileEmpty);
                    MapManager.instance.floorTilemaps[tile.gridPosition.z].SetColor(tile.gridPosition, GameManager.instance.attackEmptyColor);
                }
            }
        }

        return attackTiles;
    }
}
