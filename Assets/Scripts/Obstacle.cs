using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Obstacle : MonoBehaviour
{
    /**
     * This list is used as a vector based list to represent what spaces the Obstacle
     * takes up on the map originated from 0,0 being the center point of the object.
     * 
     * Note for ease of use:
     * <1,0>  is for up, right
     * <0,1>  is for up, left
     * <-1,0> is for down left
     * <0,-1> is for down right
     */
    public List<Vector3Int> occupiedSpaces;
    
    [Header("====== Grid Info ======")]
    [Tooltip("Position of the player on the grid")] public Vector3Int gridPosition;

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
}
