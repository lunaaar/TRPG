using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Level : ScriptableObject
{
    public string name;
    public enum LevelType { KillAll, Protect, CapturePoint, Payload}
    public new LevelType levelType;

    public int turnCount;
    public int maxTurns;

    public abstract bool checkWinCondition();
}
