using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


//TODO: Rename to GUI or GUI_Menu?
public class GuiManager : MonoBehaviour
{
    public static GuiManager instance;
    public static bool gameIsPaused = false;

    public GameObject damageNumberPrefab;

    [Space(5)]
    [Header("Page Info")]
    public GameObject pauseMenu;
    public GameObject firstButton;
    public List<GameObject> pages;
    [SerializeField] private static int currentPage;

    [Space(5)]
    [Header("Page Specific Stuff")]
    public List<GameObject> charaterStatsDisplay;

    [Space(5)]
    [Header("References")]
    [SerializeField] private GameObject guiReference;
    [SerializeField] private GameObject leftButton;
    [SerializeField] private GameObject rightButton;

    [Space(5)]
    [Header("Action UI")]
    [SerializeField] private GameObject actionUI;

    private void Awake()
    {
        if (instance == null) instance = this;
        gameIsPaused = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentPage = 0;
        actionUI.SetActive(false);
    }

    public void Resume()
    {

        Debug.Log("Resume");

        //Cursor.visible = false;
        pages[currentPage].SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
        guiReference.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void Pause()
    {
        Debug.Log("Pause");
        
        //Cursor.visible = true;
        guiReference.SetActive(false);
        pauseMenu.SetActive(true);
        currentPage = 0;

        leftButton.SetActive(false);

        pages[currentPage].SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButton);

        gameIsPaused = true;
        Time.timeScale = 0f;
    }

    // Arrow Button Functions
    public void previousPage()
    {
        leftButton.SetActive(true);
        rightButton.SetActive(true);

        pages[currentPage].SetActive(false);
        currentPage = Mathf.Max(currentPage -= 1, 0);

        if (currentPage == 0)
        {
            leftButton.SetActive(false);
        }

        pages[currentPage].SetActive(true);
    }

    public void nextPage()
    {
        leftButton.SetActive(true);
        rightButton.SetActive(true);

        pages[currentPage].SetActive(false);
        currentPage = Mathf.Min(currentPage += 1, pages.Count-1);

        if(currentPage == pages.Count - 1)
        {
            rightButton.SetActive(false);
        }

        pages[currentPage].SetActive(true);
    }


    public bool isMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    /**
     * Buttons
     * 
     */

    public void moveButton()
    {
        hideAttackUI();
        CursorMovement.instance.showMovementPath = true;
        //CursorMovement.instance.selectedCharacter.showMovementRange();
    }

    public void actionButton()
    {
        CursorMovement.instance.selectedCharacter.setupActionButtons(CursorMovement.instance.selectedCharacter.gridPosition);
        CursorMovement.instance.showMovementPath = true;
        actionUI.SetActive(true);
    }

    public void noneButton()
    {
        hideAttackUI();
        MapManager.instance.resetMovementTiles();
        MapManager.instance.resetAttackTiles();
        CursorMovement.instance.showMovementPath = false;
    }

    public void hideAttackUI()
    {
        actionUI.SetActive(false);
    }

    public void LoadMenu()
    {
        Debug.Log("Loading Menu...");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}
