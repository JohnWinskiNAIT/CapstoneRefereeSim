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
    private Camera mainCamera;

    [SerializeField]
    float maxSpeed, accelerationSpeed, breakingSpeed;
    [SerializeField]
    float cameraSpeed, cameraMaxY, cameraMinY;

    private InputAction moveAction, lookAction, callAction, whistleAction, pauseAction;
    private Vector2 moveInput, lookInput;
    private Vector3 cameraAngle;

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
        cameraAngle = mainCamera.transform.rotation.eulerAngles;
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

    private void PlayerMovement()
    {
        //Apply force.
        rb.AddRelativeForce(moveInput * accelerationSpeed * Time.fixedDeltaTime, ForceMode.Force);

        //Apply cap if greater than max speed. (Parabolic acceleration curve for later?)
        Vector2 capTest = new Vector2(rb.velocity.x, rb.velocity.z);
        if (capTest.magnitude > maxSpeed)
        {
            rb.velocity = new Vector3(rb.velocity.normalized.x * maxSpeed, rb.velocity.y, rb.velocity.normalized.z);
        }
    }

    private void LateUpdate()
    {
        //Do all camera-related changes in here.

        //Apply input to the internal value of where the camera should be looking.
        cameraAngle += new Vector3(lookInput.y, lookInput.x, 0) * cameraSpeed;

        //Determine new values to rotate to.
        float xChange = 0;
        float yChange = 0;

        //Perform rotation on physical game object.
        mainCamera.transform.Rotate(mainCamera.transform.right, xChange);
        mainCamera.transform.Rotate(mainCamera.transform.up, yChange);
    }
}
