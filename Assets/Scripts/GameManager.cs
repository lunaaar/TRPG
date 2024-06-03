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
    public bool hasEnemyActed;

    public Level currentLevel;

    public PathFinder pathFinder;

    public GameObject combatUIReference;

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

        pathFinder = new PathFinder();

        listOfAllCharacters = ((Character[])FindObjectsOfType(typeof(Character))).ToList();
        listOfAllFriendly = new List<Character>();
        listOfAllEnemies = new List<Character>();

        int counter = 0;

        foreach (Character c in listOfAllCharacters)
        {
            if (c.alignment == Character.AlignmentStatus.Friendly)
            {
                listOfAllFriendly.Add(c);
                c.uiReference = GuiManager.instance.charaterStatsDisplay[counter];

                c.setUpStatScreen();

              /*var temp = c.uiReference.GetComponentsInChildren<Image>()[1];
                temp.sprite = c.characterImage;
                temp.color = Color.white;

                var arrayOfTexts = c.uiReference.GetComponentsInChildren<TextMeshProUGUI>();

                arrayOfTexts[0].text = c.name;
                arrayOfTexts[1].text = c.characterStats.contains("maxHealth").ToString();
                arrayOfTexts[3].text = c.characterStats.contains("currentHealth").ToString();*/

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
        hasEnemyActed = false;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            BOOST boost = new BOOST();
            boost.performAction(test, test, false);

            TestCrit crit = new TestCrit();
            crit.performAction(test, test, false);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("TEST END OF TURN PROCESS");
            //CursorMovement.instance.selectedCharacter = test;
            processEndOfTurn();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            StartCoroutine(enemyAction());
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
                    StartCoroutine(updateCombatOrder());
                    CursorMovement.instance.characterActionPerformed = false;
                    CursorMovement.instance.characterMovementPerformed = false;
                    count++;
                }
                break;
            case (Character.AlignmentStatus.Enemy):

                if (!hasEnemyActed)
                {
                    Debug.Log(CursorMovement.instance.selectedCharacter.name + " takes their turn");

                    StartCoroutine(enemyAction());
                    hasEnemyActed = true;
                }
                

                //CursorMovement.instance.characterIsSelected = true;

                //Enemy would do some logic here to attack or defend or something.
                //Enemy enemy = (Enemy)CursorMovement.instance.selectedCharacter;

                //Debug.Log("Enemy: " + enemy.name);

                //Enemy.EnemyAction test = enemy.calculateBestMove();

                //Debug.Log(test.ToString());

                //CursorMovement.instance.generatePath(test.tileToMoveTo.gridPosition);

                //test.action.performAction(enemy, test.target, false);

                //updateCombatOrder();

                if (CursorMovement.instance.characterMovementPerformed &&
                    CursorMovement.instance.characterActionPerformed &&
                    !CursorMovement.instance.characterIsMoving)
                {
                    processEndOfTurn();
                    StartCoroutine(updateCombatOrder());
                    CursorMovement.instance.characterActionPerformed = false;
                    CursorMovement.instance.characterMovementPerformed = false;
                    count++;
                }
                //count++;
                break;
            default:
                break;
        }
    }


    public IEnumerator enemyAction()
    {
        yield return new WaitForSeconds(3);

        Enemy enemy = (Enemy)CursorMovement.instance.selectedCharacter;

        Enemy.EnemyAction test = enemy.calculateBestMove();

        CursorMovement.instance.characterIsSelected = true;

        CursorMovement.instance.characterIsMoving = true;

        test.action.uses--;
        test.action.performAction(enemy, test.target, false);

        CursorMovement.instance.generatePath(test.tileToMoveTo.gridPosition);

        CursorMovement.instance.characterActionPerformed = true;

        CursorMovement.instance.characterIsSelected = false;

        updateCombatOrder();
    }

    public void showGUI(Character character, Vector3Int gP)
    {
        combatUIReference.SetActive(true);
        //character.showGUI(gP);
    }

    public void hideGUI(Character character)
    {
        combatUIReference.SetActive(false);
        character.hideGUI();
    }

    private void getCombatOrder()
    {
        combatOrder = new List<Character>(listOfAllCharacters);
        combatOrder = combatOrder.OrderByDescending(ch => ch.characterStats.contains("Speed")).ToList();
        tempText.text = "Current Character: " + combatOrder[0].name;

    }

    private IEnumerator updateCombatOrder()
    {
        Debug.Log("Update Combat Order");

        yield return new WaitForSeconds(1);

        var character = combatOrder[0];
        combatOrder.Remove(combatOrder[0]);
        combatOrder.Add(character);

        tempText.text = "Current Character: " + combatOrder[0].name;
        CursorMovement.instance.selectedCharacter = combatOrder[0];

        if(CursorMovement.instance.selectedCharacter.alignment == Character.AlignmentStatus.Enemy)
        {
            hasEnemyActed = false;
        }
    }

    private void processEndOfTurn()
    {
        Debug.Log("Process end of turn");

        bool didModGetRemoved = false;

        //. Process Modification Stuff
        foreach(Character c in instance.listOfAllCharacters)
        {
            for (int i = 0; i < c.listOfModifications.Count; i++)
            {
                Action.Modification m = c.listOfModifications[i];

                m.duration--;

                if (m.duration == 0)
                {
                    //m.action.undoAction(m.caster, m.target);
                    c.listOfModifications.RemoveAt(i);
                    didModGetRemoved = true;
                }
            }

            //. Increases the duration of all the modifications on the selected creatures.
            if (didModGetRemoved)
            {
                foreach (Stats.Stat s in c.characterStats.baseStats)
                {
                    c.characterStats.SetStats(s.key, CursorMovement.instance.selectedCharacter.characterStats.baseStatContains(s.key));
                }

                for (int j = 0; j <c.listOfModifications.Count; j++)
                {
                    Debug.Log(c.listOfModifications[j].key);
                    Debug.Log(c.listOfModifications[j].action.name);
                    Action.Modification m = c.listOfModifications[j];
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

        currentLevel.processEndOfTurn();
    }

    public Character getCharacterAt(Vector3Int position)
    {
        foreach(Character c in listOfAllCharacters){
            if (c.gridPosition == position) return c;
        }

        return null;
    }

    public string getOtherAlignemnt(string alignment)
    {
        if (alignment == "Friendly") return "Enemy";
        else if (alignment == "Enemy") return "Friendly";

        return "";
    }
}
