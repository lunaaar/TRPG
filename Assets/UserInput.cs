using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : MonoBehaviour
{
    public static UserInput instance;

    public Vector2 moveInput { get; private set; }
    public bool selectInput { get; private set; }

    private PlayerInput playerInput;

    private InputAction moveAction;
    private InputAction selectAction;
    
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        playerInput = GetComponent<PlayerInput>();

        setupInputActions();
    }

    void Update()
    {
        updateInputs();
    }

    private void setupInputActions()
    {
        moveAction = playerInput.actions["LookAround"];
        selectAction = playerInput.actions["Select"];
    }

    private void updateInputs()
    {
        if(playerInput.currentControlScheme == "Keyboard")
        {
            moveInput = moveAction.ReadValue<Vector2>();
            selectInput = selectAction.WasPressedThisFrame();
        }
        else if(playerInput.currentControlScheme == "Gamepad")
        {
            var vector = moveAction.ReadValue<Vector2>();

            if (Mathf.Abs(vector.x) > 0 && Mathf.Abs(vector.y) == 0)
            {
                vector.y = vector.x * -1;
                Debug.Log("TEST");
            }
            else if (Mathf.Abs(vector.y) > 0)
            {
                vector.x = vector.y;
            }
            Debug.Log(vector);
            moveInput += moveAction.ReadValue<Vector2>();
            selectInput = selectAction.WasPressedThisFrame();
        }
        
    }

    public Vector3Int getHoverTile()
    {
        return Vector3Int.zero;
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
