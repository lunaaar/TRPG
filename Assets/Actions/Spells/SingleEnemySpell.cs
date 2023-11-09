using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "new_single_enemy_spell", menuName = "Actions/Spell/SingleEnemySpell")]
public class SingleEnemySpell : Spell
{
    /**
     * Single Enemy Spell:
     * -------------------
     * This will 
     * 
     */

    public SingleEnemySpell()
    {
        name = "New Single Enemy Spell";
        range = 3;
        uses = 10;
        actionTargets = ActionTargets.SingleEnemy;
        damageType = DamageType.Necrotic;
    }

    public override List<GridTile> showActionRange(List<GridTile> movementTiles, GridTile start, int movementRange)
    {
        return base.showActionRange(movementTiles, start, movementRange);
    }
}
