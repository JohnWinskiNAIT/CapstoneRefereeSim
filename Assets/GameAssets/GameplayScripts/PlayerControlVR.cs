using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Management;

public class PlayerControlVR : MonoBehaviour
{
    [SerializeField]
    LazerEmitter emitter;
    Canvas canvas;
    [SerializeField]
    InputActionAsset inputActions;
    [SerializeField]
    Camera cam;
    [SerializeField]
    GameObject vrLeftHand, vrRightHand;
    [SerializeField]    //this needs tweaking
    float vrLCallHeight, vrRMagnitude;
    [SerializeField]
    public GameObject vrCamera;
    [SerializeField]
    GameObject nonVRCamera;

    private Rigidbody rb;

    public float maxSpeed, accelerationSpeed, breakingModifier, storeInputDuration;
    public float lookSensitivity = 1;

    private InputAction moveAction, lookAction, pauseAction;
    public string HeldAction { get; private set; }

    private float storeTimestamp;

    private Vector2 moveInput;
    private float lookInput;
    public Vector3 CameraAngle { get; private set; }
    public Vector3 MoveAngle { get; private set; }

    Vector3 basePosition;

    private PlayerUIManager uiManager;
    public PlayerState CurrentPlayerState { get; private set; }
    private Vector3 savedVelocity;

    public bool isVREnabled;



    [SerializeField] GameObject VRUIHeadPos;
    [SerializeField] GameObject VRUIWorldPos;

    [SerializeField] GameObject[] Worldspacers; //UI Elements that need to be put into Worldspace when in VR
    [SerializeField] GameObject[] OnFacers;     //UI Elements 

    [SerializeField] GameObject?[] Hitboxes;     //Disabled initially, Enabled only in VR. Not used for the Wheel.

        //\aaaaaaaaaaaaaaaaaaaaaa/////////aaaaaaaaaaaaaaaaaaaaaaaaaaaa/////////aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa

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
        pauseAction = inputActions.FindActionMap("Gameplay").FindAction("Pause");
        lookAction = inputActions.FindActionMap("Gameplay").FindAction("Look");

        rb = GetComponent<Rigidbody>();
        CurrentPlayerState = PlayerState.Control;

        GameplayEvents.InitializePlay.AddListener(ResetPlayer);
        GameplayEvents.SetPause.AddListener(PausePlayer);

