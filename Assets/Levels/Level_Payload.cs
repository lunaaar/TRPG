using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new_payload_level", menuName = "Levels/PayloadLevel")]
public class Level_Payload : Level
{
    public Payload payload;
    
    public Level_Payload()
    {
        levelType = LevelType.Payload;
    }

    public void getAllPayloads()
    {
        payload = (Payload)FindObjectOfType(typeof(Payload));
    }

    public override void processEndOfTurn()
    {
        //. Check status, if friendly push 1 more forward.
        
        throw new System.NotImplementedException();
    }

    public override bool checkWinCondition()
    {
        if(payload.gridPosition == payload.payloadPath[payload.payloadPath.Count])
        {
            return true;
        }

        return false;
    }
}
