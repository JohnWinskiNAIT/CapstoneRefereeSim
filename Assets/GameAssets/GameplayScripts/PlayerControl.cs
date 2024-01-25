using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    [SerializeField]
    InputActionAsset inputActions;
    private Rigidbody rb;

    private InputAction moveAction, lookAction, callAction, whistleAction, pauseAction;
    private Vector2 moveInput, lookInput;

    // Makes sure to get all actions on Awake as opposed to start, otherwise OnEnable goes first.
    void Awake()
    {
        moveAction = inputActions.FindActionMap("Gameplay").FindAction("Move");
        lookAction = inputActions.FindActionMap("Gameplay").FindAction("Look");
        callAction = inputActions.FindActionMap("Gameplay").FindAction("Call");
        whistleAction = inputActions.FindActionMap("Gameplay").FindAction("Whistle");
        pauseAction = inputActions.FindActionMap("Gameplay").FindAction("Pause");
    }

    // Update is called once per frame
    void Update()
    {
        // Getting Vector2 inputs for move and look.
        moveInput = moveAction.ReadValue<Vector2>();
        lookInput = lookAction.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        
    }


}
