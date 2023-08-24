using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public List<Character> combatOrder;

    public List<Character> listOfAllCharacters;
    public List<Character> listOfAllFriendly;
    public List<Character> listOfAllEnemies;

    public TextMeshProUGUI tempText;


    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (instance == null) instance = this;
    }

    private void Start()
    {
        //Cursor.visible = false;
        listOfAllCharacters = ((Character[])FindObjectsOfType(typeof(Character))).ToList();
        listOfAllFriendly = new List<Character>();
        listOfAllEnemies = new List<Character>();

        foreach(Character c in listOfAllCharacters)
        {
            if (c.alignment == Character.AlignmentStatus.Friendly) listOfAllFriendly.Add(c);
            else if (c.alignment == Character.AlignmentStatus.Enemy) listOfAllEnemies.Add(c);
        }

        getCombatOrder();
    }


    private void FixedUpdate()
    {
        //Core Gameloop time.
        //CursorMovement.instance.selectedCharacter = combatOrder[0];


    }

    public void showGUI(Character character)
    {
        Debug.Log("Show Gui");
    }

    private void getCombatOrder()
    {
        combatOrder = new List<Character>(FindObjectsOfType<Character>());
        combatOrder = combatOrder.OrderByDescending(ch => ch.characterStats.contains("Speed")).ToList();
         
    }

    public void updateCombatOrder()
    {
        var character = combatOrder[0];
        combatOrder.Remove(combatOrder[0]);
        combatOrder.Add(character);

        tempText.text = "Current Character: " + combatOrder[0].name;
    }
}
