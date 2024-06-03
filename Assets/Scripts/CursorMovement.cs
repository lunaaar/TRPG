using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using UnityEngine.UI;

public class CursorMovement : MonoBehaviour
{
    public static CursorMovement instance;

    [SerializeField] private GameObject testGUI; //TODO: rename me.


    [Header("===== Character Info =====")]
    [Tooltip("Speed of the Character when they move")] public float movementSpeed;
    [SerializeField] [Tooltip("Character whose turn it is.")] private Character currentTurnCharacter;
    [SerializeField] [Tooltip("The Character that has been selected / clicked on")] public Character selectedCharacter;
    [Space(10)]

    [Header("Turn Booleans")]
    [Tooltip("Is the player trying to move")]public bool showMovementPath;
    public bool characterIsMoving;
    public bool characterIsSelected;
    
    [SerializeField] public bool characterActionPerformed;
    [SerializeField] public bool characterMovementPerformed;



    [Header("===== Overlay Tile Stuff =====")]
    //TODO: No need for this to be a prefab.
    public GameObject overlayTilePrefab;
    private GameObject overlayTile;
    [SerializeField] private Sprite mapCursor;
    [Space(10)]
    //public GameObject movementPreview;

    //? PathFinder things:
    private List<GridTile> path = new List<GridTile>();
    private List<GridTile> arrowPath = new List<GridTile>();

    [Space(5)]

    [Header("Materials")]
    public Material outline;
    public Material @default;
    public GameObject testDog;

    private void Start()
    {
        if (instance == null) instance = this;

        overlayTile = Instantiate(overlayTilePrefab);
        overlayTile.GetComponent<SpriteRenderer>().color = Color.clear;

        characterIsSelected = false;
        characterIsMoving = false;

        //var filler = tilemap.CellToWorld(new Vector3Int(0, 0));
        //this.transform.position = tilemap.GetCellCenterWorld(new Vector3Int((int)filler.x, (int)filler.y));

        //movementPreview.SetActive(false);
    }

    private void Update()
    {

    }

