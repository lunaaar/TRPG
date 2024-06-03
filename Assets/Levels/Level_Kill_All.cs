using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new_kill_all_level", menuName = "Levels/KillAllLevel")]
public class Level_Kill_All : Level
{
    public Level_Kill_All()
    {
        levelType = LevelType.KillAll;
    }

    public override void processEndOfTurn()
    {
        Debug.Log("Kill All End of Turn.");
    }

    public override bool checkWinCondition()
    {
        /*
         * Win Condition:
         * --------------
         * Returns true if all enemies are dead.
         */
        foreach(Character c in GameManager.instance.listOfAllEnemies)
        {
            if(c.characterStats.contains("currentHealth") > 0)
            {
                return false;
            }
        }

        return true;
    }
}
