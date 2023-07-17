using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance;

    public static MapManager Instance { get { return _instance; } }

    public Dictionary<Vector3Int, GridTile> map;

    public Character[] listOfCharacters;

    private Obstacle[] obstacles;

    public GameObject[] objectives;
    public CapturePoint[] capturePoints;

    public PathFinder pathFinder;

    //TEST; To be removed when colored display is not needed anymore.
    public Tilemap tilemap;
    public Tile notOccupiedTile;
    public Tile occupiedTile;
    public Tile obstacleTile;
    public Tile objectiveTile;

    public Vector3Int[] testTerrainTiles;
    public Tile testTerrainTile;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        pathFinder = new PathFinder();

        listOfCharacters = (Character[])FindObjectsOfType(typeof(Character));
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
        foreach (Character character in listOfCharacters)
        {
            updateOccupiedStatus(character.gridPos, "Occupied");
        }

        //Sets all tiles with any obstacles on to occupied.
        foreach (Obstacle obstacle in obstacles)
        {
            for(int i = 0; i < obstacle.size.x; i++)
            {
                for(int j = 0; j < obstacle.size.y; j++)
                {
                    updateOccupiedStatus(new Vector3Int(i + obstacle.gridPos.x, j + obstacle.gridPos.y), "Terrain");
                }
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