    // LateUpdate is called once per frame after Update
    void LateUpdate()
    {
        if (characterIsMoving)
        {
            //TODO: change this.
            //movementPreview.SetActive(false);
            //arrowTilemap.ClearAllTiles();
            resetArrowPath();
            goto handle;
        }

        var uiHit = GuiManager.instance.isMouseOverUI();
        Vector3Int tilePos = getHoverTile();

        //. If we are hovering off the tilemap or on the UI, Don't show the map cursor.
        if (tilePos == Vector3Int.back || uiHit)
        {
            GetComponent<SpriteRenderer>().sprite = null;
            goto handle;
        }

        var characterHit = GetFocusedOn("Character");

        //. Gets the center of the current tile in world space.
        var tilePosWorld = MapManager.instance.floorTilemaps[tilePos.z].GetCellCenterWorld(tilePos);

        GetComponent<SpriteRenderer>().sprite = mapCursor;

        //if (path.Count == 0)
        if (path.Count == 0 && characterMovementPerformed)
        {
            characterIsMoving = false;
        }

        resetArrowPath();

        //? Process for displaying the movement path and active spell AOE if applicable.
        if (characterIsSelected && tilePos != selectedCharacter.gridPosition
            && selectedCharacter.alignment == Character.AlignmentStatus.Friendly) //. && character.selectedAction != null
        {
            //Resets the arrow to new tile.
            /*. TODO: need to rework.
            arrowTilemap.ClearAllTiles();
            */
            selectedCharacter.showMovementRange();
            //MapManager.instance.resetAttackTiles();

            if (selectedCharacter.selectedAction != null)
            {
                var action = (Action)selectedCharacter.selectedAction;

                if (!characterActionPerformed && action.actionTargets == Action.ActionTargets.AOE)
                {
                    MapManager.instance.resetAttackTiles();
                }
            }

            if (tilePos.z != 0)
            {
                calculatePath(tilePos);
            }
        }


        if (UserInput.instance.holdSelectInput)
        {
            Debug.Log("HOLD DETECTED");

            Debug.Log("TEST: " + MapManager.instance.map[getHoverTile()].status);

            switch (MapManager.instance.map[getHoverTile()].status)
            {
                case "NotOccupied":
                    Debug.Log("Correct!");
                    break;
                default:
                    Debug.LogWarning("Defaulted on Hold Switch statement. Clicked something we don't handle");
                    break;
            }
            return;
        }

        if (!UserInput.instance.tapSelectInput) goto handle;

        Debug.Log("Clicked on " + tilePos + "\n" + "     Status: " + MapManager.instance.map[tilePos].status);

        //If we click anywhere on the map;

        overlayTile.transform.position = tilePosWorld;
        overlayTile.GetComponent<SpriteRenderer>().sortingOrder = tilePos.z + 1;

        MapManager.instance.resetMovementTiles();
        //TODO: attackRangeTilemap.ClearAllTiles();

        //? We click on a character
        if (characterHit.HasValue)
        {
            characterIsMoving = false;

            currentTurnCharacter = characterHit.Value.transform.gameObject.GetComponent<Character>();

            //If the tilePos isn't the character's position
            if (!currentTurnCharacter.gridPosition.Equals(tilePos))
            {
                Debug.Log("When does this happen?");
                
                generatePath(tilePos);
                goto handle;
            }

            //? We click on an attackTile.

            if (characterIsSelected && selectedCharacter.attackTiles != null && !characterActionPerformed)
            {
                var action = (Action)selectedCharacter.selectedAction;

                var amount = arrowPath.Count - (action.range + 1);

                if (action.uses <= 0)
                {
                    Debug.Log("CANNOT USE MOVE PICK AGAIN");
                    goto handle;
                }

                if (selectedCharacter.attackTiles.Contains(MapManager.instance.map[currentTurnCharacter.gridPosition]))
                {
                    action.uses--;
                    action.performAction(selectedCharacter, currentTurnCharacter, false);

                    Debug.Log(selectedCharacter.characterName + " performed Action: " + action.name + " against " + currentTurnCharacter.characterName);

                    currentTurnCharacter = selectedCharacter;

                    if (amount > 0)
                    {
                        characterIsMoving = true;
                        generatePath(arrowPath[amount].gridPosition);
                    }

                    characterActionPerformed = true;
                    characterMovementPerformed = true;
                    characterIsSelected = false;

                    arrowPath.Clear();
                    MapManager.instance.resetMovementTiles();
                    MapManager.instance.resetAttackTiles();

                    selectedCharacter.hideGUI();

                    goto handle;
                }
            }

            //We click on the same character twice.
            if (characterIsSelected && currentTurnCharacter == selectedCharacter)
            {
                Debug.Log("Deselected: " + selectedCharacter);

                //movementPreview.SetActive(false);

                //movementRangeTilemap.ClearAllTiles();
                MapManager.instance.resetMovementTiles();
                //attackRangeTilemap.ClearAllTiles();
                MapManager.instance.resetAttackTiles();

                currentTurnCharacter.isSelected = false;
                characterIsSelected = false;

                selectedCharacter.selectedAction = null;
                selectedCharacter.hideGUI();

                selectedCharacter = null;
            }

            //We click of a different character.
            if (characterIsSelected && currentTurnCharacter != selectedCharacter)
            {
                Debug.Log("Clicked on a different Character");

                //movementRangeTilemap.ClearAllTiles();
                MapManager.instance.resetMovementTiles();
                //attackRangeTilemap.ClearAllTiles();
                MapManager.instance.resetAttackTiles();

                currentTurnCharacter.isSelected = false;
                currentTurnCharacter = null;
                characterIsSelected = false;
            }

            //We click on the selected character.
            if (!characterIsSelected && currentTurnCharacter == selectedCharacter
                 && selectedCharacter.alignment == Character.AlignmentStatus.Friendly)
            {
                Debug.Log("Clicked on selected Character: " + currentTurnCharacter.name);

                CameraController.instance.camera.transform.position = new Vector3(selectedCharacter.transform.position.x, selectedCharacter.transform.position.y, -10);
                //UserInput.instance.mouse.WarpCursorPosition(new Vector2(selectedCharacter.transform.position.x, selectedCharacter.transform.position.y));
                UserInput.instance.mouse.WarpCursorPosition(CameraController.instance.camera.WorldToScreenPoint(selectedCharacter.transform.position));

                currentTurnCharacter.isSelected = true;
                characterIsSelected = true;

                selectedCharacter.GetComponent<SpriteRenderer>().material = @default;
                selectedCharacter = currentTurnCharacter;
                selectedCharacter.GetComponent<SpriteRenderer>().material = outline;

                //. Is this necessary or can the instance of this be placed with character.showGUI();
                GameManager.instance.showGUI(selectedCharacter, selectedCharacter.gridPosition);

                //if(!characterMovementPerformed)
                //character.showMovementRange();
            }
        }
        //? We click on a tile instead
        else
        {
            //. If the character isn't selected
            if (!characterIsSelected)
            {
                Debug.Log("Basic");
                overlayTile.GetComponent<SpriteRenderer>().color = Color.white;
                selectedCharacter.GetComponent<SpriteRenderer>().material = @default;

                characterIsSelected = false;
                //arrowTilemap.ClearAllTiles();

                goto handle;
            }

            if (!characterMovementPerformed
                && selectedCharacter.movementTiles.Contains(MapManager.instance.map[tilePos])
                && path.Count <= selectedCharacter.movementRange && showMovementPath)
            {
                Debug.Log("MOVE TIME");
                characterIsMoving = true;
                characterMovementPerformed = true;

                //. Is this necessary or can the instance of this be placed with character.showGUI();
                //. We must invoke showGui again to update the referenced position for the buttons.

                GameManager.instance.showGUI(selectedCharacter, tilePos);

                //. arrowTilemap.ClearAllTiles();
                resetArrowPath();
                MapManager.instance.resetMovementTiles();
                MapManager.instance.resetAttackTiles();

                generatePath(arrowPath[Mathf.Max(arrowPath.Count - 1, 0)].gridPosition);

                goto handle;
            }


            if (!characterActionPerformed && selectedCharacter.selectedAction == null)
            {
                Debug.Log("TEST2");

                overlayTile.GetComponent<SpriteRenderer>().color = Color.white;
                selectedCharacter.GetComponent<SpriteRenderer>().material = @default;
                characterIsSelected = false;

                arrowPath.Clear();
                //. arrowTilemap.ClearAllTiles();
                resetArrowPath();

                goto handle;
            }
            /**if (!characterIsSelected || selectedCharacter.selectedAction == null || characterMovementPerformed)
            {
                Debug.Log("TEST");
                
                overlayTile.GetComponent<SpriteRenderer>().color = Color.white;
                selectedCharacter.GetComponent<SpriteRenderer>().material = @default;

                characterIsSelected = false;
                arrowTilemap.ClearAllTiles();

                goto handle;
            }*/

            var action = (Action)selectedCharacter.selectedAction;

            if (!characterActionPerformed && action.actionTargets == Action.ActionTargets.AOE)
            {
                Debug.Log("HUH3");

                characterIsMoving = true;
                characterMovementPerformed = true;
                characterActionPerformed = true;
                characterIsSelected = false;

                action.uses--;
                action.performAction(selectedCharacter, currentTurnCharacter, false);

                Debug.Log(selectedCharacter.characterName + " performed Action: " + "\n" + action.name);

                generatePath(arrowPath[Mathf.Max(arrowPath.Count - 1, 0)].gridPosition);

                arrowPath.Clear();
                //. arrowTilemap.ClearAllTiles();
                resetArrowPath();

                goto handle;

            }

            if (arrowPath.Count > 0)
            {
                generatePath(arrowPath[Mathf.Max(arrowPath.Count - 1, 0)].gridPosition);
            }
        }

        handle:
        if (path.Count > 0)
        {
            handlePath();
        }
    }

