using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance;

    public static MapManager Instance { get { return _instance; } }

    public Dictionary<Vector3Int, GridTile> map;

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        var tileMap = gameObject.GetComponentInChildren<Tilemap>();

        map = new Dictionary<Vector3Int, GridTile>();

        BoundsInt bounds = tileMap.cellBounds;

        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                var location = new Vector3Int(x, y, 0);

                if (tileMap.HasTile(location))
                {
                    GridTile gridTile = new GridTile(location);

                    map.Add(location, gridTile);
                }
            }
        }
    }
}
