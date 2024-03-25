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
    public List<Vector3Int> usedSpaces;
    
    [Header("====== Grid Info ======")]
    [Tooltip("Position of the player on the grid")] public Vector3Int gridPosition;
    [Tooltip("Reference to the grid")] public GameObject grid;
    private Tilemap t;

    private void Awake()
    {
        t = grid.GetComponentInChildren<Tilemap>();

        //Alligns internal grid position with where it actually is;
        gridPosition = t.WorldToCell(this.transform.position);
        this.transform.position = t.GetCellCenterWorld(gridPosition);
    }
    public void updateGridPos(Vector3Int v)
    {
        gridPosition = v;
        this.transform.position = t.GetCellCenterWorld(gridPosition);
    }

    public void updateGridPos()
    {
        gridPosition = grid.GetComponentInChildren<Tilemap>().WorldToCell(this.transform.position);
        this.transform.position = t.GetCellCenterWorld(gridPosition);
    }
}
