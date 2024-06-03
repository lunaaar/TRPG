using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Payload : Objective
{
    //. Funnily enough. a Payload is just a capture point that moves. So this is going to be very similar.
    
    [Header("====== Payload ======")]
    [Range(1, 6)] public int movementSpeed;

    [Space(5)]
    public List<Vector3> displayPayloadPath;
    public List<Vector3Int> payloadPath; //? List of Grid positions

    public Vector3Int next;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void setUpPath()
    {
        payloadPath.Add(gridPosition);
        
        foreach (Vector3 v in displayPayloadPath)
        {
            payloadPath.Add(MapManager.instance.floorTilemaps[(int)v.z - 1].WorldToCell(v));
        }

        next = payloadPath[1];
    }

    public void moveAlongPath()
    {
        var step = movementSpeed * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, MapManager.instance.floorTilemaps[next.z - 1].GetCellCenterWorld(next), step);

        if (Vector2.Distance(transform.position, MapManager.instance.floorTilemaps[next.z - 1].GetCellCenterWorld(next)) < 0.0001f)
        {
            //MapManager.instance.updateTileStatus(gridPosition, "NotOccupied");

            resetTilemap();

            updateGridPos(next);

            updateTilemap();
            return;
        }
    }
}
