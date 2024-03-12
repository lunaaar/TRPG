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

    public override void performAction(Character caster, Character target)
    {
        foreach (GridTile tile in attackTiles.Distinct())
        {
            if (MapManager.instance.map[tile.gridPosition].status == "Enemy")
            {
                target = GameManager.instance.getCharacterAt(tile.gridPosition);

                if (target == null) continue;

                var damageTaken = (caster.characterStats.contains("Magic") + damage) - target.characterStats.contains("Resist");

                //? Mathf.Max prevents the target from taking negative damage and actually healing.
                target.characterStats.SetStats("currentHealth", Mathf.Max(target.characterStats.contains("currentHealth") - damageTaken, 0));
                target.updateHealthBar();
            }
        }
    }
}
