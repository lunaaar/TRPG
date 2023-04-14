using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DetectTileTest : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile green;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log(position);
            Debug.Log(Vector3Int.FloorToInt(position));
            tilemap.SetTile(Vector3Int.FloorToInt(position), green);
        }
    }
}