    private void showArrowPath()
    {
        /**var color = Color.white;
        color.a = .5f;

        movementPreview.SetActive(true);
        //movementPreview.transform.position = this.transform.position;
        movementPreview.GetComponent<SpriteRenderer>().sprite = selectedCharacter.GetComponent<SpriteRenderer>().sprite;
        movementPreview.GetComponent<SpriteRenderer>().color = color;

        //debugObject.GetComponent<SpriteRenderer>().color = Color.clear;
        //debugObject.active = false;
        */

        //? Currently testing new arrow format.

        var action = (Action)selectedCharacter.selectedAction;

        if (action == null)
        {
            if (selectedCharacter.movementTiles.Contains(arrowPath[arrowPath.Count - 1]))
            {
                foreach (GridTile gt in arrowPath)
                {
                    var z = gt.gridPosition.z;
                    MapManager.instance.floorTilemaps[z].SetTileFlags(gt.gridPosition, TileFlags.None);
                    MapManager.instance.floorTilemaps[z].SetColor(gt.gridPosition, GameManager.instance.arrowColor);
                }

                return;
            }

            return;
        }

        if (action.GetType().ToString() == "AOESpell")
        {
            
            //MapManager.instance.resetAttackTiles();
            //TODO: Arrow stuff
            //selectedCharacter.showMovementRange();

            foreach (GridTile gt in arrowPath)
            {
                var z = gt.gridPosition.z;
                MapManager.instance.floorTilemaps[z].SetTileFlags(gt.gridPosition, TileFlags.None);
                MapManager.instance.floorTilemaps[z].SetColor(gt.gridPosition, GameManager.instance.arrowColor);
            }

            return;
        }
        else if (selectedCharacter.attackTiles.Contains(arrowPath[arrowPath.Count - 1]))
        {
            var pathSum = GameManager.instance.pathFinder.findPath(MapManager.instance.map[selectedCharacter.gridPosition], arrowPath[arrowPath.Count - 1]).Sum(t => t.movementPenalty);


            if (pathSum > selectedCharacter.movementRange && pathSum < selectedCharacter.movementRange + action.range)
            {
                return;
            }
            
            var test = arrowPath.Skip(0).Take(arrowPath.Count - action.range);

            foreach (GridTile gt in test)
            {
                var z = gt.gridPosition.z;
                MapManager.instance.floorTilemaps[z].SetTileFlags(gt.gridPosition, TileFlags.None);
                MapManager.instance.floorTilemaps[z].SetColor(gt.gridPosition, GameManager.instance.arrowColor);
            }
        }
        else
        {
            //. This seems to be called when hovering over a movement tile.
            foreach (GridTile gt in arrowPath)
            {
                var z = gt.gridPosition.z;
                MapManager.instance.floorTilemaps[z].SetTileFlags(gt.gridPosition, TileFlags.None);
                MapManager.instance.floorTilemaps[z].SetColor(gt.gridPosition, GameManager.instance.arrowColor);
            }
        }
    }

