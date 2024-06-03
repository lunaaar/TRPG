using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : MonoBehaviour
{
    public static UserInput instance;
    public Vector2 baseSens; //? This doesn't change.
    [Range(0, 1)] public float sensitivity;
    public Mouse mouse;

    public Vector2 moveInput { get; private set; }
    [SerializeField] private Vector2 moveDirection;
    private Vector2 overflow;

    public bool tapSelectInput { get; private set; }
    public bool holdSelectInput { get; private set; }
    public bool menuInput { get; private set; }
    private PlayerInput playerInput;
    private InputAction tapSelectAction;
    private InputAction holdSelectAction;

    [Space(7)]

    [Header("===== Audio Clips =====")]

    //TODO: I should honestly really move these to the other code in the pause menu script.

    [SerializeField] private AudioSource bookOpen;
    [SerializeField] private AudioSource bookClose;
    [SerializeField] private AudioSource changePageSound;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (instance == null) instance = this;

        playerInput = GetComponent<PlayerInput>();

        setupInputActions();

        mouse = Mouse.current;
    }

    private void OnDisable()
    {
        tapSelectAction.Disable();
        holdSelectAction.Disable();
    }
    private void setupInputActions()
    {
        tapSelectAction = playerInput.actions["TapSelect"];
        tapSelectAction.Enable();

        holdSelectAction = playerInput.actions["HoldSelect"];
        holdSelectAction.Enable();

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
            //moveInput += move * sensitivity;

            if (move.magnitude < 0.1f) return;

            var mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            var warpPosition = mousePosition + overflow + baseSens * Time.deltaTime * move * sensitivity;

            warpPosition = new Vector2(Mathf.Clamp(warpPosition.x, 0, Screen.width), Mathf.Clamp(warpPosition.y, 0, Screen.height));

            overflow = new Vector2(warpPosition.x % 1, warpPosition.y % 1);

            //Pointer.current.position = warpPosition;
            //Mouse.current.WarpCursorPosition(warpPosition);
            moveInput = warpPosition;
        }
    }

    private void UpdateGameplayInputs()
    {
        holdSelectInput = holdSelectAction.triggered;

        //tapSelectInput = tapSelectAction.WasPressedThisFrame();
        tapSelectInput = tapSelectAction.triggered;

        Debug.Log(tapSelectInput);

    }

    public void OpenUI(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (GuiManager.gameIsPaused)
            {
                playerInput.SwitchCurrentActionMap("Gameplay");
                bookClose.Play();
                GuiManager.instance.Resume();
            }
            else
            {
                playerInput.SwitchCurrentActionMap("MenuNav");
                bookOpen.Play();
                GuiManager.instance.Pause();
            }
        }
    }

    //======= Next Page Things =======

    IEnumerator nextPage()
    {
        changePageSound.Play();
        GuiManager.instance.nextPage();
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
        GuiManager.instance.previousPage();
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
