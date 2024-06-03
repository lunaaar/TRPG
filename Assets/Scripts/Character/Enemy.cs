using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class Enemy : Character
{
    public class EnemyAction
    {
        public Action action;
        public GridTile tileToMoveTo;
        public Character target;
        public int score;

        public EnemyAction(Action a, GridTile mt, Character t, int s)
        {
            action = a; tileToMoveTo = mt; target = t; score = s;
        }

        public string ToString()
        {
            return "\n" + "Action: " + action.name + "\n"
                +  "Move to tile " + tileToMoveTo.gridPosition + "\n"
                + "Target: " + target.name + "\n"
                + "Score: " + score;
        }
    }


    public void Update()
    {
        
    }

    public Enemy()
    {
        alignment = AlignmentStatus.Enemy;
    }
    
    public EnemyAction calculateBestMove()
    {
        //? Write notes here on how it will work.

        List<EnemyAction> testList = new List<EnemyAction>();

        List<Action> actionList = new List<Action>();

        actionList.AddRange(listOfWeapons);
        actionList.AddRange(listOfSpells);
        actionList.AddRange(listOfAbilities);

        var movementTiles = getTilesInRange(movementRange);

        //List<GridTile> actionRangeList = a.showActionRange(movementTiles, MapManager.instance.map[gridPosition], movementRange, alignment.ToString(), true);

        //Debug.Log(actionRangeList.Count);

        foreach (GridTile tile in movementTiles)
        {
            var path = GameManager.instance.pathFinder.findPath(MapManager.instance.map[gridPosition], tile);

            int score = path.Sum(t => t.movementPenalty);

            foreach (Action a in actionList)
            {
                if(a.uses == 0)
                {
                    continue;
                }
                
                List<GridTile> actionRangeList = a.showActionRange(movementTiles, tile, 0, alignment.ToString(), true);

                if (a.actionTargets == Action.ActionTargets.AOE)
                {
                    //? Extra bullshit because of AOE.
                    foreach (GridTile tile2 in actionRangeList)
                    {
                        score += a.performAction(this, this, true);
                    }
                }
                else
                {
                    foreach (GridTile tile2 in actionRangeList)
                    {
                        Character characterAt = GameManager.instance.getCharacterAt(tile2.gridPosition);
                        if (characterAt != null && characterAt != this && GameManager.instance.pathFinder.findPath(tile, tile2).Count == a.range)
                        {
                            var test = score + a.performAction(this, GameManager.instance.getCharacterAt(tile2.gridPosition), true);

                            testList.Add(new EnemyAction(a, tile,
                                GameManager.instance.getCharacterAt(tile2.gridPosition),
                                score + test));
                        }
                        
                        //score += a.performAction(this, GameManager.instance.getCharacterAt(tile2.gridPosition), true);

                    }
                }

            }
        }

        List<EnemyAction> sortedList = testList.OrderByDescending(o => o.score).ToList();

        return sortedList[0];
    }
}