    private void handlePath()
    {
        arrowPath.Clear();

        //? If the path is greater than 0, and is within the character's movement radius, move along the path.
        
        //. Used to be character.movementRange;
        if (path.Sum(t => t.movementPenalty) <= selectedCharacter.movementRange)
        {
            moveAlongPath();
        }
        else if (currentTurnCharacter != null && currentTurnCharacter.isSelected)
        {
            path.Clear();
        }

    }

    public void generatePath(Vector3Int tilePos)
    {
        if (characterIsSelected && selectedCharacter.gridPosition.Equals(tilePos))
        //if (characterIsSelected && character.gridPosition.Equals(tilePos))
        {
            characterIsSelected = false;
        }

        if (characterIsSelected)
        {
            overlayTile.GetComponent<SpriteRenderer>().color = Color.white;

            path.Clear();
            path = GameManager.instance.pathFinder.findPath(MapManager.instance.map[selectedCharacter.gridPosition], MapManager.instance.map[tilePos]);
            //path = pathFinder.findPath(MapManager.instance.map[character.gridPosition], MapManager.instance.map[tilePos]);

            selectedCharacter.isSelected = false;
            //character.isSelected = false;
            characterIsSelected = false;
        }

        if (selectedCharacter == null)
        //if (character == null)
        {
            return;
        }

        //? Sets the current tile the player moves to as Friendly.

        //path.Count <= character.movementRange
        if (path.Sum(t => t.movementPenalty) <= selectedCharacter.movementRange && path.Count > 0 && characterIsMoving)
        {
            if (GameManager.instance.currentLevel.levelType == Level.LevelType.CapturePoint)
            {
                var level = (Level_Capture_Point)GameManager.instance.currentLevel;
                foreach (CapturePoint c in level.capturePoints)
                {
                    if (c.objectivePositions.Contains(MapManager.instance.map[currentTurnCharacter.gridPosition]))
                    {
                        MapManager.instance.updateTileStatus(currentTurnCharacter.gridPosition, "Objective");
                        break;
                    }
                    else
                    {
                        MapManager.instance.updateTileStatus(currentTurnCharacter.gridPosition, "NotOccupied");
                    }
                }
            }

            MapManager.instance.updateTileStatus(selectedCharacter.gridPosition, "NotOccupied");
            MapManager.instance.updateTileStatus(tilePos, selectedCharacter.alignment.ToString());
            //MapManager.instance.updateTileStatus(tilePos, "Friendly");
        }
    }


