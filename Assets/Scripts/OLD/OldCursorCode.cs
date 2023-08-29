using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldCursorCode
{
    /*
     * This is sitting here as refernce for old legacy code that has since
     * been changed.
     * 
     * User to be the CursorMovement LaterUpdate code
     */

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
