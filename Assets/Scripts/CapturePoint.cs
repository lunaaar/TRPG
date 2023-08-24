using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CapturePoint : MonoBehaviour
{
    public enum Status { Enemy, Friendly, Neutral}


    [Header("====== Capture Point Info ======")]
    [Range(1, 6)] [Tooltip("How big is the circular radius of the zone")] public int influenceRange = 3;
    [Tooltip("Who currently is in control of the capture point")] public Status status;

    [Space(10)]

    [Header("====== Grid Info ======")]
    [Tooltip("Position of the Capture Point Center on the grid")] public Vector3Int gridPos;
    [Tooltip("Reference to the grid")] public GameObject grid;

    private Tilemap t;

    public Tile capturePointTile;
    private int calcRange;

    public List<Vector3Int> capturePointTilemapPositions;

    private void Awake()
    {
        t = grid.GetComponentInChildren<Tilemap>();

        //Alligns internal grid position with where it actually is;
        gridPos = t.WorldToCell(this.transform.position);
        this.transform.position = t.GetCellCenterWorld(gridPos);

        //TODO: this will likely need to be updated to take into account different size center objects.
    }

    // Start is called before the first frame update
    void Start()
    {
        if(influenceRange % 2 != 0) calcRange = Mathf.FloorToInt(Mathf.Floor(influenceRange / 2));
        getTilePositionsInRange();
    }

    private void getTilePositionsInRange()
    {
        //Could potentially move this system over to the obstacle system of having
        //it be a list of conditions
        
        if (influenceRange % 2 == 0)
        {
            /*
             * Even Condition:
             * Treats gridPos as the bottom right most position
             */
            for(int x = 0; x < influenceRange; x++)
            {
                for(int y = 0; y < influenceRange; y++)
                {
                    capturePointTilemapPositions.Add(new Vector3Int(gridPos.x + x, gridPos.y + y));
                }
            }
        }
        else
        {
            /*
             * Odd Condition:
             * Treats gridPos as the center position
             */
            for (int x = -calcRange; x <= calcRange; x++)
            {
                for (int y = -calcRange; y <= calcRange; y++)
                {
                    capturePointTilemapPositions.Add(new Vector3Int(gridPos.x + x, gridPos.y + y));
                }
            }
        }
    }

    public void updateTilemap(Dictionary<Vector3Int, GridTile> map, Tilemap tilemap)
    {   
        if (influenceRange % 2 == 0)
        {
            /*
             * Even Condition:
             * Treats gridPos as the bottom right most position
             */
            for (int x = 0; x < influenceRange; x++)
            {
                for (int y = 0; y < influenceRange; y++)
                {
                    Vector3Int gridLocation = new Vector3Int(gridPos.x + x, gridPos.y + y);
                    tilemap.SetTile(gridLocation, capturePointTile);
                    map[gridLocation].status = "Objective";
                }
            }
        }
        else
        {
            /*
             * Odd Condition:
             * Treats gridPos as the center position
             */
            for (int x = -calcRange; x <= calcRange; x++)
            {
                for (int y = -calcRange; y <= calcRange; y++)
                {
                    Vector3Int gridLocation = new Vector3Int(gridPos.x + x, gridPos.y + y);
                    tilemap.SetTile(gridLocation, capturePointTile);
                    map[gridLocation].status = "Objective";
                }
            }
        }
    }
}
