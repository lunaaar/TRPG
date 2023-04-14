using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public int movementRadius = 3;
    public string name;
    CursorMovement cursorMovement;

    //public UnityEvent unityEvent = new UnityEvent();

    private void Start()
    {

    }

    private void Update()
    {
        
    }

    public void isSelected(GameObject tileHit)
    {
        Debug.Log(name + " has been selected");

        tileHit.GetComponent<SpriteRenderer>().color = Color.blue;

        //TODO:
        //Highlight Player

        //Show Movement Squares (if player turn)

        // Show character info on hud
    }


}
