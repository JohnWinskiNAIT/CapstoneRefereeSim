using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    [SerializeField]
    InputActionAsset inputActions;
    private Rigidbody rb;

    [SerializeField]
    float maxSpeed, accelerationSpeed, breakingSpeed;
    [SerializeField]
    float cameraSpeed, cameraMaxY, cameraMinY;

    private InputAction moveAction, lookAction, callAction, whistleAction, pauseAction;
    private Vector2 moveInput, lookInput;
    public Vector3 cameraAngle { get; private set; }

    // Makes sure to get all actions on Awake as opposed to start, otherwise OnEnable goes first.
    void Awake()
    {
        moveAction = inputActions.FindActionMap("Gameplay").FindAction("Move");
        lookAction = inputActions.FindActionMap("Gameplay").FindAction("Look");
        callAction = inputActions.FindActionMap("Gameplay").FindAction("Call");
        whistleAction = inputActions.FindActionMap("Gameplay").FindAction("Whistle");
        pauseAction = inputActions.FindActionMap("Gameplay").FindAction("Pause");
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        cameraAngle = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        // Getting Vector2 inputs for move and look.
        moveInput = moveAction.ReadValue<Vector2>();
        lookInput = lookAction.ReadValue<Vector2>();

        //Call pausemanager when pause is pressed.
        if (pauseAction.WasPressedThisFrame())
        {
            PauseManager.instance.PauseGame();
        }
    }

    private void FixedUpdate()
    {
        //Do all movement-related changes in here.
        PlayerMovement();

        //Camera logic might need to be determined here then visuals performed in late update.
    }

    private void LateUpdate()
    {
        // cameraAngle changes based on inputs to be used by the camera script. Capped at 360 and 0 going over and under.
        cameraAngle += new Vector3(-lookInput.y, lookInput.x, 0) * cameraSpeed * Time.deltaTime;

        if (cameraAngle.x > 360)
        {
            cameraAngle = new Vector3(cameraAngle.x - 360, cameraAngle.y);
        }
        if (cameraAngle.x < 0)
        {
            cameraAngle = new Vector3(cameraAngle.x + 360, cameraAngle.y);
        }
    }

    private void PlayerMovement()
    {
        //Apply force.
        rb.AddRelativeForce(new Vector3(moveInput.x, 0, moveInput.y) * accelerationSpeed * Time.fixedDeltaTime, ForceMode.Force);

        //Apply cap if greater than max speed. (Parabolic acceleration curve for later?)
        Vector2 capTest = new Vector2(rb.velocity.x, rb.velocity.z);
        if (capTest.magnitude > maxSpeed)
        {
            rb.velocity = new Vector3(rb.velocity.normalized.x * maxSpeed, rb.velocity.y, rb.velocity.normalized.z * maxSpeed);
        }
    }

    #region Enable and Disable
    private void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
    }
    #endregion
}
