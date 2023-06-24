using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CapturePoint : MonoBehaviour
{
    [Header("====== Capture Point Info ======")]
    [Range(1, 6)] [Tooltip("How big is the circular radius of the zone")] public int influenceRange = 3;

    [Space(10)]

    [Header("====== Grid Info ======")]
    [Tooltip("Position of the Capture Point Center on the grid")] public Vector3Int gridPos;
    [Tooltip("Reference to the grid")] public GameObject grid;

    private Tilemap t;

    public Tile capturePointTile;
    public int calcRange;

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
        calcRange = Mathf.FloorToInt(Mathf.Floor(influenceRange / 2));

        getTilePositionsInRange();
    }

    private void getTilePositionsInRange()
    {
        if (influenceRange % 2 == 0)
        {
            //even condition
        }
        else
        {
            //odd condition
            for (int x = -calcRange; x <= calcRange; x++)
            {
                for (int y = -calcRange; y <= calcRange; y++)
                {
                    Vector3Int gridLocation = new Vector3Int(gridPos.x + x, gridPos.y + y);
                    capturePointTilemapPositions.Add(gridLocation);
                }
            }
        }
    }


    public void updateTilemap(Dictionary<Vector3Int, GridTile> map, Tilemap tilemap)
    {   
        if (influenceRange % 2 == 0)
        {
            //even condition
        }
        else
        {
            //odd condition
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
