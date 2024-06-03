using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Level : ScriptableObject
{
    [Tooltip("What is the name of the level?")] public string levelName;
    [Tooltip("What is the scene name associated with the level")] public string sceneName;
    public enum LevelType { KillAll, Protect, CapturePoint, Payload}
    public LevelType levelType;

    public int turnCount;
    public int maxTurns;

    public abstract void processEndOfTurn();

    public abstract bool checkWinCondition();
}
