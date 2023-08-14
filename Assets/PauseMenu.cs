using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;
    public static bool gameIsPaused = false;

    public GameObject pauseMenu;
    public GameObject firstButton;
    public List<GameObject> pages;
    [SerializeField] private static int currentPage;


    private void Awake()
    {
        if (instance == null) instance = this;
        gameIsPaused = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentPage = 0;
    }

    public void Resume()
    {

        Debug.Log("Resume");
        pages[currentPage].SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
        pauseMenu.SetActive(false);
    }

    public void Pause()
    {
        Debug.Log("Pause");
        pauseMenu.SetActive(true);
        currentPage = 0;
        pages[currentPage].SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButton);

        gameIsPaused = true;
        Time.timeScale = 0f;

        //StartCoroutine(CoverWaitFunction());
    }

    // Arrow Button Functions
    public void previousPage()
    {
        pages[currentPage].SetActive(false);
        currentPage = Mathf.Max(currentPage -= 1, 0);
        pages[currentPage].SetActive(true);
    }

    public void nextPage()
    {
        pages[currentPage].SetActive(false);
        currentPage = Mathf.Min(currentPage += 1, pages.Count-1);
        pages[currentPage].SetActive(true);
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