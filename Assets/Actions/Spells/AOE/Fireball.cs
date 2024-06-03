using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Fireball : AOESpell
{
    List<GridTile> attackTiles = new List<GridTile>();

    public Fireball()
    {
        name = "New Fireball";
        range = 3;
        uses = 2;
        actionTargets = ActionTargets.AOE;
        damageType = DamageType.Holy;

        aoeRadius = 2;
    }

    public override int performAction(Character caster, Character target, bool justCalculate)
    {
        string targetAlignment = "";
        int totalDamage = 0;

        switch (caster.alignment)
        {
            case (Character.AlignmentStatus.Friendly):
            case (Character.AlignmentStatus.Neutral):
                targetAlignment = "Enemy";
                break;
            case (Character.AlignmentStatus.Enemy):
                targetAlignment = "Friendly";
                break;
        }

        foreach (GridTile tile in attackTiles.Distinct())
        {
            if (MapManager.instance.map[tile.gridPosition].status == targetAlignment)
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
}
