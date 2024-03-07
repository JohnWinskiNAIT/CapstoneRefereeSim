using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Management;

public class PlayerControl : MonoBehaviour
{
    [SerializeField]
    LazerEmitter emitter;
    Canvas canvas;



    [SerializeField]
    InputActionAsset inputActions;
    [SerializeField]
    Camera cam;
    private Rigidbody rb;

    [SerializeField]
    float maxSpeed, accelerationSpeed, breakingModifier, storeInputDuration;
    [SerializeField]
    float cameraSpeed, cameraMaxY, cameraMinY;

    private InputAction moveAction, lookAction, callAction, pauseAction, whistleAction;
    public InputAction heldAction { get; private set; }

    private float storeTimestamp;

    private Vector2 moveInput, lookInput;
    public Vector3 cameraAngle { get; private set; }
    public Vector3 inputAngle { get; private set; }

    private PlayerUIManager uiManager;
    private PlayerState playerState;
    private Vector3 autoskateDestination, savedVelocity;

    public bool isVREnabled;

    /// <summary>
    /// ////////////////////bool isInMenu; maybe this should e covered in state?
    /// we want to make the Call button Call when playying hockey, but be a Menu selecting pointer every other time
    /// </summary>

    GameObject waypointParent;

    private enum PlayerState
    {
        Control,
        Lockout,
        Autoskate
    }

    // Makes sure to get all actions on Awake as opposed to start, otherwise OnEnable goes first.
    void Awake()
    {
        isVREnabled = GameUtilities.VREnabled();

        if (!isVREnabled)
        {
            Debug.Log("VR UnEnabled");
        }

        /// Depending on what options the player selects
        /// inputActions = 

        moveAction = inputActions.FindActionMap("Gameplay").FindAction("Move");
        lookAction = inputActions.FindActionMap("Gameplay").FindAction("Look");
        callAction = inputActions.FindActionMap("Gameplay").FindAction("Call/Select");
        whistleAction = inputActions.FindActionMap("Gameplay").FindAction("Whistle/Cancel");
        pauseAction = inputActions.FindActionMap("Gameplay").FindAction("Pause");
        playerState = PlayerState.Control;

        GameplayEvents.LoadCutscene.AddListener(LoadWaypoints);
        GameplayEvents.CutsceneTrigger.AddListener(CutsceneListener);
        GameplayEvents.InitializePlay.AddListener(ResetPlayer);
        GameplayEvents.SetPause.AddListener(PausePlayer);
    }

    private void Start()
    {
        uiManager = GameplayManager.Instance.playerUI.GetComponent<PlayerUIManager>();
        uiManager.isVREnabled = isVREnabled;

        rb = GetComponent<Rigidbody>();
        cameraAngle = Vector3.zero;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //Cannot move if selection wheel is open.
        if (playerState == PlayerState.Control)
        {
            // Getting Vector2 inputs for move and look.
            moveInput = moveAction.ReadValue<Vector2>();
            lookInput = lookAction.ReadValue<Vector2>();

            //Call pausemanager when pause is pressed.
            if (pauseAction.WasPressedThisFrame())
            {
                GameplayEvents.SetPause.Invoke(true);
            }

            //Stored action stuff. If nothing is stored, it checks for to store whistle or call. If something is stored,
            //it runs a check to see if it's still being held.
            if (heldAction == null)
            {
                if (whistleAction.WasPressedThisFrame())
                {
                    heldAction = whistleAction;
                    storeTimestamp = Time.time;
                }
                if (callAction.WasPressedThisFrame())
                {
                    heldAction = callAction;
                    storeTimestamp = Time.time;
                }
            }
            else
            {
                StoredActionCheck();
            }

            /*Vector3 test1 = new Vector2(rb.velocity.x, rb.velocity.z);
            Vector3 test2 = new Vector2(playe)
            Debug.Log(Vector3.Angle(Vector3.forward, test2));*/
            //Debug.Log(transform.forward);
        }
        else
        {
            moveInput = Vector2.zero;
            lookInput = Vector2.zero;
            heldAction = null;
        }

        if (playerState == PlayerState.Autoskate)
        {
            AutoskateMovement();
            AutoskateCheck();
        }
    }

    //This checks if the stored action (whistle or call) is still being held. If not, cancel the charge.
    //If true, check if it's been held for long enough and use the appropriate method if so.
    private void StoredActionCheck()
    {
        if (!heldAction.IsPressed())
        {
            heldAction = null;
        }
        if (Time.time > storeTimestamp + storeInputDuration)
        {
            if (heldAction == whistleAction)
            {
                WhistleActivate();
            }
            if (heldAction == callAction)
            {
                CallActivate();
            }
        }
    }

    //Public function for UI to check how long the action has been stored for as a time (t) value.
    public float StoredActionStatus()
    {
        float returnValue;
        if (heldAction == null)
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
        //Unfinished.
        GameplayManager.Instance.SetCallTimer();
        GameplayManager.Instance.CallPrep();
        heldAction = null;
    }

    private void WhistleActivate()
    {
        //Unfinished.
        GameplayManager.Instance.SetCallTimer();
        GameplayEvents.EndPlay.Invoke();
        heldAction = null;
    }

    private void PausePlayer(bool pausing)
    {
        if (pausing)
        {
            playerState = PlayerState.Lockout;
            savedVelocity = rb.velocity;
            rb.velocity = Vector3.zero;
        }
        else
        {
            playerState = PlayerState.Control;
            rb.velocity = savedVelocity;
        }
    }