    private List<GridTile> generateArrowPath(Vector3Int tilePos)
    {
        if (characterIsSelected && !currentTurnCharacter.gridPosition.Equals(tilePos))
        {
            overlayTile.GetComponent<SpriteRenderer>().color = Color.white;

            arrowPath.Clear();
            arrowPath = GameManager.instance.pathFinder.findArrowPath(MapManager.instance.map[selectedCharacter.gridPosition], MapManager.instance.map[tilePos]);

            //arrowPath = pathFinder.findPath(MapManager.instance.map[selectedCharacter.gridPosition], MapManager.instance.map[tilePos]);
        }

        return arrowPath;
    }

    private void resetArrowPath()
    {
        foreach (GridTile gt in arrowPath)
        {
            var z = gt.gridPosition.z;

            if (selectedCharacter.movementTiles.Contains(gt))
            {
                //MapManager.instance.floorTilemaps[z].SetTileFlags(gt.gridPosition, TileFlags.None);
                MapManager.instance.floorTilemaps[z].SetColor(gt.gridPosition, GameManager.instance.movementEmptyColor);
            }
            else if (selectedCharacter.selectedAction != null)
            {
                /**if (selectedCharacter.attackTiles.Contains(gt))
                {
                    //MapManager.instance.floorTilemaps[z].SetTileFlags(gt.gridPosition, TileFlags.None);
                    MapManager.instance.floorTilemaps[z].SetColor(gt.gridPosition, GameManager.instance.attackEmptyColor);
                }*/
            }
            else
            {
                //MapManager.instance.floorTilemaps[z].SetTileFlags(gt.gridPosition, TileFlags.None);
                MapManager.instance.floorTilemaps[z].SetColor(gt.gridPosition, Color.white);
            }
        }
    }

    private void calculatePath(Vector3Int tilePos)
    {
        var action = (Action)selectedCharacter.selectedAction;

        //. If we have not done an action and the action has been selected.
        if (!characterActionPerformed && action != null)
        {
            if (action.GetType().ToString() == "AOESpell")
            {
                var aoe = (AOESpell)action;

                selectedCharacter.attackTiles = aoe.showActionRange(selectedCharacter.movementTiles, MapManager.instance.map[tilePos], selectedCharacter.movementRange, selectedCharacter.alignment.ToString(), false);

                var path = GameManager.instance.pathFinder.findTruePath(MapManager.instance.map[selectedCharacter.gridPosition], MapManager.instance.map[tilePos]);

                if (path.Count <= selectedCharacter.movementRange + action.range && path.Count > 0)
                {
                    generateArrowPath(tilePos);
                    arrowPath = arrowPath.GetRange(0, Mathf.Max(path.Count - action.range, 0));

                    if (!characterMovementPerformed && showMovementPath) showArrowPath();
                    return;
                }
            }
            else if (selectedCharacter.attackTiles.Contains(MapManager.instance.map[tilePos]) && showMovementPath)
            {
                generateArrowPath(tilePos);
                showArrowPath();
                return;
            }
        }

        if (!characterMovementPerformed)
        {
            if (action != null && action.GetType().ToString() == "AOESpell")
            {
                generateArrowPath(selectedCharacter.gridPosition);
            }
            else if (selectedCharacter.movementTiles.Contains(MapManager.instance.map[tilePos]) && showMovementPath)
            {
                generateArrowPath(tilePos);
                showArrowPath();
            }
        }
    }

