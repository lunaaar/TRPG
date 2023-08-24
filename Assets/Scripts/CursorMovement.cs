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

    [Header("===== Cursor Sprite References =====")]
    [SerializeField] private Sprite mapCursor;
    [SerializeField] private Sprite voidCursor;

    [Header("===== Overlay Tile Stuff =====")]
    //TODO: No need for this to be a prefab.
    public GameObject overlayTilePrefab;
    private GameObject overlayTile;

    //Character Stuff
    [Header("===== Character Bools =====")]
    private Character character;
    [SerializeField] private bool characterIsSelected;
    [SerializeField] private bool characterIsMoving;
    [SerializeField] private Character selectedCharacter;
    private PathFinder pathFinder;
    private List<GridTile> path = new List<GridTile>();

    private List<GridTile> arrowPath = new List<GridTile>();

    [Space(10)]
    [Header("===== References =====")]
    public Tilemap tilemap;

    [Space(7)]
    public Tilemap arrowTilemap;
    public RuleTile arrowTile;

    [Space(7)]
    public Tilemap movementRangeTilemap;
    public RuleTile movementTile;

    [Space(7)]
    public Tilemap attackRangeTilemap;
    public RuleTile attackTile;

    [Space(7)]
    public Tile healTile;

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
        this.GetComponent<SpriteRenderer>().sprite = mapCursor;
    }

    // LateUpdate is called once per frame after Update
    void LateUpdate()
    {
        Vector3Int tilePos = getHoverTile();

        //var tileHit = GetFocusedOnTile();
        var characterHit = GetFocusedOnCharacter();

        //We are not hovering over a tile.
        if (tilePos == Vector3Int.back)
        {
            this.GetComponent<SpriteRenderer>().sprite = voidCursor;
            return;
        }
        else
        {
            this.GetComponent<SpriteRenderer>().sprite = mapCursor;
        }

        // Gets the center of the current tile in world space.
        var tilePosWorld = tilemap.GetCellCenterWorld(tilePos);


        if (path.Count == 0) characterIsMoving = false;

        if (characterIsMoving)
        {
            arrowTilemap.ClearAllTiles();
            goto handle;
        }

        //Process for displaying the movement path the character would take given current tilePos.
        if (characterIsSelected)
        {
            //Resets the arrow to new tile.
            arrowTilemap.ClearAllTiles();
            var ap = generateArrowPath(tilePos);

            if (ap.Sum(t => t.movementPenalty) <= character.movementRange + character.weapons[0].attackRange &&
                selectedCharacter.attackTiles.Contains(MapManager.instance.map[tilePos]) || selectedCharacter.movementTiles.Contains(MapManager.instance.map[tilePos]))
            {
                foreach (GridTile gt in arrowPath)
                {
                    arrowTilemap.SetTile(gt.gridPosition, arrowTile);
                }
            }
        }

        //New stuff Starts Here

        selectedCharacter = GameManager.instance.combatOrder[0];

        if(selectedCharacter.alignment == Character.AlignmentStatus.Enemy)
        {
            //DO ENEMY COMBAT HERE
            // CALL SOME MENTHOD IN ENEMY
            
            GameManager.instance.updateCombatOrder();
            goto handle;
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
            if (!character.gridPos.Equals(tilePos))
            {
                generatePath(tilePos);
                goto handle;
            }

            //If we click on an Enemy Character.
            if(character.alignment == Character.AlignmentStatus.Enemy)
            {
                if (characterIsSelected && selectedCharacter.attackTiles.Contains(MapManager.instance.map[character.gridPos]))
                {
                    Debug.Log(selectedCharacter.characterName + " attacked " + character.characterName);
                    selectedCharacter.weapons[0].Attack(selectedCharacter.characterStats, character);
                    character = selectedCharacter;
                    generatePath(arrowPath[Mathf.Max(arrowPath.Count - (character.weapons[0].attackRange + 1), 0)].gridPosition);
                    GameManager.instance.updateCombatOrder();
                    goto handle;
                }
            }

            //We click on any Friendly Character.

            if (characterIsSelected && character == selectedCharacter)
            {
                movementRangeTilemap.ClearAllTiles();
                attackRangeTilemap.ClearAllTiles();
                character = null;
                characterIsSelected = false;
                Debug.Log(character);
                character.isSelected = false;
            }

            //We click on the selected character.
            if (!characterIsSelected && character == selectedCharacter)
            {
                character.isSelected = true;
                characterIsSelected = true;

                selectedCharacter = character;

                GameManager.instance.showGUI(selectedCharacter);

                character.showMovementAndAttackRange(movementRangeTilemap, movementTile, attackRangeTilemap, attackTile, MapManager.instance);
            }
        }
        //We click on a tile instead
        else
        {
            //If we are trying to pick a tile
            if (!characterIsSelected)
            {
                overlayTile.GetComponent<SpriteRenderer>().color = Color.white;
            }
            else if (selectedCharacter.attackTiles.Contains(MapManager.instance.map[tilePos]) || selectedCharacter.movementTiles.Contains(MapManager.instance.map[tilePos]))
            {
                Debug.Log("TEST");
                GameManager.instance.updateCombatOrder();
            }

            generatePath(tilePos);
            arrowTilemap.ClearAllTiles();


            
        }

        handle:
        handlePath();



        /**
        if (UserInput.instance.selectInput)
        {
            overlayTile.transform.position = tilePosWorld;

            movementRangeTilemap.ClearAllTiles();
            attackRangeTilemap.ClearAllTiles();

            //We click on a character on it.
            if (characterHit.HasValue)
            {
                overlayTile.GetComponent<SpriteRenderer>().color = Color.clear;
                
                characterIsMoving = false;

                character = characterHit.Value.transform.gameObject.GetComponent<Character>();

                //if we are hovering over the tile the character is on (mostly here for if we are trying to click on the tile behind a character)
                if (character.gridPos.Equals(tilePos))
                {
                    //If we click on the same character twice
                    if (character == selectedCharacter)
                    {
                        movementRangeTilemap.ClearAllTiles();
                        attackRangeTilemap.ClearAllTiles();
                        selectedCharacter = null;
                        characterIsSelected = false;
                        return;
                    }

                    switch (character.alignment) {
                        /**
                        case (Character.AlignmentStatus.Friendly):
                            character.isSelected = true;
                            characterIsSelected = true;

                            selectedCharacter = character;

                            gamemanager.showGUI(selectedCharacter);

                            character.showMovementAndAttackRange(movementRangeTilemap, movementTile, attackRangeTilemap, attackTile, mapManager);
                            
                            break;
                        case (Character.AlignmentStatus.Enemy):

                            if (characterIsSelected && selectedCharacter.attackTiles.Contains(mapManager.map[character.gridPos]))
                            {
                                Debug.Log(selectedCharacter.characterName + " attacked " + character.characterName);
                                selectedCharacter.weapons[0].Attack(selectedCharacter.characterStats, character);
                                character = selectedCharacter;
                                generatePath(arrowPath[Mathf.Max(arrowPath.Count - (character.weapons[0].attackRange + 1),0)].gridPosition);
                            }

                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    //If we are trying to pick a tile
                    generatePath(tilePos);
                }
            }
            //We click on not a character
            else
            {
                //If we are trying to pick a tile
                if (!characterIsSelected)
                {
                    overlayTile.GetComponent<SpriteRenderer>().color = Color.white;
                }

                generatePath(tilePos);
                arrowTilemap.ClearAllTiles();
            }
        }
        //Player is hovering off the grid.
        else
        {

        }
        
        arrowPath.Clear();

        //If the path is greater than 0, and is within the character's movement radius, move along the path.
        if (path.Count > 0 && path.Sum(t => t.movementPenalty) <= character.movementRange)
        {
            moveAlongPath();
            GameManager.instance.updateCombatOrder();
        }
        else if (character != null && character.isSelected)
        {
            path.Clear();
        }
        */
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
        if (characterIsSelected && character.gridPos.Equals(tilePos))
        {
            characterIsSelected = false;
        }

        if (characterIsSelected)
        {
            overlayTile.GetComponent<SpriteRenderer>().color = Color.white;

            path.Clear();
            path = pathFinder.findPath(MapManager.instance.map[character.gridPos], MapManager.instance.map[tilePos]);
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
                if (c.capturePointTilemapPositions.Contains(character.gridPos))
                {
                    MapManager.instance.updateOccupiedStatus(character.gridPos, "Objective");
                    break;
                }
                else
                {
                    MapManager.instance.updateOccupiedStatus(character.gridPos, "NotOccupied");
                }
            }

            MapManager.instance.updateOccupiedStatus(tilePos, "Occupied");

        }
    }


    private List<GridTile> generateArrowPath(Vector3Int tilePos)
    {
        if (characterIsSelected && !character.gridPos.Equals(tilePos))
        {
            overlayTile.GetComponent<SpriteRenderer>().color = Color.white;

            arrowPath.Clear();
            arrowPath = pathFinder.findArrowPath(MapManager.instance.map[character.gridPos], MapManager.instance.map[tilePos]);
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