    private void FixedUpdate()
    {
        if (playerState == PlayerState.Control)
        {
            //Do all movement-related changes in here.
            PlayerMovement();

            //Camera logic might need to be determined here then visuals performed in late update.
        }

        if (playerState == PlayerState.Autoskate)
        {
            //Logic for auto movement goes here
            AutoskateMovement();
        }
    }

    private void LateUpdate()
    {
        if (isVREnabled)
        {

        }
        else
        {
            CameraClamp();

            // This section eliminates a janky and awkward transition between >360 and 0, and vice versa. Still a bit awkward.
            // Smoothing it out may involve checking the values (0.9f below) used for interpolation.
            float rotateValue = (cameraAngle.y - transform.rotation.eulerAngles.y);
            if (rotateValue > 180)
            {
                rotateValue -= 360;
            }
            if (rotateValue < -180)
            {
                rotateValue += 360;
            }
            transform.Rotate(Vector3.up, rotateValue * 0.9f * Time.fixedDeltaTime);
        }
    }

    private void CameraClamp()
    {
        // cameraAngle changes based on inputs to be used by the camera script. Capped at 360 and 0 going over and under.
        cameraAngle += new Vector3(-lookInput.y, lookInput.x, 0) * cameraSpeed * Time.deltaTime;

        //Full rotation logic + clamping the x angle to serialized values since it controls the up/down of the camera.
        if (cameraAngle.x > 360)
        {
            cameraAngle = new Vector3(cameraAngle.x - 360, cameraAngle.y);
        }
        if (cameraAngle.x < 0)
        {
            cameraAngle = new Vector3(cameraAngle.x + 360, cameraAngle.y);
        }
        if (cameraAngle.x < 180 && cameraAngle.x > cameraMinY)
        {
            cameraAngle = new Vector3(cameraMinY, cameraAngle.y);
        }
        if (cameraAngle.x > 180 && cameraAngle.x < cameraMaxY)
        {
            cameraAngle = new Vector3(cameraMaxY, cameraAngle.y);
        }

        if (cameraAngle.y > 360)
        {
            cameraAngle = new Vector3(cameraAngle.x, cameraAngle.y - 360);
        }
        if (cameraAngle.y < 0)
        {
            cameraAngle = new Vector3(cameraAngle.x, cameraAngle.y + 360);
        }
    }

    private void PlayerMovement()
    {
        //Determines the angle between where the player's velocity is going and the player's input.
        inputAngle = new Vector3(moveInput.x, 0, moveInput.y);

        //If VR active, usees camera Y instead of player Y.
        Quaternion angleCheck;
        if (isVREnabled)
        {
            angleCheck = Quaternion.AngleAxis(cam.transform.eulerAngles.y, Vector3.up);
        }
        else
        {
            angleCheck = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up);
        }

        inputAngle = angleCheck * inputAngle;

        //Add an object over a GameObject friend parameter to have it display where the player is moving.
        /*if (friend != null)
        {
            friend.transform.position = transform.position + (horizontalCheck * 2);
        }*/

        //Add relative force.
        rb.AddForce(inputAngle * accelerationSpeed * Time.fixedDeltaTime, ForceMode.Force);

        //Apply cap if greater than max speed. (Parabolic acceleration curve for later?)
        Vector2 capTest = new Vector2(rb.velocity.x, rb.velocity.z);
        if (capTest.magnitude > maxSpeed || Vector2.Angle(new Vector2(inputAngle.x, inputAngle.z), new Vector2(rb.velocity.x, rb.velocity.z)) > 90)
        {
            rb.velocity = new Vector3(rb.velocity.x * breakingModifier, rb.velocity.y, rb.velocity.z * breakingModifier);
        }
    }

    //Add force towards the autoskate destination and avoid exceeding speed limit.
    private void AutoskateMovement()
    {
        Vector3 direction = autoskateDestination - transform.position;

        if ((autoskateDestination - transform.position).magnitude > 2f)
        {
            rb.AddForce(direction.normalized * accelerationSpeed * Time.fixedDeltaTime, ForceMode.Force);
        }


        Vector2 capTest = new Vector2(rb.velocity.x, rb.velocity.z);
        if (capTest.magnitude > maxSpeed || (autoskateDestination - transform.position).magnitude < 4f)
        {
            rb.velocity = new Vector3(rb.velocity.x * breakingModifier, rb.velocity.y, rb.velocity.z * breakingModifier);
        }
    }

    //If close enough to autoskate destination, invoke event to continue progress.
    private void AutoskateCheck()
    {
        if ((autoskateDestination - transform.position).magnitude < 3f)
        {
            GameplayManager.Instance.moveDone = true;
        }
    }

    private void ResetPlayer()
    {
        playerState = PlayerState.Control;
        transform.position = Vector3.zero;
    }

    #region Enable and Disable

    public void SetPlayerControl(int setState)
    {
        playerState = (PlayerState)setState;
    }

    private void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
        callAction.Enable();
        whistleAction.Enable();
        pauseAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        callAction.Disable();
        whistleAction.Disable();
        pauseAction.Disable();
    }
    #endregion

    #region EventListeners

    private void CutsceneListener(int progress)
    {
        if (waypointParent.transform.GetChild(progress) != null)
        {
            autoskateDestination = waypointParent.transform.GetChild(progress).position;
        }
        if (playerState != PlayerState.Autoskate)
        {
            playerState = PlayerState.Autoskate;
        }
    }

    private void LoadWaypoints(CutsceneData cutsceneData)
    {
        waypointParent = cutsceneData.waypointParent;
    }

    #endregion
}
