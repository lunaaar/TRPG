using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance;

    public GameObject overlayTilePrefab;

    public GameObject overlayCountainer;


    public static MapManager Instance { get { return _instance; } }
    

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

        BoundsInt bounds = tileMap.cellBounds;

        for(int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                var location = new Vector3Int(x, y, 0);

                if (tileMap.HasTile(location))
                {
                    GameObject overlayTile = Instantiate(overlayTilePrefab, overlayCountainer.transform);
                    var cellWorldPosition = tileMap.GetCellCenterWorld(location);

                    overlayTile.transform.position = new Vector3(cellWorldPosition.x, cellWorldPosition.y, cellWorldPosition.z);
                }
            }
        }
    }

}
