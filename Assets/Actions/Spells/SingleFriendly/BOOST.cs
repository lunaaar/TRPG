using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new_boost", menuName = "Actions/Spells/Single Friendly Spell/BOOST")]
public class BOOST : SingleFriendlySpell
{
    
    public int boostAmount;
    
    public BOOST()
    {
        name = "Boost";
        range = 3;
        closeRange = 1;
        uses = 2;

        actionTargets = ActionTargets.SingleAlly;
        damageType = DamageType.Holy;

        duration = 1;
        boostAmount = 3;
    }

    public override int performAction(Character caster, Character target, bool justCalculate)
    {
        target.listOfModifications.Add(new Modification(name, caster, target, this, duration));

        Debug.Log("BOOST" + boostAmount);

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
        int buffTotal = target.characterStats.contains("Attack") + boostAmount;
        target.characterStats.SetStats("Attack", buffTotal);

        return buffTotal;
    }

    /**public override void undoAction(Character caster, Character target)
    {
        target.characterStats.SetStats("Attack", target.characterStats.contains("Attack") - boostAmount);
    }*/

    public override List<GridTile> showActionRange(List<GridTile> movementTiles, GridTile start, int movementRange, string casterAlignment, bool justCalculate)
    {
        return base.showActionRange(movementTiles, start, movementRange, casterAlignment, justCalculate);
    }
}
