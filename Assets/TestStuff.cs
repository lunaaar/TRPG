using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class TestStuff : MonoBehaviour
{
    public int testRange = 3;
    public int attackRange = 1;

    public MapManager mapManager;

    public GameObject overlayTilePrefab;
    public GameObject overlayTileContainer;
    public GameObject testContainer;
    public Color highlightColor;
    public Color attackColor;

    [Header("====== Grid Info ======")]
    [Tooltip("Position of the player on the grid")] public Vector3Int gridPos;
    [Tooltip("Reference to the grid")] public GameObject grid;
    private Tilemap t;
    private PathFinder pathFinder;

    // Start is called before the first frame update
    void Awake()
    {
        pathFinder = new PathFinder();

        t = grid.GetComponentInChildren<Tilemap>();

        //Alligns internal grid position with where it actually is;
        gridPos = t.WorldToCell(this.transform.position);
        this.transform.position = t.GetCellCenterWorld(gridPos);
    }

    private void Start()
    {
        
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3Int test = new Vector3Int(-4, -6, 0);
            doTestMovement(test);
        }
    }

    public void doTestMovement(Vector3Int destination)
    {
        Debug.Log(pathFinder.findPath(mapManager.map[gridPos], mapManager.map[destination]).Sum(tile => tile.movementPenalty));
    }
}
