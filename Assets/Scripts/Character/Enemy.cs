using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Enemy : MonoBehaviour
{
    //FIX A LOT OF THINGS
    
    
    
    
    
    
    
    
    [Header("===== Enemy Stats =====")]
    public string enemyName;
    [Range(1, 6)] [Tooltip("How many tiles can this character move?")] public int movementRange = 3;
    public int currentHealth = 10;
    public int maxHealth = 10;
    public List<Weapon> weapons;

    public enum AlignmentStatus { Friendly, Neutral, Enemy }
    [Tooltip("Alignment Status of the Character, determines how the manager treats this character.")] public AlignmentStatus alignment;

    [Header("====== Grid Info ======")]
    [Tooltip("Position of the player on the grid")] public Vector3Int gridPos;
    [Tooltip("Reference to the grid")] public GameObject grid;
    private Tilemap t;
    private PathFinder pathFinder;

    private void Awake()
    {
        t = grid.GetComponentInChildren<Tilemap>();

        //Alligns internal grid position with where it actually is;
        gridPos = t.WorldToCell(this.transform.position);
        this.transform.position = t.GetCellCenterWorld(gridPos);
    }
}
