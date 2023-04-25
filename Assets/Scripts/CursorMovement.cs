using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CursorMovement : MonoBehaviour
{
    
    [Header("===== Character Details =====")]
    [Tooltip("Speed of the Character when they move")] public float speed;
    [Space(10)]

    [Header("===== Overlay Tile Stuff =====")]
    //TODO: No need for this to be a prefab. Maybe change how this works, maybe ask Nagy how we want to show highlighting
    public GameObject overlayTilePrefab;
    private GameObject overlayTile;
    public GameObject overlayTileContainer;

    private Character character;
    private PathFinder pathFinder;
    private List<GridTile> path = new List<GridTile>();

    private List<GridTile> rangeTiles = new List<GridTile>();

    [Space(10)]
    [Header("===== References =====")]
    public Tilemap tilemap;
    public MapManager mapManager;

    private void Start()
    {
        overlayTile = Instantiate(overlayTilePrefab);

        pathFinder = new PathFinder();
    }

    // LateUpdate is called once per frame after Update
    void LateUpdate()
    {
        Vector3Int tilePos = getHoverTile();

        //var tileHit = GetFocusedOnTile();
        var characterHit = GetFocusedOnCharacter();

        //We are hovering a tile.
        if (tilePos != Vector3Int.back)
        {
            // Gets the center of the current tile in world space.
            var tilePosWorld = tilemap.GetCellCenterWorld(tilePos);
            this.transform.position = tilePosWorld;
            
            if (Input.GetMouseButtonDown(0))
            {
                foreach (Transform child in overlayTileContainer.transform)
                {
                    Destroy(child.gameObject);
                }

                overlayTile.transform.position = tilePosWorld; 
                
                //We click on a tile with a character
                if (characterHit.HasValue)
                {
                    character = characterHit.Value.transform.gameObject.GetComponent<Character>();

                    //We clicked on a character on their tile.
                    if (character.gridPos.Equals(tilePos))
                    {
                        overlayTile.GetComponent<SpriteRenderer>().color = Color.green;
                        character.GetComponent<Character>().isSelected = true;

                        rangeTiles = pathFinder.getTilesInRange(mapManager.map[tilePos], character.movementRange);

                        foreach(var tile in rangeTiles)
                        {
                            var oPrefab = Instantiate(overlayTilePrefab, tilemap.GetCellCenterWorld(tile.gridPosition), Quaternion.identity, overlayTileContainer.transform);
                            oPrefab.GetComponent<SpriteRenderer>().color = character.highlightColor;
                        }
                    }
                    //This is if we click on a tile behind a character.
                    else
                    {
                        generatePath(tilePos);
                    }
                }
                //We click on not a character
                else
                {
                    generatePath(tilePos);
                }
            }
        }
        //Player is hovering off the grid.
        else
        {
            
        }

        //If the path is greater than 0, and is within the character's movement radius, move along the path.
        if (path.Count > 0 && path.Count <= character.movementRange)
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
        overlayTile.GetComponent<SpriteRenderer>().color = Color.white;
        if (character != null && character.isSelected)
        {
            path.Clear();
            path = pathFinder.findPath(mapManager.map[character.gridPos], mapManager.map[tilePos]);
            character.isSelected = false;

            mapManager.updateOccupiedStatus(character.gridPos, false);
            mapManager.updateOccupiedStatus(tilePos, true);
        }
    }

    private void moveAlongPath()
    {
        var step = speed * Time.deltaTime;

        character.transform.position = Vector2.MoveTowards(character.transform.position, tilemap.GetCellCenterWorld(path[0].gridPosition), step);

        if(Vector2.Distance(character.transform.position, tilemap.GetCellCenterWorld(path[0].gridPosition)) < 0.0001f)
        {
            character.updateGridPos(path[0].gridPosition);
            path.RemoveAt(0);
        }

    }

    //Retuns the current tile the cursor is hovering over as a Vector3Int position on the tilemap.
    public Vector3Int getHoverTile()
    {
        var mousPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var mousPos2D = new Vector2(mousPos.x, mousPos.y);

        Vector3Int tilePos = tilemap.WorldToCell(mousPos2D);

        Tile tile = tilemap.GetTile<Tile>(tilePos);

        if(tile != null)
        {
            return tilePos;
        }

        return Vector3Int.back;
    }

    //Raycast done from Mouse position to determine if we are hovering over a character.
    //  * Could possibly change to be not a raycast but not sure if needed.
    public RaycastHit2D? GetFocusedOnCharacter()
    {
        Vector3 mousPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousPos2D = new Vector2(mousPos.x, mousPos.y);

        RaycastHit2D hit = Physics2D.Raycast(mousPos2D, Vector2.zero, 1, LayerMask.GetMask("Character"));

        if (hit)
        {
            return hit;
        }

        return null;
    }
}
