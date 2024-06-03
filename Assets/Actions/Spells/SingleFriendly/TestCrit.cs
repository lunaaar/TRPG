using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new_crit", menuName = "Actions/Spells/Single Friendly Spell/TESTCRIT")]
public class TestCrit : SingleFriendlySpell
{
    int boostAmount;

    public TestCrit()
    {
        name = "TEST CRIT";
        range = 2;
        closeRange = 1;
        uses = 1;

        actionTargets = ActionTargets.SingleAlly;
        damageType = DamageType.Fire;

        duration = 3;
        boostAmount = 2;
    }

    public override int performAction(Character caster, Character target, bool justCalculate)
    {
        target.listOfModifications.Add(new Modification(name, caster, target, this, duration));

        Debug.Log("CRIT: "+ boostAmount);

        int buffTotal = target.characterStats.contains("Attack") + boostAmount;

        Debug.Log(buffTotal);
        if (!justCalculate)
        {
            target.characterStats.SetStats("Attack", buffTotal);
        }

        return buffTotal;
    }
    public override int reapplyAction(Character caster, Character target)
    {
        var buffTotal = target.characterStats.contains("Attack") * boostAmount;
        target.characterStats.SetStats("Attack", buffTotal);

        return buffTotal;
    }


    public override List<GridTile> showActionRange(List<GridTile> movementTiles, GridTile start, int movementRange, string casterAlignment, bool justCalculate)
    {
        return base.showActionRange(movementTiles, start, movementRange, casterAlignment, justCalculate);
    }
}
