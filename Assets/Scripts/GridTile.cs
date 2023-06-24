using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridTile
{

    [Header("====== A* Values ======")]
    public int G, H;
    public int F { get { return G + H;} }

    

    [Header("===== Flood Value =====")]
    public int value;

    //Set to true if anything is occupying the tile.
    public string status;

    //Previous GridTile in a path.
    public GridTile previous;

    //Current position on the tilemap of the GridTile.
    public Vector3Int gridPosition;


    public int movementPenalty;

    public GridTile(Vector3Int gP)
    {
        gridPosition = gP;
    }
    
    public void setBlockedStatus(string s)
    {
        this.status = s;
    }

    public override string ToString()
    {
        return "Tile @: " + gridPosition + "\n" + "F=" + F + "\n" + "G=" + G + "\n" + "H=" + H;
    }
}
