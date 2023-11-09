using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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

    [Header("DEBUG COLORS")]
    [SerializeField] private bool debugMode;

    [SerializeField] private Tile notOccupiedTile;
    [SerializeField] private Tile occupiedTile;
    [SerializeField] private Tile obstacleTile;
    [SerializeField] private Tile objectiveTile;
    [SerializeField] private Tile objectiveBaseTile;

    public Vector3Int[] testDifficultTerrainList;
    public Tile testDifficultTerrain;

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
                    updateTileStatus(location, "NotOccupied");
                }
            }
        }
        
        
        //Sets all the objectives on the tilemap
        foreach (CapturePoint cp in capturePoints)
        {
            cp.updateTilemap(map, tilemap, objectiveTile);
            updateTileStatus(cp.gridPos, "ObjectiveBase");
        }

        //Sets all tiles characters are on to occupied.
        foreach (Character friendly in GameManager.instance.listOfAllFriendly)
        {
            updateTileStatus(friendly.gridPosition, "Friendly");
        }

        foreach(Character enemy in GameManager.instance.listOfAllEnemies)
        {
            updateTileStatus(enemy.gridPosition, "Enemy");
        }

        //Sets all tiles with any obstacles on to occupied.
        foreach (Obstacle obstacle in obstacles)
        {
            updateTileStatus(obstacle.gridPos, "Obstacle");
            foreach (Vector3Int pos in obstacle.usedSpaces)
            {
                updateTileStatus(pos + obstacle.gridPos, "Obstacle");
            }
        }

        //Difficult Terrain Test
        foreach(Vector3Int v in testDifficultTerrainList)
        {
            tilemap.SetTile(v, testDifficultTerrain);
            map[v].movementPenalty = 2;
        }
    }

    public void updateTileStatus(Vector3Int location, string status)
    {
        switch (status) {  
            case "Objective":
                if(debugMode) tilemap.SetTile(location, objectiveTile);
                map[location].status = "Objective";
                break;
            case "Friendly":
                if (debugMode) tilemap.SetTile(location, occupiedTile);
                map[location].status = "Friendly";
                break;
            case "Enemy":
                if (debugMode) tilemap.SetTile(location, occupiedTile);
                map[location].status = "Enemy";
                break;
            case "Obstacle":
                if (debugMode) tilemap.SetTile(location, obstacleTile);
                map[location].status = "Obstacle";
                break;
            case "NotOccupied":
                if (debugMode) tilemap.SetTile(location, notOccupiedTile);
                map[location].status = "NotOccupied";
                break;
            case "ObjectiveBase":
                if (debugMode) tilemap.SetTile(location, objectiveBaseTile);
                //This might come back to bite me later.
                map[location].status = "Obstacle";
                break;
            default:
                break;
        }

        map[location].movementPenalty = 1;
    }
}