    private void moveAlongPath()
    {
        MapManager.instance.resetMovementTiles();
        MapManager.instance.resetAttackTiles();

        characterIsMoving = true;

        var step = movementSpeed * Time.deltaTime;

        //Tile temp;
        //temp.transform.GetPosition();

        // var temp = tilemap.GetCellCenterWorld(path[0].gridPosition);
        var temp = path[0].gridPosition;

        temp.z += 1;

        //? used to be tilemap
        //. used to be character;

        selectedCharacter.transform.position = Vector3.MoveTowards(selectedCharacter.transform.position, MapManager.instance.floorTilemaps[temp.z].GetCellCenterWorld(temp), step);

        //? used to be tilemap
        //. used to be character;

        if (Vector2.Distance(selectedCharacter.transform.position, MapManager.instance.floorTilemaps[temp.z].GetCellCenterWorld(temp)) < 0.0001f)
        {
            selectedCharacter.updateGridPos(temp);
            path.RemoveAt(0);
        }

        //? testing something.
        if(path.Count == 0)
        {
            Debug.Log("Is this happening first?");
            characterMovementPerformed = true;
            characterIsMoving = false;
        }


        if (path.Count == 0 && characterMovementPerformed
            && !characterActionPerformed)
        {
            Debug.Log("END MOVEMENT");
            MapManager.instance.resetMovementTiles();
            //MapManager.instance.resetAttackTiles();

            if(selectedCharacter.alignment.ToString() == "Friendly")
            {
                selectedCharacter.showMovementRange();
            }
        }

    }

    //Retuns the current tile the cursor is hovering over as a Vector3Int position on the tilemap.
    public Vector3Int getHoverTile()
    {
        //BoundsInt bounds = tilemap.cellBounds;
        //BoundsInt bounds = MapManager.instance.floorTilemaps[0].cellBounds;

        var mousPos = Camera.main.ScreenToWorldPoint(UserInput.instance.moveInput);

        for (int i = MapManager.instance.floorTilemaps.Length - 1; i >= 0; i--)
        {
            var mousPos2D = new Vector3(mousPos.x, mousPos.y, i);

            Vector3Int tilePos = MapManager.instance.floorTilemaps[i].WorldToCell(mousPos2D);

            //? Tile tile = tilemap.GetTile<Tile>(tilePos);
            Tile tile = MapManager.instance.floorTilemaps[i].GetTile<Tile>(tilePos);

            var test = new Vector3Int(tilePos.x, tilePos.y, tilePos.z + 1);

            if (i + 1 < MapManager.instance.floorTilemaps.Length)
            {
                if (MapManager.instance.floorTilemaps[i + 1].GetTile<Tile>(test) != null)
                {
                    continue;
                }
            }

            //? used to be tilemap. tilemap.GetCellCenterWorld(tilepos);
            transform.position = MapManager.instance.floorTilemaps[i].GetCellCenterWorld(tilePos);
            GetComponent<SpriteRenderer>().sortingOrder = i + 1;

            if (tile != null)
            {
                return tilePos;
            }
        }

        return Vector3Int.back;
    }

    //Raycast done from Mouse position to determine if we are hovering over a character.
    //  * Could possibly change to be not a raycast but not sure if needed.
    public RaycastHit2D? GetFocusedOn(string layer)
    {
        Vector3 mousPos = Camera.main.ScreenToWorldPoint(UserInput.instance.moveInput);
        Vector2 mousPos2D = new Vector2(mousPos.x, mousPos.y);


        //? Vector3Int tilePos = tilemap.WorldToCell(mousPos2D);
        //Vector3Int tilePos = MapManager.instance.floorTilemaps[0].WorldToCell(mousPos2D);
        //? RaycastHit2D hit = Physics2D.Raycast(tilemap.GetCellCenterWorld(tilePos), Vector2.zero, 1, LayerMask.GetMask("Character"));
        RaycastHit2D hit = Physics2D.Raycast(mousPos2D, Vector2.zero, 1, LayerMask.GetMask(layer));

        if (hit)
        {
            return hit;
        }

        return null;
    }
}
