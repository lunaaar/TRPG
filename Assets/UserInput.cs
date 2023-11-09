using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : MonoBehaviour
{
    public static UserInput instance;
    [Range(0, 2)] public float sensitivity;
    public Vector2 moveInput { get; private set; }
    [SerializeField] private Vector2 moveDirection;

    public bool selectInput { get; private set; }
    public bool menuInput { get; private set; }
    private PlayerInput playerInput;
    private InputAction selectAction;

    [Space(7)]

    [Header("===== Audio Clips =====")]

    [SerializeField] private AudioSource bookOpen;
    [SerializeField] private AudioSource bookClose;
    [SerializeField] private AudioSource changePageSound;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (instance == null) instance = this;

        playerInput = GetComponent<PlayerInput>();

        setupInputActions();
    }
    private void setupInputActions()
    {
        selectAction = playerInput.actions["Select"];
        selectAction.Enable();

        moveInput = new Vector2(590, 274);
    }

    private void Update()
    {
        //Debug.Log(playerInput.currentActionMap);

        if (playerInput.currentActionMap.name == "Gameplay")
        {
            UpdateGameplayInputs();
            Move(moveDirection);
        }
    }

    //Context function called through event system to update inputs.
    public void OnMove(InputAction.CallbackContext context)
    {
        moveDirection = playerInput.actions["LookAround"].ReadValue<Vector2>();
    }

    //Function called every frame to actually adjust character based on inputs.
    private void Move(Vector2 move)
    {
        if (playerInput.currentControlScheme == "Mouse")
        {
            moveInput = move;
        }
        else if (playerInput.currentControlScheme == "Gamepad" || playerInput.currentControlScheme == "Keyboard")
        {
            moveInput += move * sensitivity;
        }
    }

    private void UpdateGameplayInputs()
    {
        selectInput = selectAction.WasPressedThisFrame();
    }

    public void OpenUI(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("TEST2");

            if (PauseMenu.gameIsPaused)
            {
                playerInput.SwitchCurrentActionMap("Gameplay");
                bookClose.Play();
                PauseMenu.instance.Resume();
            }
            else
            {
                playerInput.SwitchCurrentActionMap("MenuNav");
                bookOpen.Play();
                PauseMenu.instance.Pause();
            }
        }
    }

    //======= Next Page Things =======

    IEnumerator nextPage()
    {
        changePageSound.Play();
        PauseMenu.instance.nextPage();
        yield return null;
    }

    public void nextPageAction(InputAction.CallbackContext context)
    {
        Debug.Log("Next");
        if (context.performed)
        {
            StartCoroutine(nextPage());
            StopCoroutine(nextPage());
        }
        
    }

    public void nextPageButton()
    {
        StartCoroutine(nextPage());
        StopCoroutine(nextPage());
    }


    //======= Previous Page Things =======

    IEnumerator previousPage()
    {
        changePageSound.Play();
        PauseMenu.instance.previousPage();
        yield return null;
    }

    public void previousPageAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            StartCoroutine(previousPage());
            StopCoroutine(previousPage());
        }
    }

    public void previousPageButton()
    {
        StartCoroutine(previousPage());
        StopCoroutine(previousPage());
    }

    void RemapButtonClicked(InputAction actionToRebind)
    {
        var rebindOperation = actionToRebind.PerformInteractiveRebinding()
                    // To avoid accidental input from mouse motion
                    .WithControlsExcluding("Mouse")
                    .OnMatchWaitForAnother(0.1f)
                    .Start();
    }

}
