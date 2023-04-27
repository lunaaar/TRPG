using UnityEngine;
using UnityEngine.Tilemaps;

public class Character : MonoBehaviour
{
    [Header("====== Character Info ======")]
    [Tooltip("Character Name")] public string characterName;
    [Range(1, 6)] [Tooltip("How many tiles can this character move?")] public int movementRange = 3;
    [Tooltip("Is the Character actively selected?")] public bool isSelected;
    [Tooltip("Int variable for if the character is friendly(0), neutral(1), or hostile(2).")] public int friendOrEnemy;
    [Tooltip("Color of the range highlight")] public Color highlightColor;
    [Space(10)]

    [Header("====== Grid Info ======")]
    [Tooltip("Position of the player on the grid")] public Vector3Int gridPos;
    [Tooltip("Reference to the grid")] public GameObject grid;
    private Tilemap t;
    public MapManager mapManager;


    private void Awake()
    {
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
