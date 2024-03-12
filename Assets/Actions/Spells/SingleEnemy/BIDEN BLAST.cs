using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new_blast", menuName = "Actions/Spells/Single Enemy Spell/Blast")]
public class BIDENBLAST : SingleEnemySpell
{
    public BIDENBLAST()
    {
        name = "Blast";
        range = 2;
        closeRange = 1;
        uses = 5;
        damage = 2;

        actionTargets = ActionTargets.SingleEnemy;
        damageType = DamageType.Necrotic;
    }

    public override void performAction(Character caster, Character target)
    {
        var damageTaken = caster.characterStats.contains("Magic") + damage - target.characterStats.contains("Resistance");

        target.characterStats.SetStats("currentHealth", Mathf.Max(target.characterStats.contains("currentHealth") - damageTaken, 0));
        target.updateHealthBar();
    }

    public override List<GridTile> showActionRange(List<GridTile> movementTiles, GridTile start, int movementRange)
    {
        return base.showActionRange(movementTiles, start, movementRange);
    }

}
