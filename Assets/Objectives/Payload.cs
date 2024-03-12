using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Payload : Objective
{
    //. Funnily enough. a Payload is just a capture point that moves. So this is going to be very similar.
    
    [Header("====== Payload ======")]
    [Range(1, 6)] [Tooltip("How big is the cube radius of the zone")] public int influenceRange = 3;

    public List<Vector3> displayPayloadPath;
    public List<Vector3Int> payloadPath;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void setUpPath()
    {
        foreach (Vector3 v in displayPayloadPath)
        {
            payloadPath.Add(MapManager.instance.floorTilemaps[(int)v.z - 1].WorldToCell(v));
        }
    }

    public void updateTilemap()
    {
        PathFinder pathFinder = new PathFinder();

        objectivePositions = pathFinder.getCube(MapManager.instance.map[gridPosition], influenceRange);

        foreach (GridTile tile in objectivePositions)
        {
            var z = tile.gridPosition.z;

            MapManager.instance.map[tile.gridPosition].status = "Objective";
            //MapManager.instance.floorTilemaps[tilePosition.z].SetTileFlags(tilePosition, TileFlags.None);
            //Debug.Log(MapManager.instance.floorTilemaps[tilePosition.z].GetColor(tilePosition));
            MapManager.instance.floorTilemaps[z].SetColor(tile.gridPosition, GameManager.instance.testColor);
            //Debug.Log(MapManager.instance.floorTilemaps[tilePosition.z].GetColor(tilePosition));
        }
    }
}
