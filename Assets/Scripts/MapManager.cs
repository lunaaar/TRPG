using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    public Dictionary<Vector3Int, GridTile> map;

    private Obstacle[] obstacles;

    public GameObject[] objectives;
    public CapturePoint[] capturePoints;

    //public PathFinder pathFinder;

    /*
     * Test Coloring, should make this option permanent but only in a "debug mode"
     * - The following will be the rubric for what the colors associatedly mean.
     * 
     * Rubric:
     * 
     * - White     = base floor
     * 
     * - Green     = notOccupied
     * - Red       = occupied
     * - Dark Gray = obstacle
     * - Pink      = objective space
     * 
     * - Brown     = difficult terrain (not occupied but moveMod != 1)
     * 
     * Unused Colors:
     * - Orange
     * - Blue
     * - Bright Pink
     * - Purple
     * 
     */
    public Tilemap tilemap;
    [SerializeField] private Tile notOccupiedTile;
    [SerializeField] private Tile occupiedTile;
    [SerializeField] private Tile obstacleTile;
    [SerializeField] private Tile objectiveTile;

    public Vector3Int[] testTerrainTiles;
    public Tile testTerrainTile;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (instance == null) instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //pathFinder = new PathFinder();
        capturePoints = (CapturePoint[])FindObjectsOfType(typeof(CapturePoint));
        obstacles = (Obstacle[])FindObjectsOfType(typeof(Obstacle));

        map = new Dictionary<Vector3Int, GridTile>();

        setUpMap();
    }

    public void setUpMap()
    {
        //Sets up base map.

        BoundsInt bounds = tilemap.cellBounds;

        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                var location = new Vector3Int(x, y, 0);

                if (tilemap.HasTile(location))
                {
                    GridTile gridTile = new GridTile(location);
                    map.Add(location, gridTile);
                    updateOccupiedStatus(location, "NotOccupied");
                }
            }
        }
        
        //Sets all the objectives on the tilemap
        foreach (CapturePoint cp in capturePoints)
        {
            cp.updateTilemap(map, tilemap);
            updateOccupiedStatus(cp.gridPos, "Terrain");
        }

        //Sets all tiles characters are on to occupied.
        foreach (Character character in GameManager.instance.listOfAllCharacters)
        {
            updateOccupiedStatus(character.gridPos, "Occupied");
        }

        //Sets all tiles with any obstacles on to occupied.
        foreach (Obstacle obstacle in obstacles)
        {
            updateOccupiedStatus(obstacle.gridPos, "Terrain");
            foreach (Vector3Int pos in obstacle.usedSpaces)
            {
                updateOccupiedStatus(pos + obstacle.gridPos, "Terrain");
            }
        }

        //TEST STUFF
        foreach(Vector3Int v in testTerrainTiles)
        {
            tilemap.SetTile(v, testTerrainTile);
            map[v].movementPenalty = 2;
        }
    }

    public void updateOccupiedStatus(Vector3Int location, string status)
    {
        switch (status) {  
            case "Objective":
                tilemap.SetTile(location, objectiveTile);
                map[location].status = "Objective";
                break;
            case "Occupied":
                tilemap.SetTile(location, occupiedTile);
                map[location].status = "Occupied";
                break;
            case "Terrain":
                tilemap.SetTile(location, obstacleTile);
                map[location].status = "Occupied";
                break;
            case "NotOccupied":
                tilemap.SetTile(location, notOccupiedTile);
                map[location].status = "NotOccupied";
                break;
            default:
                break;
        }

        map[location].movementPenalty = 1;
    }
}