        basePosition = transform.position;
    }

    private void Start()
    {
        if (isVREnabled)
        {
            Destroy(nonVRCamera);
            Destroy(GetComponent<PlayerCamera>());
            Destroy(GetComponent<PlayerControl>());
        }

        uiManager = GameplayManager.Instance.playerUI.GetComponent<PlayerUIManager>();
        //uiManager.isVREnabled = isVREnabled;
        CameraAngle = cam.transform.rotation.eulerAngles;
        Cursor.lockState = CursorLockMode.Locked;

        RepositionUI();
    }

    // Update is called once per frame
    void Update()
    {
        GameplayManager.Instance.cameraDone = true;

        //Cannot move if selection wheel is open.
        if (CurrentPlayerState == PlayerState.Control)
        {
            // Getting Vector2 inputs for move and look.
            moveInput = moveAction.ReadValue<Vector2>();
            lookInput = lookAction.ReadValue<Vector2>().x; 
            //Debug.Log(lookInput);

            //Call pausemanager when pause is pressed.
            if (pauseAction.WasPressedThisFrame())
            {
                GameplayEvents.SetPause.Invoke(true);
            }

            //Stored action stuff. If nothing is stored, it checks for to store whistle or call. If something is stored,
            //it runs a check to see if it's still being held.
            if (GameplayManager.Instance.Call == GameplayManager.CallState.None)
            {
                StoredActionCheck();
            }

            //Vector3 test1 = new Vector2(rb.velocity.x, rb.velocity.z);
            //Vector3 test2 = new Vector2(playe)
            //Debug.Log(Vector3.Angle(Vector3.forward, test2));
            //Debug.Log(transform.forward);
        }
        else
        {
            moveInput = Vector2.zero;
            HeldAction = null;
        }
        //Debug.Log(HeldAction);
    }

    void RepositionUI()
    {
        for (int i = 0; i < Worldspacers.Length; i++)
        {
            if (Worldspacers[i].gameObject.GetComponent<Canvas>() != null)
            {
                Worldspacers[i].gameObject.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            }
            Worldspacers[i].gameObject.transform.SetParent(VRUIWorldPos.transform, false);
            Worldspacers[i].gameObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            //Debug.Log($"{Worldspacers[i].gameObject.transform.name} is parented to:{Worldspacers[i].gameObject.transform.parent.name}");
        }
        for (int i = 0; i < OnFacers.Length; i++)
        {
            OnFacers[i].gameObject.transform.SetParent(VRUIHeadPos.transform, false);
            OnFacers[i].gameObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            //Debug.Log($"{OnFacers[i].gameObject.transform.name} is parented to:{OnFacers[i].gameObject.transform.parent.name}");
        }
    }

    //VR STORED ACTION METHOD
    private void StoredActionCheck()
    {
        if (HeldAction == null)
        {
            if ((vrRightHand.transform.position - cam.transform.position).magnitude <= vrRMagnitude)
            {
                HeldAction = "Whistle";
            }
            else if (vrLeftHand.transform.position.y >= vrLCallHeight)
            {
                HeldAction = "Call";
            }
        }
        else
        {
            //Add an "and" to this alongside currentAction being Whistle
            if ((vrRightHand.transform.position - cam.transform.position).magnitude <= vrRMagnitude && HeldAction == "Whistle")
            {
                storeTimestamp += Time.deltaTime;
                if (storeTimestamp > storeInputDuration)
                {
                    WhistleActivate();
                }
            }
            else if (vrLeftHand.transform.position.y >= vrLCallHeight && HeldAction == "Call")
            {
                storeTimestamp += Time.deltaTime;
                if (storeTimestamp > storeInputDuration)
                {
                    CallActivate();
                }
            }
            else
            {
                storeTimestamp = 0f;
                HeldAction = null;
            }
        }

        //Debug.Log(vrLeftHand.transform.position.y);
        //Debug.Log((vrRightHand.transform.position - cam.transform.position).magnitude);
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
            returnValue = (Time.time - storeTimestamp) / storeInputDuration;
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
            PlayerRotation();
        }
    }

    private void PlayerMovement()
    {
        //Determines the angle between where the player's velocity is going and the player's input.
        MoveAngle = new Vector3(moveInput.x, 0, moveInput.y);

        //If VR active, usees camera Y instead of player Y.
        Quaternion angleCheck;
        //if (isVREnabled)
        //{
        angleCheck = Quaternion.AngleAxis(cam.transform.eulerAngles.y, Vector3.up);
        //}
        //else
        //{
        //    angleCheck = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up);
        //}

        MoveAngle = angleCheck * MoveAngle;

        //Add an object over a GameObject friend parameter to have it display where the player is moving.
        /*if (friend != null)
        {
            friend.transform.position = transform.position + (horizontalCheck * 2);
        }*/

        //Add relative force.
        rb.AddForce(MoveAngle * (accelerationSpeed * Time.fixedDeltaTime), ForceMode.Force);

        //Apply cap if greater than max speed. (Parabolic acceleration curve for later?)
        Vector2 capTest = new(rb.velocity.x, rb.velocity.z);
        if (capTest.magnitude > maxSpeed || Vector2.Angle(new Vector2(MoveAngle.x, MoveAngle.z), new Vector2(rb.velocity.x, rb.velocity.z)) > 90)
        {
            rb.velocity = new Vector3(rb.velocity.x * breakingModifier, rb.velocity.y, rb.velocity.z * breakingModifier);
        }
    }

    private void PlayerRotation()
    {
        if (Mathf.Abs(lookInput) > 0.2)
        {
            transform.Rotate(0, lookInput, 0);
        }
    }

    //public void SetCamAngles(Vector3 camAngles)
    //{
    //    CameraAngle = camAngles;
    //}

    //If close enough to autoskate destination, invoke event to continue progress.

    #region Enable and Disable

    public void SetPlayerControl(PlayerState setState)
    {
        CurrentPlayerState = setState;
    }

    private void OnEnable()
    {
        moveAction.Enable();
        pauseAction.Enable();
        lookAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        pauseAction.Disable();
        lookAction.Disable();
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
