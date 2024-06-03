using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{
    public enum Status { Enemy, Friendly, Neutral}
    [Tooltip("Who currently is in control of the capture point")] public Status status;
   

    //. These likely need to renanme.
    public List<GridTile> objectivePositions;
    public List<GridTile> basePositions;

    [Range(0, 4)] [Tooltip("What is the range of the objectiveBase?")] public int baseRange = 1;
    [Range(1, 6)] [Tooltip("How big is the cube radius of the zone")] public int influenceRange = 3;

    [Header("====== Grid Info ======")]
    [Tooltip("Position of the Capture Point Center on the grid")] public Vector3Int gridPosition;

    public void updateGridPos()
    {
        var sortOrder = GetComponent<SpriteRenderer>().sortingOrder;

        var tilemap = MapManager.instance.floorTilemaps[sortOrder - 1];

        gridPosition = tilemap.WorldToCell(transform.position);

        transform.position = tilemap.GetCellCenterWorld(gridPosition);

        gridPosition.z -= 1;
    }

    public void updateGridPos(Vector3Int gridPos)
    {
        var tilemap = MapManager.instance.floorTilemaps[gridPos.z];

        gridPosition = gridPos;

        transform.position = tilemap.GetCellCenterWorld(gridPos);

        GetComponent<SpriteRenderer>().sortingOrder = gridPos.z;

        gridPosition.z -= 1;
    }

    public void updateTilemap()
    {
        objectivePositions = GameManager.instance.pathFinder.getCube(MapManager.instance.map[gridPosition], baseRange, influenceRange);

        foreach (GridTile tile in objectivePositions)
        {
            if (MapManager.instance.map[tile.gridPosition].status == "NotOccupied")
            {
                MapManager.instance.updateTileStatus(tile.gridPosition, "Objective");
            }
        }

        basePositions = GameManager.instance.pathFinder.getCube(MapManager.instance.map[gridPosition], baseRange);

        foreach (GridTile tile in basePositions)
        {
            MapManager.instance.updateTileStatus(tile.gridPosition, "ObjectiveBase");
        }

        //MapManager.instance.updateTileStatus(gridPosition, "ObjectiveBase");
    }
    public void resetTilemap()
    {
        //MapManager.instance.updateTileStatus(gridPosition, "NotOccupied");
        foreach (GridTile tile in basePositions)
        {
            if (tile.status == "ObjectiveBase")
            {
                MapManager.instance.updateTileStatus(tile.gridPosition, "NotOccupied");
            }
        }

        foreach (GridTile tile in objectivePositions)
        {
            if (tile.status == "Objective")
            {
                MapManager.instance.updateTileStatus(tile.gridPosition, "NotOccupied");
            }
        }


    }


    public void calculateStatus()
    {
        int friendlyCount = 0;
        int enemyCount = 0;

        foreach (GridTile gridTile in objectivePositions)
        {
            switch (MapManager.instance.map[gridTile.gridPosition].status)
            {
                case ("Friendly"):
                    friendlyCount++;
                    break;
                case ("Enemy"):
                    enemyCount++;
                    break;
            }
        }

        if (friendlyCount == enemyCount)
        {
            status = Status.Neutral;
        }
        else
        {
            status = (friendlyCount > enemyCount) ? Status.Friendly : Status.Enemy;
        }
    }
}
