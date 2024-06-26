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

    public override int performAction(Character caster, Character target, bool justCalculate)
    {
        var damageTaken = caster.characterStats.contains("Magic") + damage - target.characterStats.contains("Resistance");

        if (!justCalculate)
        {
            target.characterStats.SetStats("currentHealth", Mathf.Max(target.characterStats.contains("currentHealth") - damageTaken, 0));
            target.updateHealthBar();

            DamageDisplay.create(damageTaken, target.transform.position, Color.red);
        }

        return damageTaken;
    }

    public override List<GridTile> showActionRange(List<GridTile> movementTiles, GridTile start, int movementRange, string casterAlignment, bool justCalculate)
    {
        return base.showActionRange(movementTiles, start, movementRange, casterAlignment, justCalculate);
    }

}
