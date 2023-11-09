using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new_capture_point_level", menuName = "Levels/CapturePointLevel")]
public class Level_Capture_Point : Level
{
    public void OnEnable()
    {
        levelType = LevelType.CapturePoint;
    }

    public override bool checkWinCondition()
    {
        /*
         * Win Condition:
         * --------------
         * Returns true if its the last turn and all your units are in the capture point
         * 
         * 
         * Thinking of changing this though. Not sure how fun this would feel. Typically games do a point system for a team, and your team wins by having more points
         * 
         * I think the smarter way to do this, is have capture points overwatch style, where at the end of every turn, capture point checks if it should switch ownership.
         * You win if its the last turn and if your capture point is in your control, you win.
         */

        if (turnCount != maxTurns) return false;

        foreach(CapturePoint capturePoint in MapManager.instance.capturePoints)
        {
            foreach(Character character in GameManager.instance.listOfAllFriendly)
            {
                if (!capturePoint.capturePointTilemapPositions.Contains(character.gridPosition))
                {
                    return false;
                }
            }
        }


        return true;
    }
}