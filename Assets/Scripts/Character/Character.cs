using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class Character : MonoBehaviour
{
    [Header("====== Character Stats ======")]
    [Tooltip("Character Name")] public string characterName;
    [Range(1, 6)] [Tooltip("How many tiles can this character move?")] public int movementRange = 3;

    public Stats characterStats;

    public List<Weapon> weapons;
    [Space(2)]
    public List<Ability> abilities;
    [Space(2)]
    public List<Spell> spells;

    [Space(2)]
    [Tooltip("Is the Character actively selected?")] public bool isSelected;

    [Space(5)]
    [Header("====== Character Info ======")]
    public List<GridTile> movementTiles;
    public List<GridTile> attackTiles;
    public enum AlignmentStatus { Friendly, Neutral, Enemy}
    [Tooltip("Alignment Status of the Character, determines how the manager treats this character.")] public AlignmentStatus alignment;
    private Slider slider;
    
    [Space(10)]

    [Header("====== Grid Info ======")]
    [Tooltip("Position of the player on the grid")] public Vector3Int gridPos;
    [Tooltip("Reference to the grid")] public GameObject grid;
    private Tilemap t;
    private PathFinder pathFinder;

    private void Awake()
    {
        //Set Up Heathbar
        slider = this.GetComponentInChildren<Slider>();
        slider.maxValue = characterStats.contains("maxHealth");
        updateHealthBar();

        t = grid.GetComponentInChildren<Tilemap>();

        //Alligns internal grid position with where it actually is;
        gridPos = t.WorldToCell(this.transform.position);
        this.transform.position = t.GetCellCenterWorld(gridPos);
    }

    private void Start()
    {
        pathFinder = new PathFinder();
    }

    private void Update()
    {
        
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

    public void updateHealthBar()
    {
        slider.value = characterStats.contains("currentHealth");
    }

    public List<GridTile> getTilesInRange(int range, MapManager map)
    {
        GridTile start = map.map[gridPos];

        List<GridTile> inRangeTiles = new List<GridTile>();
        inRangeTiles.Add(start);
        int step = 0;

        List<GridTile> previousStep = new List<GridTile>();
        previousStep.Add(start);

        while (step < range)
        {
            var surroundingTiles = new List<GridTile>();
            foreach (var tile in previousStep)
            {
                if (!tile.status.Equals("Occupied") && tile.gridPosition != gridPos )
                {
                    surroundingTiles.AddRange(pathFinder.getNeighbourAttackTiles(tile));
                }
                else if(tile.gridPosition == gridPos)
                {
                    surroundingTiles.AddRange(pathFinder.getNeighbourAttackTiles(tile));
                }
            }

            inRangeTiles.AddRange(surroundingTiles);
            previousStep = surroundingTiles.Distinct().ToList();
            step++;
        }

        return inRangeTiles.Distinct().ToList();
    }

    public void showMovementAndAttackRange(Tilemap movementTilemap, RuleTile moveTile, Tilemap attackTilemap, RuleTile attackTile, MapManager mapManager)
    {
        GridTile start = mapManager.map[gridPos];

        movementTiles = new List<GridTile>();

        int step = 0;

        List<GridTile> tilesToCheck = new List<GridTile>();
        tilesToCheck.Add(start);

        while (step < movementRange)
        {
            var surroundingTiles = new List<GridTile>();

            foreach (var tile in tilesToCheck)
            {
                surroundingTiles.AddRange(pathFinder.getNeighbourTiles(tile));
            }

            foreach (var tile in surroundingTiles)
            {
                if (!tile.status.Equals("Occupied") && pathFinder.findPath(start, tile).Sum(t => t.movementPenalty) <= movementRange)
                {
                    movementTiles.Add(tile);
                    tilesToCheck.Add(tile);
                }
            }
            step++;
        }

        movementTiles = movementTiles.Distinct().ToList();

        foreach (var tile in movementTiles)
        {
            movementTilemap.SetTile(tile.gridPosition, moveTile);
        }

        // Attack Range time
        attackTiles = new List<GridTile>();

        step = 0;

        List<GridTile> attackTilesToCheck = movementTiles;

        while (step < weapons[0].attackRange)
        {
            var surroundingTiles = new List<GridTile>();

            foreach (var tile in attackTilesToCheck)
            {
                surroundingTiles.AddRange(pathFinder.getNeighbourAttackTiles(tile));
            }

            foreach (var tile in surroundingTiles)
            {
                
                if (tile.status.Equals("Occupied"))
                {
                    attackTiles.Add(tile);
                }
                else
                {
                    attackTilesToCheck.Add(tile);
                }

                var path = pathFinder.findPath(start, tile);

                if((path.Sum(t => t.movementPenalty) <= weapons[0].attackRange + movementRange && path.Sum(t => t.movementPenalty) > movementRange )|| path.Count == weapons[0].attackRange + movementRange)
                {
                    attackTiles.Add(tile);
                }

            }
            step++;
        }

        attackTiles = attackTiles.Distinct().ToList();
        attackTiles.Remove(start);

        foreach (var tile in attackTiles)
        {
            attackTilemap.SetTile(tile.gridPosition, attackTile);
        }
    }
}
