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

    [Tooltip("Character Name")]
    public string characterName;

    public Sprite characterImage;

    [Range(1, 6)] [Tooltip("How many tiles can this character move?")]
    public int movementRange = 3;

    public Stats characterStats;
    public GameObject uiReference;

    public enum AlignmentStatus { Friendly, Neutral, Enemy }
    [Tooltip("Alignment Status of the Character, determines how the manager treats this character.")] public AlignmentStatus alignment;

    [Header("===== Lists of Things ======")]

    [Tooltip("List of all the weapons")]
    public List<Weapon> listOfWeapons;

    [Space(2)]

    [Tooltip("List of all the spells")]
    public List<Spell> listOfSpells;

    [Space(2)]

    [Tooltip("List of all the abilities")]
    public List<Ability> listOfAbilities;

    [Space(2)]

    [Tooltip("List of any changes the player has made to another character")]
    public List<Action.Modification> listOfModifications;

    [Header("===== Selected Info =====")]

    [Space(5)]
    public ScriptableObject selectedAction;

    [Space(2)]

    [Tooltip("Is the Character actively selected?")]
    public bool isSelected;

    [Space(5)]
    [Header("====== Character Info ======")]
    public List<GridTile> movementTiles;
    public List<GridTile> attackTiles;

    private Slider healthSlider;
    
    [Space(10)]

    [Header("====== Grid Info ======")]
    [Tooltip("Position of the player on the grid")] public Vector3Int gridPosition;
    //private PathFinder pathFinder;

    [Header("TEST STUFF")]
    public GameObject buttonReference;
    public Transform transformReference;

    private void Awake()
    {
        //Set Up Heathbar
        healthSlider = this.GetComponentInChildren<Slider>();
        healthSlider.maxValue = characterStats.contains("maxHealth");
        updateHealthBar();
    }

    private void Start()
    {
        selectedAction = null;

        //pathFinder = new PathFinder();
    }

    private void Update()
    {

    }

    public void setupActionButtons(Vector3Int gP)
    {
        Debug.Log("SHOW GUI");
        
        foreach (Transform child in transformReference)
        {
            Destroy(child.gameObject);
        }

        //? I think for now, lets try and get some simple system in place so we can at least test
        //? The rest of the code around how itll end up working.

        Vector3 buttonPosition = new Vector3(0, 0, 0);

        List<Action> listOfAll = new List<Action>(listOfWeapons);
        listOfAll.AddRange(listOfSpells);
        listOfAll.AddRange(listOfAbilities);

        GameObject button;
        Vector3 position;

        foreach(Action a in listOfAll)
        {
            button = Instantiate(buttonReference, transformReference);
            position = Vector3.zero;

            button.GetComponent<RectTransform>().anchoredPosition = buttonPosition;
            //button.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            buttonPosition.y -= 16;
            button.GetComponent<Button>().onClick.AddListener(() => setSelectedAction(a, gP));

            /**if(a.actionIcon != null)
            {
                button.GetComponentsInChildren<Image>()[1].sprite = a.actionIcon;
                button.GetComponentsInChildren<Image>()[1].color = Color.black;
                button.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
            }
            else
            {*/
            button.GetComponentInChildren<TextMeshProUGUI>().text = " " + a.name;
            //}

            switch (a.actionType)
            {
                case (Action.ActionType.Weapon):
                    button.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
                    break;
                case (Action.ActionType.Spell):
                    button.GetComponentInChildren<TextMeshProUGUI>().color = Color.blue;
                    break;
                case (Action.ActionType.Ability):
                    button.GetComponentInChildren<TextMeshProUGUI>().color = Color.green;
                    break;
                default:
                    Debug.LogError("Defaulted in showGUI foreach loop.");
                    break;
            }
        }

        button = Instantiate(buttonReference, transformReference);
        position = button.GetComponent<RectTransform>().localPosition;

        button.GetComponentInChildren<TextMeshProUGUI>().text = " No Action";
        button.GetComponent<RectTransform>().anchoredPosition = buttonPosition;
        button.GetComponent<Button>().onClick.AddListener(() => noAction());
    }

    public void hideGUI()
    {
        foreach (Transform child in transformReference)
        {
            Destroy(child.gameObject);
        }
    }

    public void noAction()
    {
        Debug.Log("Test No Action");
        
        MapManager.instance.resetAttackTiles();
        selectedAction = null;

        CursorMovement.instance.characterActionPerformed = true;
    }

    public void setSelectedAction(ScriptableObject scriptableObject, Vector3Int gP)
    {
        //CursorMovement.instance.attackRangeTilemap.ClearAllTiles();
        MapManager.instance.resetAttackTiles();

        Debug.Log("Set selectedAction to: " + "\n" + scriptableObject.name);
        selectedAction = scriptableObject;

        //Annoyingly, you cannot case on Types, so we must convert it to a string and case by that.

        //? This is only here because of bad code, ideally by the end of this project it should just be the else.
        if (selectedAction.GetType().BaseType.BaseType.ToString() == "Action") 
        {
            var action = (Action)selectedAction;

            int moveRange = movementRange;
            if (CursorMovement.instance.characterMovementPerformed) moveRange = 0;
            attackTiles = action.showActionRange(movementTiles, MapManager.instance.map[gridPosition], moveRange, alignment.ToString(), false);
        }
        else if(selectedAction.GetType().BaseType.BaseType.BaseType.ToString() == "Action")
        {
            //TODO: THIS IS THE CORRECT TIMELINE.
            
            var action = (Action)selectedAction;

            int moveRange = movementRange;
            if (CursorMovement.instance.characterMovementPerformed) moveRange = 0;
            attackTiles = action.showActionRange(movementTiles, MapManager.instance.map[gridPosition], moveRange, alignment.ToString(), false);

        }
    }


    //? Needs to be updated for new tilemap.
    //. Passed in a gridPosition
    public void updateGridPos(Vector3Int gridPos)
    {
        var tilemap = MapManager.instance.floorTilemaps[gridPos.z];

        gridPosition = gridPos;

        transform.position = tilemap.GetCellCenterWorld(gridPos);

        GetComponent<SpriteRenderer>().sortingOrder = gridPos.z;

        gridPosition.z -= 1;

        //var temp = MapManager.instance.floorTilemaps[v.z].GetCellCenterWorld(v);

        //v.z -= 1;
    }

    public void updateGridPos()
    {
        var sortOrder = GetComponent<SpriteRenderer>().sortingOrder;

        var tilemap = MapManager.instance.floorTilemaps[sortOrder - 1];

        gridPosition = tilemap.WorldToCell(transform.position);

        transform.position = tilemap.GetCellCenterWorld(gridPosition);

        gridPosition.z -= 1;
    }

    public void updateHealthBar()
    {
        healthSlider.value = characterStats.contains("currentHealth");

        //? This needs to update stat on pause menu.
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
                    surroundingTiles.AddRange(GameManager.instance.pathFinder.getNeighbourAttackTiles(tile));
                }
                else if(tile.gridPosition == gridPosition)
                {
                    surroundingTiles.AddRange(GameManager.instance.pathFinder.getNeighbourAttackTiles(tile));
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
        movementTiles.Add(start);

        var surroundingTiles = new List<GridTile>();

        int range = 0;

        if (!CursorMovement.instance.characterMovementPerformed) range = movementRange;

        while (step < range)
        {
            surroundingTiles.Clear();

            foreach (var tile in tilesToCheck)
            {
                //surroundingTiles.AddRange(pathFinder.getNeighbourTiles(tile));
                surroundingTiles.AddRange(GameManager.instance.pathFinder.getNeighbourTiles(tile));
            }

            foreach (var tile in surroundingTiles)
            {
                if ((tile.status.Equals("NotOccupied")  || tile.status.Equals("Objective"))
                    && GameManager.instance.pathFinder.findPath(start, tile).Sum(t => t.movementPenalty) <= movementRange)
                {
                    movementTiles.Add(tile);
                    tilesToCheck.Add(tile);
                }
            }
            step++;
        }

        movementTiles = new List<GridTile>(movementTiles.Distinct());

        if (CursorMovement.instance.characterMovementPerformed)
        {
            //CursorMovement.instance.movementRangeTilemap.SetTile(start.gridPosition, CursorMovement.instance.movementTile);
            MapManager.instance.floorTilemaps[start.gridPosition.z].SetColor(start.gridPosition, GameManager.instance.movementEmptyColor);
            return;
        }

        var list = new List<GridTile>();

        foreach (var tile in movementTiles.ToList())
        {
            var z = tile.gridPosition.z;
            
            if(!MapManager.instance.map.ContainsKey(new Vector3Int(tile.gridPosition.x, tile.gridPosition.y, z + 1)))
            {
                MapManager.instance.floorTilemaps[z].SetTileFlags(tile.gridPosition, TileFlags.None);
                MapManager.instance.floorTilemaps[z].SetColor(tile.gridPosition, GameManager.instance.movementEmptyColor);
                list.Add(tile);
            }
        }

        movementTiles = list.ToList();
    }


    public void setUpStatScreen()
    {
        var characterPortrait = uiReference.GetComponentsInChildren<Image>()[1];
        characterPortrait.sprite = characterImage;
        characterPortrait.color = Color.white;

        var arrayOfTexts = uiReference.GetComponentsInChildren<TextMeshProUGUI>();

        arrayOfTexts[0].text = name;
        arrayOfTexts[1].text = characterStats.contains("maxHealth").ToString();
        arrayOfTexts[3].text = characterStats.contains("currentHealth").ToString();

        arrayOfTexts[6].text = characterStats.contains("Attack").ToString();
        arrayOfTexts[8].text = characterStats.contains("Magic").ToString();
        arrayOfTexts[10].text = characterStats.contains("Defense").ToString();
        arrayOfTexts[12].text = characterStats.contains("Resistance").ToString();
        arrayOfTexts[14].text = characterStats.contains("Speed").ToString();
    }
}
