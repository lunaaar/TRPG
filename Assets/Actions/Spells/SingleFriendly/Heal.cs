using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "new_heal", menuName = "Actions/Spells/Single Friendly Spell/Heal")]
public class Heal : SingleFriendlySpell
{
    public Heal()
    {
        name = "Heal";
        range = 3;
        closeRange = 1;
        uses = 3;
        actionTargets = ActionTargets.SingleAlly;
        damageType = DamageType.Holy;
    }

    public override int performAction(Character caster, Character target, bool justCalculate)
    {
        var healAmount = caster.characterStats.contains("Magic");

        if (!justCalculate)
        {
            target.characterStats.SetStats("currentHealth", Mathf.Min(target.characterStats.contains("MaxHealth"), healAmount + target.characterStats.contains("currentHealth")));
            target.updateHealthBar();

            DamageDisplay.create(healAmount, target.transform.position, Color.green);
        }

        return healAmount;
    }

    public override List<GridTile> showActionRange(List<GridTile> movementTiles, GridTile start, int movementRange, string casterAlignment, bool justCalculate)
    {
        return base.showActionRange(movementTiles, start, movementRange, casterAlignment, justCalculate);
    }
}
