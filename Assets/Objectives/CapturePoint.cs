using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class CapturePoint : Objective
{
    [Header("====== Capture Point Info ======")]
    [Range(1, 6)] [Tooltip("How big is the cube radius of the zone")] public int influenceRange = 3;

    private void OnDrawGizmos()
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
    }

    // Start is called before the first frame update
    void Start()
    {

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

    public void resetTilemap()
    {
        foreach (GridTile tile in objectivePositions)
        {
            var z = tile.gridPosition.z;

            MapManager.instance.map[tile.gridPosition].status = "NotOccupied";
            //MapManager.instance.floorTilemaps[tilePosition.z].SetTileFlags(tilePosition, TileFlags.None);
            //Debug.Log(MapManager.instance.floorTilemaps[tilePosition.z].GetColor(tilePosition));
            MapManager.instance.floorTilemaps[z].SetColor(tile.gridPosition, Color.white);
            //Debug.Log(MapManager.instance.floorTilemaps[tilePosition.z].GetColor(tilePosition));
        }
    }
}
