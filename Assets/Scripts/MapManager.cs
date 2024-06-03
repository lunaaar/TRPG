using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    public Dictionary<Vector3Int, GridTile> map;

    private Obstacle[] obstacles;

    //public CapturePoint[] capturePoints;

    //public PathFinder pathFinder;

    /*?
     * Test Coloring: (only active when debugMode is flipped to true).
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

    [Header("Test Tilemap Idea")]
    public Tilemap[] floorTilemaps;
    public Tilemap[] movementTilemaps;
    public Tilemap[] attackTilemaps;



    [Header("DEBUG COLORS")]
    [SerializeField] private bool debugMode;

    [SerializeField] private Tile notOccupiedTile;
    [SerializeField] private Tile occupiedTile;
    [SerializeField] private Tile obstacleTile;
    [SerializeField] private Tile objectiveTile;
    [SerializeField] private Tile objectiveBaseTile;
    [SerializeField] private Tile waterTile;

    public Vector3Int[] testDifficultTerrainList;
    public Tile testDifficultTerrain;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (instance == null) instance = this;

        obstacles = (Obstacle[])FindObjectsOfType(typeof(Obstacle));

        map = new Dictionary<Vector3Int, GridTile>();

        
    }

    // Start is called before the first frame update
    void Start()
    {
        //pathFinder = new PathFinder();
        //capturePoints = (CapturePoint[])FindObjectsOfType(typeof(CapturePoint));
        //setUpMap();

        /**foreach(Tilemap t in floorTilemaps)
        {
            setUpMap(t);
        }*/

        floorTilemaps = GameObject.Find("Floor Parent").GetComponentsInChildren<Tilemap>();

        setUpMaps();
    }

    public void setUpMaps()
    {
        Vector3Int location;

        Tilemap tilemap;

        //. Process Entire Map first.
        for (int i = 0; i < floorTilemaps.Length; i++)
        {   
            BoundsInt bounds = floorTilemaps[i].cellBounds;

            for(int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for(int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    location = new Vector3Int(x, y, i);

                    if(i == 0 && floorTilemaps[i].HasTile(location))
                    {
                        GridTile gridTile = new GridTile(location);
                        map.Add(location, gridTile);
                        //updateTileStatus(location, "Water", floorTilemaps[i]);
                        updateTileStatus(location, "Water");
                    }
                    else if (floorTilemaps[i].HasTile(location))
                    {
                        GridTile gridTile = new GridTile(location);
                        map.Add(location, gridTile);
                        //floorTilemaps[i].SetTileFlags(location, TileFlags.None);
                        //updateTileStatus(location, "NotOccupied", floorTilemaps[i]);
                        updateTileStatus(location, "NotOccupied");
                    }
                }
            }
        }

        //Sets all the objectives on the tilemap

        switch (GameManager.instance.currentLevel.levelType) 
        {
            case (Level.LevelType.CapturePoint):
                var levelCP = (Level_Capture_Point)GameManager.instance.currentLevel;

                levelCP.getAllCapturePoints();

                foreach (CapturePoint capturePoint in levelCP.capturePoints)
                {
                    capturePoint.updateGridPos();

                    capturePoint.updateTilemap();

                    capturePoint.calculateStatus();
                }
                break;
            case (Level.LevelType.Payload):
                var levelPay = (Level_Payload)GameManager.instance.currentLevel;

                levelPay.getAllPayloads();

                //. Possibly expansion for a FOR loop here if we ever want to handle multiple payloads.

                levelPay.payload.updateGridPos();

                levelPay.payload.updateTilemap();

                levelPay.payload.setUpPath();

                //tilemap = floorTilemaps[levelPay.payload.GetComponent<SpriteRenderer>().sortingOrder - 1];
                //updateTileStatus(levelPay.payload.gridPosition, "ObjectiveBase");

                levelPay.payload.calculateStatus();

                break;
            case (Level.LevelType.KillAll):
                break;
            case (Level.LevelType.Protect):
                break;
            default:
                break;
        }

        //. Process every character.
        foreach (Character character in GameManager.instance.listOfAllCharacters)
        {
            character.updateGridPos();
            tilemap = floorTilemaps[character.GetComponent<SpriteRenderer>().sortingOrder - 1];
            //updateTileStatus(character.gridPosition, character.alignment.ToString(), tilemap);
            updateTileStatus(character.gridPosition, character.alignment.ToString());
        }

        //. Sets all tiles with any obstacles on to occupied.
        foreach (Obstacle obstacle in obstacles)
        {
            obstacle.updateGridPos();

            updateTileStatus(obstacle.gridPosition, "Obstacle");
            foreach (Vector3Int pos in obstacle.occupiedSpaces)
            {
                updateTileStatus(pos + obstacle.gridPosition, "Obstacle");
            }
        }
    }

    public void setUpMap()
    {
        /**
        //Sets up base map.
        BoundsInt bounds = tilemap.cellBounds;
        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                for(int z = 0; z < bounds.max.z; z++)
                {
                    var location = new Vector3Int(x, y, z);

                    if(z == 0 && tilemap.HasTile(location))
                    {
                        GridTile gridTile = new GridTile(location);
                        map.Add(location, gridTile);
                        updateTileStatus(location, "Water");
                    }
                    else if (tilemap.HasTile(location))
                    {
                        GridTile gridTile = new GridTile(location);
                        map.Add(location, gridTile);
                        tilemap.SetTileFlags(location, TileFlags.None);
                        updateTileStatus(location, "NotOccupied");
                    }
                }
            }
        }

        /**
        //Sets all the objectives on the tilemap
        if(GameManager.instance.currentLevel.levelType == Level.LevelType.CapturePoint)
        {
            var level = (Level_Capture_Point)GameManager.instance.currentLevel;

            level.getAllCapturePoints();

            foreach (CapturePoint cp in level.capturePoints)
            {
                cp.updateTilemap(map, tilemap, objectiveTile);
                updateTileStatus(cp.gridPos, "ObjectiveBase");
                cp.calculateStatus();
            }
        }


        
        //Sets all tiles characters are on to occupied.
        foreach (Character friendly in GameManager.instance.listOfAllFriendly)
        {
            //Debug.Log(friendly.gridPosition);
            updateTileStatus(friendly.gridPosition, "Friendly");
        }
        /**
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
        }*/
    }

    public void updateTileStatus(Vector3Int location, string status)
    {
        map[location].status = status;
        map[location].movementPenalty = 1;

        if (debugMode)
        {
            floorTilemaps[location.z].SetColor(location, Color.white);
            
            switch (status)
            {
                case "NotOccupied":
                    floorTilemaps[location.z].SetTile(location, notOccupiedTile);
                    break;
                case "Friendly":
                    floorTilemaps[location.z].SetTile(location, occupiedTile);
                    break;
                case "Enemy":
                    floorTilemaps[location.z].SetTile(location, occupiedTile);
                    break;
                case "Objective":
                    floorTilemaps[location.z].SetTile(location, objectiveTile);
                    break;
                case "ObjectiveBase":
                    floorTilemaps[location.z].SetTile(location, objectiveBaseTile);
                    break;
                case "Obstacle":
                    floorTilemaps[location.z].SetTile(location, obstacleTile);
                    break;
                case "Water":
                    floorTilemaps[location.z].SetTile(location, waterTile);
                    break;
                default:
                    break;
            }
        }
    }

    public void updateTileStatus(Vector3Int location, string status, Tilemap tilemap)
    {
        switch (status) {
            case "NotOccupied":
                if (debugMode) tilemap.SetTile(location, notOccupiedTile);
                map[location].status = "NotOccupied";
                break;
            case "Friendly":
                if (debugMode) tilemap.SetTile(location, occupiedTile);
                map[location].status = "Friendly";
                break;
            case "Enemy":
                if (debugMode) tilemap.SetTile(location, occupiedTile);
                map[location].status = "Enemy";
                break;
            case "Objective":
                if(debugMode) tilemap.SetTile(location, objectiveTile);
                map[location].status = "Objective";
                break;
            case "Obstacle":
                if (debugMode) tilemap.SetTile(location, obstacleTile);
                map[location].status = "Obstacle";
                break;
            case "ObjectiveBase":
                if (debugMode) tilemap.SetTile(location, objectiveBaseTile);
                //? This might come back to bite me later.
                //. NO SHIT SHERLOCK
                map[location].status = "ObjectiveBase";
                break;
            case "Water":
                if (debugMode) tilemap.SetTile(location, waterTile);
                map[location].status = "Water";
                break;
            default:
                break;
        }

        map[location].movementPenalty = 1;
    }

    public void resetMap()
    {
        foreach(KeyValuePair<Vector3Int, GridTile> entry in map)
        {
            floorTilemaps[entry.Value.gridPosition.z].SetColor(entry.Value.gridPosition, Color.white);
        }
    }

    public void resetMovementTiles()
    {
        if(CursorMovement.instance.selectedCharacter == null || CursorMovement.instance.selectedCharacter.movementTiles == null)
        {
            return;
        }
        
        foreach(GridTile t in CursorMovement.instance.selectedCharacter.movementTiles)
        {
            floorTilemaps[t.gridPosition.z].SetColor(t.gridPosition, Color.white);
        }
    }

    public void resetAttackTiles()
    {
        if(CursorMovement.instance.selectedCharacter.selectedAction == null || CursorMovement.instance.selectedCharacter.attackTiles == null)
        {
            return;
        }

        foreach (GridTile t in CursorMovement.instance.selectedCharacter.attackTiles)
        {
            floorTilemaps[t.gridPosition.z].SetColor(t.gridPosition, Color.white);
        }
    }

    public Character getCharacterAt(GridTile tile)
    {
        foreach(Character c in GameManager.instance.listOfAllCharacters)
        {
            if(c.gridPosition == tile.gridPosition)
            {
                return c;
            }
        }

        return null;
    }
}
