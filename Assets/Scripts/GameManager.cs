using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public List<Character> combatOrder;
    private int count;

    public List<Character> listOfAllCharacters;
    public List<Character> listOfAllFriendly;
    public List<Character> listOfAllEnemies;

    public TextMeshProUGUI tempText;

    public Sprite testSprite;

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

        int counter = 0;

        foreach(Character c in listOfAllCharacters)
        {
            if (c.alignment == Character.AlignmentStatus.Friendly)
            {
                listOfAllFriendly.Add(c);
                c.uiReference = PauseMenu.instance.charaterStatsDisplay[counter];

                var temp = c.uiReference.GetComponentsInChildren<Image>()[1];
                temp.sprite = c.characterImage;
                temp.color = Color.white;

                var arrayOfTexts = c.uiReference.GetComponentsInChildren<TextMeshProUGUI>();

                arrayOfTexts[0].text = c.name;
                arrayOfTexts[1].text = c.characterStats.contains("maxHealth").ToString();
                arrayOfTexts[3].text = c.characterStats.contains("currentHealth").ToString();

                counter++;
            }
            else if (c.alignment == Character.AlignmentStatus.Enemy) listOfAllEnemies.Add(c);
        }

        getCombatOrder();
        count = 0;
    }


    private void FixedUpdate()
    {
        //Core Gameloop time.

        CursorMovement.instance.selectedCharacter = combatOrder[0];

        if(count == listOfAllCharacters.Count)
        {
            //Process End of Turn effects (Capture points, etc);
            Debug.Log("TEST END OF TURN");
            //Likely increase a turn increment here;
            count = 0;
        }

        switch (CursorMovement.instance.selectedCharacter.alignment)
        {
            case (Character.AlignmentStatus.Friendly):
                //If the player picked a character and moved them to a tile.
                if (CursorMovement.instance.characterActionPerformed && CursorMovement.instance.characterMovementPerformed)
                {
                    Debug.Log(CursorMovement.instance.selectedCharacter.name);
                    updateCombatOrder();
                    CursorMovement.instance.characterActionPerformed = false;
                    count++;
                }
                break;
            case (Character.AlignmentStatus.Enemy):

                Debug.Log(CursorMovement.instance.selectedCharacter.name + " takes their turn");
                //Enemy would do some logic here to attack or defend or something.
                updateCombatOrder();
                count++;
                break;
            default:
                break;
        }
    }

    public void showGUI(Character character, Vector3Int gP)
    {
        character.showGUI(gP);
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
