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

    public Level currentLevel;

    [Space(5)]

    [Header("===== Colors =====")]
    public Color movementEmptyColor;
    public Color movementFullColor;

    public Color attackEmptyColor;
    public Color attackFullColor;

    public Color friendlyEmptyColor;
    public Color friendlyFullColor;
    
    public Color arrowColor;

    public Color testColor;


    [Space(5)]

    [Header("===== Lists =====")]

    public List<Character> listOfAllCharacters;
    public List<Character> listOfAllFriendly;
    public List<Character> listOfAllEnemies;

    public TextMeshProUGUI tempText;

    public Sprite testSprite;

    [Space(10)]
    public Character test;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (instance == null) instance = this;

        listOfAllCharacters = ((Character[])FindObjectsOfType(typeof(Character))).ToList();
        listOfAllFriendly = new List<Character>();
        listOfAllEnemies = new List<Character>();

        int counter = 0;

        foreach (Character c in listOfAllCharacters)
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

    private void Start()
    {
        //Cursor.visible = false;

    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {

        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            BOOST boost = new BOOST();
            boost.performAction(test, test);

            TestCrit crit = new TestCrit();
            crit.performAction(test, test);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("TEST END OF TURN PROCESS");
            CursorMovement.instance.selectedCharacter = test;
            processEndOfTurn();
        }
    }

    private void FixedUpdate()
    {
        //Core Gameloop time.
        if(count == 0)
        {
            CursorMovement.instance.selectedCharacter = combatOrder[0];
        }

        if(count == listOfAllCharacters.Count)
        {
            //Process End of Turn effects (Capture points, etc);
            Debug.Log("TEST END OF TURN");
            //Likely increase a turn increment here;
            count = 0;

            //. Calculate any effect that takes place at the end of turn here
            //. EX. Terrain effects, Map Effects, and decrement any timed based buffs.
        }

        switch (CursorMovement.instance.selectedCharacter.alignment)
        {
            case (Character.AlignmentStatus.Friendly):
                
                if (CursorMovement.instance.characterActionPerformed &&
                    CursorMovement.instance.characterMovementPerformed &&
                    !CursorMovement.instance.characterIsMoving)
                {
                    Debug.Log(CursorMovement.instance.selectedCharacter.name + " takes their turn");
                    //Debug.Log(CursorMovement.instance.selectedCharacter.name);
                    processEndOfTurn();
                    updateCombatOrder();
                    CursorMovement.instance.characterActionPerformed = false;
                    CursorMovement.instance.characterMovementPerformed = false;
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

    public void hideGUI(Character character)
    {
        character.hideGUI();
    }

    private void getCombatOrder()
    {
        //combatOrder = new List<Character>(FindObjectsOfType<Character>());
        combatOrder = new List<Character>(listOfAllCharacters);
        combatOrder = combatOrder.OrderByDescending(ch => ch.characterStats.contains("Speed")).ToList();
        tempText.text = "Current Character: " + combatOrder[0].name;

    }

    private void updateCombatOrder()
    {
        var character = combatOrder[0];
        combatOrder.Remove(combatOrder[0]);
        combatOrder.Add(character);

        tempText.text = "Current Character: " + combatOrder[0].name;
        CursorMovement.instance.selectedCharacter = combatOrder[0];
    }

    private void processEndOfTurn()
    {
        bool didModGetRemoved = false;

        //. Increases the duration of all the modifications on the selected creatures.
        for(int i = 0; i < CursorMovement.instance.selectedCharacter.listOfModifications.Count; i++)
        {
            Action.Modification m = CursorMovement.instance.selectedCharacter.listOfModifications[i];

            m.duration--;

            if (m.duration == 0)
            {
                //m.action.undoAction(m.caster, m.target);
                CursorMovement.instance.selectedCharacter.listOfModifications.RemoveAt(i);
                didModGetRemoved = true;
            }
        }

        if (didModGetRemoved)
        {
            foreach(Stats.Stat s in CursorMovement.instance.selectedCharacter.characterStats.baseStats)
            {
                CursorMovement.instance.selectedCharacter.characterStats.SetStats(s.key, CursorMovement.instance.selectedCharacter.characterStats.baseStatContains(s.key));
            }

            for (int j = 0; j < CursorMovement.instance.selectedCharacter.listOfModifications.Count; j++)
            {
                Debug.Log(CursorMovement.instance.selectedCharacter.listOfModifications[j].key);
                Debug.Log(CursorMovement.instance.selectedCharacter.listOfModifications[j].action.name);
                Action.Modification m = CursorMovement.instance.selectedCharacter.listOfModifications[j];
                /*?
                  So the current problem is uncommenting this crashes the game.
                  This is because performAction re-adds the spell to the list of modifications.
                  So I think we might need a new method or something. I am unsure.

                  My thoughts are either some re-apply method that redos modifications. Or maybe performAction gets a
                  boolean as well that is Used for these cases.
                  */
                m.action.reapplyAction(m.caster, m.target);
                //m.action.performAction(m.caster, m.target);
            }
        }
    }

    public Character getCharacterAt(Vector3Int position)
    {
        foreach(Character c in listOfAllCharacters){
            if (c.gridPosition == position) return c;
        }

        return null;
    }
}
