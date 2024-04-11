using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
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
    public Camera cam;

    private Rigidbody rb;

    public float maxSpeed, accelerationSpeed, breakingModifier, storeInputDuration;
    [SerializeField]
    float cameraSpeed, cameraMaxY, cameraMinY;

    private InputAction moveAction, lookAction, callAction, pauseAction, whistleAction;
    public InputAction HeldAction { get; private set; }

    private float storeTimestamp;

    private Vector2 moveInput, lookInput;
    public Vector3 CameraAngle { get; private set; }
    public Vector3 InputAngle { get; private set; }

    Vector3 basePosition;

    private PlayerUIManager uiManager;
    public PlayerState CurrentPlayerState { get; private set; }
    private Vector3 savedVelocity;

    public bool isVREnabled;

    /// <summary>
    /// ////////////////////bool isInMenu; maybe this should e covered in state?
    /// we want to make the Call button Call when playying hockey, but be a Menu selecting pointer every other time
    /// </summary>

    public enum PlayerState
    {
        Control,
        Lockout,
        Autoskate
    }

    // Makes sure to get all actions on Awake as opposed to start, otherwise OnEnable goes first.
    void Awake()
    {
        isVREnabled = GameUtilities.VREnabled();

        moveAction = inputActions.FindActionMap("Gameplay").FindAction("Move");
        lookAction = inputActions.FindActionMap("Gameplay").FindAction("Look");
        callAction = inputActions.FindActionMap("Gameplay").FindAction("Call/Select");
        whistleAction = inputActions.FindActionMap("Gameplay").FindAction("Whistle/Cancel");
        pauseAction = inputActions.FindActionMap("Gameplay").FindAction("Pause");



        rb = GetComponent<Rigidbody>();
        CurrentPlayerState = PlayerState.Control;

        GameplayEvents.InitializePlay.AddListener(ResetPlayer);
        GameplayEvents.SetPause.AddListener(PausePlayer);

        basePosition = transform.position;
    }

    private void Start()
    {
        if (!isVREnabled)
        {
            Destroy(gameObject.GetComponent<PlayerControlVR>().vrCamera);
            Destroy(gameObject.GetComponent<XROrigin>());
            Destroy(gameObject.GetComponent<PlayerControlVR>());
        }

        uiManager = GameplayManager.Instance.playerUI.GetComponent<PlayerUIManager>();
        //uiManager.isVREnabled = isVREnabled;
        GameplayManager.Instance.DeclarePlayer(gameObject);
        CameraAngle = cam.transform.rotation.eulerAngles;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //Cannot move if selection wheel is open.
        if (CurrentPlayerState == PlayerState.Control)
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
            if (GameplayManager.Instance.Call == GameplayManager.CallState.None)
            {
                if (HeldAction == null)
                {
                    if (whistleAction.WasPressedThisFrame())
                    {
                        HeldAction = whistleAction;
                        storeTimestamp = 0;
                    }
                    if (callAction.WasPressedThisFrame())
                    {
                        HeldAction = callAction;
                        storeTimestamp = 0;
                    }
                }
                else
                {
                    StoredActionCheck();
                }
            }

            //Vector3 test1 = new Vector2(rb.velocity.x, rb.velocity.z);
            //Vector3 test2 = new Vector2(playe)
            //Debug.Log(Vector3.Angle(Vector3.forward, test2));
            //Debug.Log(transform.forward);
        }
        else
        {
            moveInput = Vector2.zero;
            lookInput = Vector2.zero;
            HeldAction = null;
        }
    }

    //This checks if the stored action (whistle or call) is still being held. If not, cancel the charge.
    //If true, check if it's been held for long enough and use the appropriate method if so.
    private void StoredActionCheck()
    {
        if (!HeldAction.IsPressed())
        {
            HeldAction = null;
        }
        else
        {
            if (storeTimestamp > storeInputDuration)
            {
                if (HeldAction == whistleAction)
                {
                    WhistleActivate();
                }
                if (HeldAction == callAction)
                {
                    CallActivate();
                }
            }
            else
            {
                storeTimestamp += Time.deltaTime;
            }
        }
    }

    //Public function for UI to check how long the action has been stored for as a time (t) value.
    public float StoredActionStatus()
    {
        float returnValue;
        if (HeldAction == null)
        {
            returnValue = 0f;
        }
        else
        {
            returnValue = storeTimestamp / storeInputDuration;
        }

        return returnValue;
    }

    private void CallActivate()
    {
        //Unfinished.
        GameplayManager.Instance.SetCallTimer();
        GameplayManager.Instance.CallPrep(false);
        HeldAction = null;
    }

    private void WhistleActivate()
    {
        //Unfinished.
        GameplayManager.Instance.SetCallTimer();
        GameplayManager.Instance.CallPrep(true);
        GameplayEvents.EndPlay.Invoke();
        HeldAction = null;
    }

    private void PausePlayer(bool pausing)
    {
        if (pausing)
        {
            CurrentPlayerState = PlayerState.Lockout;
            savedVelocity = rb.velocity;
            rb.velocity = Vector3.zero;
        }
        else
        {
            CurrentPlayerState = PlayerState.Control;
            rb.velocity = savedVelocity;
        }
    }

    private void FixedUpdate()
    {
        if (CurrentPlayerState == PlayerState.Control)
        {
            //Do all movement-related changes in here.
            PlayerMovement();

            //Camera logic might need to be determined here then visuals performed in late update.
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
            float rotateValue = (CameraAngle.y - transform.rotation.eulerAngles.y);
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
        CameraAngle += new Vector3(-lookInput.y, lookInput.x, 0) * (cameraSpeed * Time.deltaTime);

        //Full rotation logic + clamping the x angle to serialized values since it controls the up/down of the camera.
        if (CameraAngle.x > 360)
        {
            CameraAngle = new Vector3(CameraAngle.x - 360, CameraAngle.y);
        }
        if (CameraAngle.x < 0)
        {
            CameraAngle = new Vector3(CameraAngle.x + 360, CameraAngle.y);
        }
        if (CameraAngle.x < 180 && CameraAngle.x > cameraMinY)
        {
            CameraAngle = new Vector3(cameraMinY, CameraAngle.y);
        }
        if (CameraAngle.x > 180 && CameraAngle.x < cameraMaxY)
        {
            CameraAngle = new Vector3(cameraMaxY, CameraAngle.y);
        }

        if (CameraAngle.y > 360)
        {
            CameraAngle = new Vector3(CameraAngle.x, CameraAngle.y - 360);
        }
        if (CameraAngle.y < 0)
        {
            CameraAngle = new Vector3(CameraAngle.x, CameraAngle.y + 360);
        }
    }

    private void PlayerMovement()
    {
        //Determines the angle between where the player's velocity is going and the player's input.
        InputAngle = new Vector3(moveInput.x, 0, moveInput.y);

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

        InputAngle = angleCheck * InputAngle;

        //Add an object over a GameObject friend parameter to have it display where the player is moving.
        /*if (friend != null)
        {
            friend.transform.position = transform.position + (horizontalCheck * 2);
        }*/

        //Add relative force.
        rb.AddForce(InputAngle * (accelerationSpeed * Time.fixedDeltaTime), ForceMode.Force);

        //Apply cap if greater than max speed. (Parabolic acceleration curve for later?)
        Vector2 capTest = new(rb.velocity.x, rb.velocity.z);
        if (capTest.magnitude > maxSpeed || Vector2.Angle(new Vector2(InputAngle.x, InputAngle.z), new Vector2(rb.velocity.x, rb.velocity.z)) > 90)
        {
            rb.velocity = new Vector3(rb.velocity.x * breakingModifier, rb.velocity.y, rb.velocity.z * breakingModifier);
        }
    }

    public void SetCamAngles(Vector3 camAngles)
    {
        CameraAngle = camAngles;
    }

    //If close enough to autoskate destination, invoke event to continue progress.

    #region Enable and Disable

    public void SetPlayerControl(PlayerState setState)
    {
        CurrentPlayerState = setState;
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

    private void ResetPlayer()
    {
        if (rb.isKinematic)
        {
            rb.isKinematic = false;
        }

        CurrentPlayerState = PlayerState.Control;
        transform.position = basePosition;
    }

    #endregion
}
