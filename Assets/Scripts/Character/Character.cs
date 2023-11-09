using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class Character : MonoBehaviour
{
    [Header("====== Character Stats ======")]
    [Tooltip("Character Name")] public string characterName;
    public Sprite characterImage;
    [Range(1, 6)] [Tooltip("How many tiles can this character move?")] public int movementRange = 3;

    public Stats characterStats;
    public GameObject uiReference;

    public List<Weapon> listOfWeapons;
    [Space(2)]
    public List<Spell> listOfSpells;
    [Space(2)]
    public List<Ability> listOfAbilities;
    

    [Space(5)]
    public ScriptableObject selectedAction;

    [Space(2)]
    [Tooltip("Is the Character actively selected?")] public bool isSelected;

    [Space(5)]
    [Header("====== Character Info ======")]
    public List<GridTile> movementTiles;
    public List<GridTile> attackTiles;
    public enum AlignmentStatus { Friendly, Neutral, Enemy}
    [Tooltip("Alignment Status of the Character, determines how the manager treats this character.")] public AlignmentStatus alignment;
    private Slider healthSlider;
    
    [Space(10)]

    [Header("====== Grid Info ======")]
    [Tooltip("Position of the player on the grid")] public Vector3Int gridPosition;
    [Tooltip("Reference to the grid")] public GameObject gridReference;
    private Tilemap tilemapReference;
    private PathFinder pathFinder;

    [Header("TEST STUFF")]
    public GameObject buttonReference;
    public Transform transformReference;

    private void Awake()
    {
        //Set Up Heathbar
        healthSlider = this.GetComponentInChildren<Slider>();
        healthSlider.maxValue = characterStats.contains("maxHealth");
        updateHealthBar();

        tilemapReference = gridReference.GetComponentInChildren<Tilemap>();

        //Alligns internal grid position with where it actually is;
        gridPosition = tilemapReference.WorldToCell(this.transform.position);
        this.transform.position = tilemapReference.GetCellCenterWorld(gridPosition);

        selectedAction = null;
    }

    private void Start()
    {
        pathFinder = new PathFinder();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            var bounds = CursorMovement.instance.movementRangeTilemap.cellBounds;
            Debug.Log(CursorMovement.instance.movementRangeTilemap.GetTilesRangeCount(bounds.max, bounds.min));
        }
    }

    public void showGUI()
    {
        foreach (Transform child in transformReference)
        {
            Destroy(child.gameObject);
        }

        //I think for now, lets try and get some simple system in place so we can at least test
        //The rest of the code around how itll end up working.

        Vector3 buttonPosition = new Vector3(-15, 0, 0);

        //Sets up all the Weapon Buttons.
        foreach (Weapon w in listOfWeapons)
        {
            GameObject b = Instantiate(buttonReference, transformReference);
            var pos = b.GetComponent<RectTransform>().localPosition;
            b.GetComponent<RectTransform>().localPosition = new Vector3(buttonPosition.x + 50, pos.y, pos.z);
            buttonPosition.x += 50;

            b.GetComponent<Image>().color = Color.red;
            b.GetComponentInChildren<TextMeshProUGUI>().text = w.name;
            b.GetComponent<Button>().onClick.AddListener(() => setSelectedAction(w));
        }
        //Sets up all the Spell Buttons.
        foreach (Spell s in listOfSpells)
        {
            GameObject b = Instantiate(buttonReference, transformReference);
            var pos = b.GetComponent<RectTransform>().localPosition;
            b.GetComponent<RectTransform>().localPosition = new Vector3(buttonPosition.x + 50, pos.y, pos.z);
            buttonPosition.x += 50;

            b.GetComponent<Image>().color = Color.blue;
            b.GetComponentInChildren<TextMeshProUGUI>().text = s.name;
            b.GetComponent<Button>().onClick.AddListener(() => setSelectedAction(s));
        }

        //Sets up all the Ability Buttons.
        foreach(Ability a in listOfAbilities)
        {
            GameObject b = Instantiate(buttonReference, transformReference);
            var pos = b.GetComponent<RectTransform>().localPosition;
            b.GetComponent<RectTransform>().localPosition = new Vector3(buttonPosition.x + 50, pos.y, pos.z);
            buttonPosition.x += 50;

            b.GetComponent<Image>().color = Color.green;
            b.GetComponentInChildren<TextMeshProUGUI>().text = a.name;
            b.GetComponent<Button>().onClick.AddListener(() => setSelectedAction(a));
        }
    }

    public void setSelectedAction(ScriptableObject scriptableObject)
    {
        CursorMovement.instance.attackRangeTilemap.ClearAllTiles();

        Debug.Log("Set selectedAction to " + scriptableObject.name);
        selectedAction = scriptableObject;

        //Annoyingly, you cannot case on Types, so we must convert it to a string and case by that.

        if (selectedAction.GetType().BaseType.BaseType.ToString() == "Action")
        {
            var action = (Action)selectedAction;
            attackTiles = action.showActionRange(movementTiles, MapManager.instance.map[gridPosition], movementRange);
        }
    }


    public void updateGridPos(Vector3Int v)
    {
        gridPosition = v;
        transform.position = tilemapReference.GetCellCenterWorld(gridPosition);
    }

    public void updateGridPos()
    {
        gridPosition = gridReference.GetComponentInChildren<Tilemap>().WorldToCell(transform.position);
        transform.position = tilemapReference.GetCellCenterWorld(gridPosition);
    }

    public void updateHealthBar()
    {
        healthSlider.value = characterStats.contains("currentHealth");
    }

    public List<GridTile> getTilesInRange(int range)
    {
        GridTile start = MapManager.instance.map[gridPosition];

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
                if (!tile.status.Equals("Occupied") && tile.gridPosition != gridPosition )
                {
                    surroundingTiles.AddRange(pathFinder.getNeighbourAttackTiles(tile));
                }
                else if(tile.gridPosition == gridPosition)
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

    public void showMovementRange()
    {  
        GridTile start = MapManager.instance.map[gridPosition];

        movementTiles = new List<GridTile>();

        int step = 0;

        List<GridTile> tilesToCheck = new List<GridTile>();
        tilesToCheck.Add(start);

        var surroundingTiles = new List<GridTile>();

        while (step < movementRange)
        {
            surroundingTiles.Clear();

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

        movementTiles = new List<GridTile>(movementTiles.Distinct());

        foreach (var tile in movementTiles)
        {

            CursorMovement.instance.movementRangeTilemap.SetTile(tile.gridPosition, CursorMovement.instance.movementTile);
            //movementTilemap.SetTile(tile.gridPosition, CursorMovement.instance.movementTile);
        }
    }
}
