using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile
{
    public int G, H;
    public int F { get { return G + H; } }

    public bool isBlocked;

    public GridTile previous;

    public Vector3Int gridPosition;
    
    public GridTile(Vector3Int gP)
    {
        gridPosition = gP;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
