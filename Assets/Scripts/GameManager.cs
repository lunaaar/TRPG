using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public List<Character> combatOrder;


    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        //Cursor.visible = false;
        getCombatOrder();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            updateCombatOrder();
        }
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
    }
}
