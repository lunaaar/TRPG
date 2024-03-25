using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{
    public enum Status { Enemy, Friendly, Neutral}
    [Tooltip("Who currently is in control of the capture point")] public Status status;
   
    public List<GridTile> objectivePositions;

    [Header("====== Grid Info ======")]
    [Tooltip("Position of the Capture Point Center on the grid")] public Vector3Int gridPosition;
    [Tooltip("Reference to the grid")] public GameObject grid;

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
