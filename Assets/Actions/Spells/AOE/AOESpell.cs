using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "new_aoe_spell", menuName = "Actions/Spell/AOE")]
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

    public override int performAction(Character caster, Character target, bool justCalculate)
    {
        int totalDamage = 0;
        string targetAlignment = "";

        switch (caster.alignment)
        {
            case (Character.AlignmentStatus.Friendly): case(Character.AlignmentStatus.Neutral):
                targetAlignment = "Enemy";
                break;
            case (Character.AlignmentStatus.Enemy):
                targetAlignment = "Friendly";
                break;
        }

        
        foreach(GridTile tile in attackTiles.Distinct())
        {
            if(MapManager.instance.map[tile.gridPosition].status == targetAlignment)
            {
                target = GameManager.instance.getCharacterAt(tile.gridPosition);

                if (target == null) continue;
                
                var damageTaken = (caster.characterStats.contains("Magic") + damage) - target.characterStats.contains("Resist");

                totalDamage += damageTaken;

                if (!justCalculate)
                {
                    //? Mathf.Max prevents the target from taking negative damage and actually healing.
                    target.characterStats.SetStats("currentHealth", Mathf.Max(target.characterStats.contains("currentHealth") - damageTaken, 0));
                    target.updateHealthBar();

                    DamageDisplay.create(damageTaken, target.transform.position, Color.red);
                }
            }
        }

        return totalDamage;
    }

    public override List<GridTile> showActionRange(List<GridTile> movementTiles, GridTile end, int movementRange, string casterAlignment, bool justCalculate)
    {
        //CursorMovement.instance.attackRangeTilemap.ClearAllTiles();

        var path = GameManager.instance.pathFinder.findTruePath(MapManager.instance.map[CursorMovement.instance.selectedCharacter.gridPosition], end);

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
            attackTiles = GameManager.instance.pathFinder.getTilesInRange(end, aoeRadius);

            if (justCalculate)
            {
                return attackTiles;
            }

            foreach (GridTile tile in attackTiles.Distinct())
            {
                if (tile.status.Equals("Water") || tile.status.Equals("Obstacle"))
                {
                    continue;
                }
                
                if (tile.status.Equals(GameManager.instance.getOtherAlignemnt(casterAlignment)))
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
