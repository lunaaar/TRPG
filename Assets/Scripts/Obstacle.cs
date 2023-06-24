using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Obstacle : MonoBehaviour
{
    //Note to self:
    //the game thinks of the size as originating from the bottom right, the tile most towards the camera.
    public Vector2Int size;
    
    [Header("====== Grid Info ======")]
    [Tooltip("Position of the player on the grid")] public Vector3Int gridPos;
    [Tooltip("Reference to the grid")] public GameObject grid;
    private Tilemap t;

    private void Awake()
    {
        t = grid.GetComponentInChildren<Tilemap>();

        //Alligns internal grid position with where it actually is;
        gridPos = t.WorldToCell(this.transform.position);
        this.transform.position = t.GetCellCenterWorld(gridPos);
    }
    public void updateGridPos(Vector3Int v)
    {
        gridPos = v;
        this.transform.position = t.GetCellCenterWorld(gridPos);
    }

    public void updateGridPos()
    {
        gridPos = grid.GetComponentInChildren<Tilemap>().WorldToCell(this.transform.position);
        this.transform.position = t.GetCellCenterWorld(gridPos);
    }
}
