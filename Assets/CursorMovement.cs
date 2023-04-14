using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CursorMovement : MonoBehaviour
{
    public Tilemap tilemap;


    // LateUpdate is called once per frame after Update
    void LateUpdate()
    {
        testTileSelector();
        
        var tileHit = GetFocusedOnTile();
        var characterHit = GetFocusedOnCharacter();

        if (tileHit.HasValue)
        {
            GameObject overlayTile = tileHit.Value.collider.gameObject;
            //this.transform.position = overlayTile.transform.position;
            
            if (Input.GetMouseButton(0))
            {
                //We click on a tile with a character
                if (characterHit.HasValue)
                {
                    GameObject character = characterHit.Value.transform.gameObject;
                    character.GetComponent<Character>().isSelected(overlayTile);
                }
                else
                {
                    overlayTile.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 255);
                }
                
            }
        }
    }


    public void testTileSelector()
    {
        var mousPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var mousPos2D = new Vector2(mousPos.x, mousPos.y);

        Vector3Int tilePos = tilemap.WorldToCell(mousPos2D);
        this.transform.position = tilePos;
        

        Tile tile = tilemap.GetTile<Tile>(tilePos);

        if(tile != null && Input.GetMouseButtonDown(0))
        {
            Debug.Log(tilemap.GetColor(tilePos));
            //tilemap.SetColor(tilePos, Color.red);
        }
        
    }

    public RaycastHit2D? GetFocusedOnTile()
    {
        Vector3 mousPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousPos2D = new Vector2(mousPos.x, mousPos.y);

        RaycastHit2D hit = Physics2D.Raycast(mousPos2D, Vector2.zero, 1, LayerMask.GetMask("Background"));

        if(hit)
        {
            return hit;
        }

        return null;
    }

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
