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

    public override void performAction(Character caster, Character target)
    {
        target.listOfModifications.Add(new Modification(name, caster, target, this, duration));

        Debug.Log("CRIT: "+ boostAmount);

        target.characterStats.SetStats("Attack", target.characterStats.contains("Attack") * boostAmount);
    }
    public override void reapplyAction(Character caster, Character target)
    {
        var total = target.characterStats.contains("Attack") * boostAmount;
        target.characterStats.SetStats("Attack", total);
    }


    public override List<GridTile> showActionRange(List<GridTile> movementTiles, GridTile start, int movementRange)
    {
        return base.showActionRange(movementTiles, start, movementRange);
    }
}
