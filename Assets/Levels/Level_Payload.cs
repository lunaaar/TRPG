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

        //var count = 0;

        payload.calculateStatus();

        Debug.Log("Payload: " + payload.name +"\n"+
            "Status: " + payload.status) ;

        if(payload.status == Objective.Status.Friendly)
        {
            while (payload.gridPosition != (payload.next + Vector3Int.back))// && count < 20)
            {
                payload.moveAlongPath();
                //count++;
            }

            payload.next = payload.payloadPath[Mathf.Min(payload.payloadPath.IndexOf(payload.next) + 1, payload.payloadPath.Count - 1)];
        }
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
