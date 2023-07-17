using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    [Header("----- Pause Menu Stuff -----")]
    public GameObject pauseMenuUI;
    public GameObject firstButton;
    public static bool gameIsPaused = false;

    public MapManager mapManager;
    

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            //Show Pause Menu and Pause Game
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        //pauseMenuUI.SetActive(false);
        Debug.Log("Resume");
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    void Pause()
    {
        //pauseMenuUI.SetActive(true);
        Debug.Log("Pause");

        //EventSystem.current.SetSelectedGameObject(null);
        //EventSystem.current.SetSelectedGameObject(firstButton);
        Time.timeScale = 0f;
        gameIsPaused = true;
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

    public void showGUI(Character character)
    {
        Debug.Log("TEST");
    }
}
