using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class CapturePoint : Objective
{
    /**private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one * influenceRange);
    }

    public void OnSceneGUI()
    {
        List<Tile> tiles = new List<Tile>();

        foreach(GridTile gt in objectivePositions)
        {
            tiles.Add((Tile)MapManager.instance.floorTilemaps[gt.gridPosition.z].GetTile(gt.gridPosition));
        }
        
        //Handles.DrawOutline(tiles, Color.red, 0);
        Handles.DrawLine(transform.position, transform.position * 2);
    }*/

    // Start is called before the first frame update
    void Start()
    {

    }
}
