using UnityEngine;
using UnityEngine.Tilemaps;

public class Character : MonoBehaviour
{
    [Header("====== Character Info ======")]
    [Tooltip("Character Name")] public string characterName;
    [Range(1, 6)] [Tooltip("How many tiles can this character move?")] public int movementRange = 3;
    [Tooltip("Is the Character actively selected?")] public bool isSelected;
    [Tooltip("Int variable for if the character is friendly(0), neutral(1), or hostile(2).")] public int friendOrEnemy;
    [Space(10)]

    [Header("====== Grid Info ======")]
    [Tooltip("Position of the player on the grid")] public Vector3Int gridPos;
    [Tooltip("Reference to the grid")] public GameObject grid;
    private Tilemap t;
    public MapManager mapManager;

    //public UnityEvent unityEvent = new UnityEvent();

    private void Start()
    {
        // I am tired and its midnight, this has something to do with tile anchors. I think we just need to move them down so they are all lined up properly.
        // Idk, i have a tab open right now of it.
        
        
        t = grid.GetComponentInChildren<Tilemap>();

        //Alligns internal grid position with where it actually is;
        gridPos = t.WorldToCell(this.transform.position);
        this.transform.position = t.GetCellCenterWorld(gridPos);
    }

    private void Update()
    {
        
    }

    public void updateGridPos(Vector3Int v)
    {
        gridPos = v;
        this.transform.position = t.GetCellCenterWorld(gridPos);
    }

    public void updateGridPos()
    {
        gridPos = grid.GetComponentInChildren<Tilemap>().WorldToCell(this.transform.position);
        this.transform.position = t.GetCellCenterWorld(gridPos);
    }

}
