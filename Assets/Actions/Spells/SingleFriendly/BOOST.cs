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

    public override void performAction(Character caster, Character target)
    {
        target.listOfModifications.Add(new Modification(name, caster, target, this, duration));

        Debug.Log("BOOST" + boostAmount);

        var total = target.characterStats.contains("Attack") + boostAmount;

        Debug.Log(total);

        target.characterStats.SetStats("Attack", total);
    }

    public override void reapplyAction(Character caster, Character target)
    {
        var total = target.characterStats.contains("Attack") + boostAmount;
        target.characterStats.SetStats("Attack", total);
    }

    /**public override void undoAction(Character caster, Character target)
    {
        target.characterStats.SetStats("Attack", target.characterStats.contains("Attack") - boostAmount);
    }*/

    public override List<GridTile> showActionRange(List<GridTile> movementTiles, GridTile start, int movementRange)
    {
        return base.showActionRange(movementTiles, start, movementRange);
    }
}
