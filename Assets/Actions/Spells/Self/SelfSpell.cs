using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "new_self_spell", menuName = "Actions/Spell/SelfSpell")]
public class SelfSpell : Spell
{
    List<GridTile> attackTiles = new List<GridTile>();

    /**
     * Self Spell:
     * -------------------
     * This spell is any spell that whose only target is them selves.
     * 
     * As such, the showSpellRange will only light up a 
     * 
     */

    public SelfSpell()
    {
        name = "New Self Spell";
        range = 0;
        uses = 5;
        actionTargets = ActionTargets.Self;
        damageType = DamageType.Holy;
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
