using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class CursorMovement : MonoBehaviour
{
    public static CursorMovement instance;

    [Header("===== Character Details =====")]
    [Tooltip("Speed of the Character when they move")] public float speed;
    [Space(10)]

    [Header("===== Overlay Tile Stuff =====")]
    //TODO: No need for this to be a prefab.
    public GameObject overlayTilePrefab;
    private GameObject overlayTile;

    //Character Stuff
    [Header("===== Character Bools =====")]
    private Character character;
    [SerializeField] private bool characterIsSelected;
    [SerializeField] private bool characterIsMoving;
    [SerializeField] public bool characterActionPerformed;

    [Space(5)]

    [SerializeField] public Character selectedCharacter;
    private PathFinder pathFinder;
    private List<GridTile> path = new List<GridTile>();

    private List<GridTile> arrowPath = new List<GridTile>();

    [Space(10)]
    [Header("===== Sprite Reference =====")]
    [SerializeField] private Sprite mapCursor;

    [Space(10)]
    [Header("===== Tilemaps & Tiles =====")]

    public Tilemap tilemap;

    [Space(7)]
    public Tilemap arrowTilemap;
    public RuleTile arrowTile;

    [Space(7)]
    public Tilemap movementRangeTilemap;
    public RuleTile movementRuleTile;
    public Tile movementTile;

    [Space(7)]
    public Tilemap attackRangeTilemap;
    public RuleTile attackRuleTile;
    public Tile attackTileEmpty;
    public Tile attackTileActive;

    [Space(3)]
    public Tile friendlyTile;

    private void Start()
    {
        if (instance == null) instance = this;
        
        overlayTile = Instantiate(overlayTilePrefab);
        overlayTile.GetComponent<SpriteRenderer>().color = Color.clear;

        pathFinder = new PathFinder();

        characterIsSelected = false;
        characterIsMoving = false;

        var filler = tilemap.CellToWorld(new Vector3Int(0, 0));
        this.transform.position = tilemap.GetCellCenterWorld(new Vector3Int((int)filler.x, (int)filler.y));
    }

    // LateUpdate is called once per frame after Update
    void LateUpdate()
    {
        
        Vector3Int tilePos = getHoverTile();

        //var tileHit = GetFocusedOnTile();
        var characterHit = GetFocusedOnCharacter();

        // Gets the center of the current tile in world space.
        var tilePosWorld = tilemap.GetCellCenterWorld(tilePos);

        //If we are hovering off the tilemap.
        if(tilePos == Vector3Int.back)
        {
            GetComponent<SpriteRenderer>().sprite = null;
            return;
        }

        GetComponent<SpriteRenderer>().sprite = mapCursor;

        if (path.Count == 0) characterIsMoving = false;

        if (characterIsMoving)
        {
            arrowTilemap.ClearAllTiles();
            goto handle;
        }

        //Process for displaying the movement path the character would take given current tilePos.
        if (characterIsSelected && character.selectedAction != null)
        {
            //Resets the arrow to new tile.
            arrowTilemap.ClearAllTiles();
            var ap = generateArrowPath(tilePos);

            //if (ap.Sum(t => t.movementPenalty) <= character.movementRange + character.listOfWeapons[0].range &&
            //    (selectedCharacter.attackTiles.Contains(MapManager.instance.map[tilePos]) || selectedCharacter.movementTiles.Contains(MapManager.instance.map[tilePos])))

            if(selectedCharacter.attackTiles.Contains(MapManager.instance.map[tilePos]) || selectedCharacter.movementTiles.Contains(MapManager.instance.map[tilePos]))
            {
                foreach (GridTile gt in arrowPath)
                {
                    arrowTilemap.SetTile(gt.gridPosition, arrowTile);
                }
            }
        }

        if (!UserInput.instance.selectInput) goto handle;

        //If we click anywhere on the map;

        overlayTile.transform.position = tilePosWorld;

        movementRangeTilemap.ClearAllTiles();
        attackRangeTilemap.ClearAllTiles();

        //We click on a character
        if (characterHit.HasValue)
        {
            //If we click on any character;

            //overlayTile.GetComponent<SpriteRenderer>().color = Color.clear;
            characterIsMoving = false;

            character = characterHit.Value.transform.gameObject.GetComponent<Character>();

            //If the tilePos isn't the character's position
            if (!character.gridPosition.Equals(tilePos))
            {
                generatePath(tilePos);
                goto handle;
            }

            //TODO: Attempted Re-write of the bottom code.

            if (characterIsSelected && selectedCharacter.attackTiles.Contains(MapManager.instance.map[character.gridPosition]))
            {
                var action = (Action)selectedCharacter.selectedAction;
                Debug.Log(selectedCharacter.characterName + " performed Action + " + action.name + " against " + character.characterName);

                action.performAction(selectedCharacter.characterStats, character);

                character = selectedCharacter;
                //generatePath(arrowPath[Mathf.Max(arrowPath.Count - (character.listOfWeapons[0].range + 1), 0)].gridPosition);
                generatePath(arrowPath[Mathf.Max(arrowPath.Count - (action.range + 1), 0)].gridPosition);

                characterActionPerformed = true;
                goto handle;
            }

            //TODO: Redo this whole section here since Actios are changing.

            //If we click on an Enemy Character.
            /**
            if(character.alignment == Character.AlignmentStatus.Enemy)
            {
                if (characterIsSelected && selectedCharacter.attackTiles.Contains(MapManager.instance.map[character.gridPosition]))
                {
                    Debug.Log(selectedCharacter.characterName + " performed Action against " + character.characterName);
                    //selectedCharacter.listOfWeapons[0].performAction(selectedCharacter.characterStats, character);
                    var action = (Action)selectedCharacter.selectedAction;
                    action.performAction(selectedCharacter.characterStats, character);

                    character = selectedCharacter;
                    //generatePath(arrowPath[Mathf.Max(arrowPath.Count - (character.listOfWeapons[0].range + 1), 0)].gridPosition);
                    generatePath(arrowPath[Mathf.Max(arrowPath.Count - (action.range + 1), 0)].gridPosition);

                    characterActionPerformed = true;
                    goto handle;
                }
            }*/

            //We click on the same character twice.
            if (characterIsSelected && character == selectedCharacter)
            {
                Debug.Log("Deselected: " + selectedCharacter);
                movementRangeTilemap.ClearAllTiles();
                attackRangeTilemap.ClearAllTiles();
                character.isSelected = false;
                characterIsSelected = false;

                selectedCharacter.selectedAction = null;

                selectedCharacter = null;
            }

            //We click of a different character.
            if (characterIsSelected && character != selectedCharacter)
            {
                Debug.Log("Clicked on a different Character");
                movementRangeTilemap.ClearAllTiles();
                attackRangeTilemap.ClearAllTiles();
                character.isSelected = false;
                character = null;
                characterIsSelected = false;
            }

            //We click on the selected character.
            if (!characterIsSelected && character == selectedCharacter)
            {
                Debug.Log("Clicked on selected Character");

                character.isSelected = true;
                characterIsSelected = true;

                selectedCharacter = character;

                //Is this necessary or can the instance of this be placed with character.showGUI();
                GameManager.instance.showGUI(selectedCharacter);

                character.showMovementRange();
            }
        }
        //We click on a tile instead
        else
        {
            //If we are trying to pick a tile
            if (!characterIsSelected)
            {
                //characterActionPerformed = false;
                overlayTile.GetComponent<SpriteRenderer>().color = Color.white;
            }
            else if (selectedCharacter.attackTiles.Contains(MapManager.instance.map[tilePos]))
            {
                if(MapManager.instance.map[tilePos].status == "Enemy")
                {
                    Debug.Log("Clicked on an Enemy");
                    characterActionPerformed = true;
                }
            }
            else if (selectedCharacter.movementTiles.Contains(MapManager.instance.map[tilePos]))
            {
                characterActionPerformed = true;
            }

            generatePath(tilePos);
            arrowTilemap.ClearAllTiles();
        }

        handle:
        handlePath();
    }


    private void handlePath()
    {
        arrowPath.Clear();


        //If the path is greater than 0, and is within the character's movement radius, move along the path.
        if (path.Count > 0 && path.Sum(t => t.movementPenalty) <= character.movementRange)
        {
            moveAlongPath();
        }
        else if (character != null && character.isSelected)
        {
            path.Clear();
        }
    }

    private void generatePath(Vector3Int tilePos)
    {
        if (characterIsSelected && character.gridPosition.Equals(tilePos))
        {
            characterIsSelected = false;
        }

        if (characterIsSelected)
        {
            overlayTile.GetComponent<SpriteRenderer>().color = Color.white;

            path.Clear();
            path = pathFinder.findPath(MapManager.instance.map[character.gridPosition], MapManager.instance.map[tilePos]);
            character.isSelected = false;
            characterIsSelected = false;
        }

        if(character == null)
        {
            return;
        }
        
        //path.Count <= character.movementRange
        if (path.Sum(t => t.movementPenalty) <= character.movementRange && path.Count > 0 && !characterIsMoving)
        {
            foreach (CapturePoint c in MapManager.instance.capturePoints)
            {
                if (c.capturePointTilemapPositions.Contains(character.gridPosition))
                {
                    MapManager.instance.updateTileStatus(character.gridPosition, "Objective");
                    break;
                }
                else
                {
                    MapManager.instance.updateTileStatus(character.gridPosition, "NotOccupied");
                }
            }

            MapManager.instance.updateTileStatus(tilePos, "Friendly");

        }
    }


    private List<GridTile> generateArrowPath(Vector3Int tilePos)
    {
        if (characterIsSelected && !character.gridPosition.Equals(tilePos))
        {
            overlayTile.GetComponent<SpriteRenderer>().color = Color.white;

            arrowPath.Clear();
            arrowPath = pathFinder.findArrowPath(MapManager.instance.map[character.gridPosition], MapManager.instance.map[tilePos]);
            //arrowPath = pathFinder.findPath(MapManager.instance.map[character.gridPos], MapManager.instance.map[tilePos]);
        }

        return arrowPath;
    }

    private void moveAlongPath()
    {
        characterIsMoving = true;

        var step = speed * Time.deltaTime;

        character.transform.position = Vector2.MoveTowards(character.transform.position, tilemap.GetCellCenterWorld(path[0].gridPosition), step);

        if (Vector2.Distance(character.transform.position, tilemap.GetCellCenterWorld(path[0].gridPosition)) < 0.0001f)
        {
            character.updateGridPos(path[0].gridPosition);
            path.RemoveAt(0);
        }

    }

    //Retuns the current tile the cursor is hovering over as a Vector3Int position on the tilemap.
    public Vector3Int getHoverTile()
    {
        var mousPos = Camera.main.ScreenToWorldPoint(UserInput.instance.moveInput);
        var mousPos2D = new Vector2(mousPos.x, mousPos.y);

        Vector3Int tilePos = tilemap.WorldToCell(mousPos2D);

        Tile tile = tilemap.GetTile<Tile>(tilePos);

        this.transform.position = tilemap.GetCellCenterWorld(tilePos);

        if (tile != null)
        {
            return tilePos;
        }

        return Vector3Int.back;
    }

    //Raycast done from Mouse position to determine if we are hovering over a character.
    //  * Could possibly change to be not a raycast but not sure if needed.
    public RaycastHit2D? GetFocusedOnCharacter()
    {
        Vector3 mousPos = Camera.main.ScreenToWorldPoint(UserInput.instance.moveInput);
        Vector2 mousPos2D = new Vector2(mousPos.x, mousPos.y);

        Vector3Int tilePos = tilemap.WorldToCell(mousPos2D);

        RaycastHit2D hit = Physics2D.Raycast(tilemap.GetCellCenterWorld(tilePos), Vector2.zero, 1, LayerMask.GetMask("Character"));

        if (hit)
        {
            return hit;
        }

        return null;
    }
}
