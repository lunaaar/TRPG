using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile
{

    [Header("====== A* Values ======")]
    public int G, H;
    public int F { get { return G + H; } }

    //Set to true if anything is occupying the tile.
    public bool isOccupied;

    //Previous GridTile in a path.
    public GridTile previous;

    //Current position on the tilemap of the GridTile.
    public Vector3Int gridPosition;
    
    public GridTile(Vector3Int gP)
    {
        gridPosition = gP;
    }
    
    public void setBlockedStatus(bool status)
    {
        isOccupied = status;
    }
}
