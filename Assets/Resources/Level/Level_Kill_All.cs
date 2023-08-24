using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new_kill_all_level", menuName = "ScriptableObjects/Level/KillAllLevel")]
public class Level_Kill_All : Level
{
    public void OnEnable()
    {
        levelType = LevelType.KillAll;
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
