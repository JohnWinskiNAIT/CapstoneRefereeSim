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
    float maxSpeed, accelerationSpeed, breakingSpeed, storeInputDuration;
    [SerializeField]
    float cameraSpeed, cameraMaxY, cameraMinY;

    private InputAction moveAction, lookAction, callAction, whistleAction, pauseAction;
    private InputAction storedAction;

    private float storeTimestamp;

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
        Cursor.lockState = CursorLockMode.Locked;
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

        //Stored action stuff.
        if (storedAction == null)
        {
            if (whistleAction.WasPressedThisFrame())
            {
                storedAction = whistleAction;
                storeTimestamp = Time.time;
            }
            if (callAction.WasPressedThisFrame())
            {
                storedAction = callAction;
                storeTimestamp = Time.time;
            }
        }
        else
        {
            StoredActionCheck();
        }
    }

    private void StoredActionCheck()
    {
        if (!storedAction.IsPressed())
        {
            storedAction = null;
        }
        if (Time.time > storeTimestamp + storeInputDuration)
        {
            if (storedAction == whistleAction)
            {
                WhistleActivate();
            }
            if (storedAction == callAction)
            {
                CallActivate();
            }
        }
    }

    public float StoredActionStatus()
    {
        float returnValue;
        if (storedAction == null)
        {
            returnValue = 0f;
        }
        else
        {
            returnValue = (Time.time - storeTimestamp) / storeInputDuration;
        }

        return returnValue;
    }

    private void CallActivate()
    {

    }

    private void WhistleActivate()
    {

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

        if (cameraAngle.y > 360)
        {
            cameraAngle = new Vector3(cameraAngle.y, cameraAngle.y - 360);
        }
        if (cameraAngle.y < 0)
        {
            cameraAngle = new Vector3(cameraAngle.x, cameraAngle.y + 360);
        }

        transform.Rotate(Vector3.up, (cameraAngle.y - transform.rotation.eulerAngles.y) * 0.9f);
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
        callAction.Enable();
        whistleAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        callAction.Disable();
        whistleAction.Disable();
    }
    #endregion
}
