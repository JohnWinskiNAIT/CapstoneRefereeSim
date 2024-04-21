using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using static PlayerControl;
using UnityEngine.InputSystem;
using System;

public class VRMenuController : MonoBehaviour
{
    LazerEmitter emitter;
    [SerializeField] GameObject emitterObj;
    [SerializeField] GameObject Pointer;    //temporary
    [SerializeField] Transform emitterPosHead;
    [SerializeField] Transform emitterPosR;
    [SerializeField] Transform emitterPosL;

    [SerializeField] bool isController;
    [SerializeField] bool isRightHanded;

    [SerializeField] GameObject canvasParent;
    Canvas canvas;

    [SerializeField] GameObject camParent;
    [SerializeField] InputActionAsset inputActions;
    private InputAction moveAction, lookAction, callselectAction, pauseAction, whistleAction;
    float lookInput;
    bool isClicking;
    bool wasClicking;

    public float lookSensitivity = 1;

    //[SerializeField] InputAction action;


    void Awake()
    {
        moveAction = inputActions.FindActionMap("Gameplay").FindAction("Move");
        lookAction = inputActions.FindActionMap("Gameplay").FindAction("Look");
        callselectAction = inputActions.FindActionMap("Gameplay").FindAction("Call/Select");
        whistleAction = inputActions.FindActionMap("Gameplay").FindAction("Whistle/Cancel");
        pauseAction = inputActions.FindActionMap("Gameplay").FindAction("Pause");
    }

    #region Enable and Disable

    private void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
        callselectAction.Enable();
        whistleAction.Enable();
        pauseAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        callselectAction.Disable();
        whistleAction.Disable();
        pauseAction.Disable();
    }
    #endregion

    private void Start()
    {
        emitter = emitterObj.GetComponent<LazerEmitter>();
        canvas = canvasParent.GetComponentInChildren<Canvas>();

        //lookSensitivity = settings.lookSensitivity
    }

    void Update()
    {
        if (callselectAction.ReadValue<float>() > 0.3)   
        {   //check isClicking
            if (!wasClicking)
            {   //check wasClicking last frame
                emitter.Activate();
            }
            //Debug.Log("MouseDown");
            wasClicking = true;
        }
        else
        {
            emitter.Deactivate();
            wasClicking = false;
        }

        LazerPositionUpdate();

        PlayerRotation();
        //rotate off of values from thum stick
    }

    private void PlayerRotation()
    {
        lookInput = lookAction.ReadValue<Vector2>().x;
        if (Mathf.Abs(lookInput) > 0.2)
        {
            transform.Rotate(0, lookInput, 0);
        }
    }

    void LazerPositionUpdate()
    {
        if (isController)
        {
            Pointer.transform.position = emitterPosHead.position;
            emitterObj.transform.position = emitterPosHead.position;

            Pointer.transform.rotation = emitterPosHead.rotation;
            emitterObj.transform.rotation = emitterPosHead.rotation;
        }
        else
        {
            if (isRightHanded)
            {
                Pointer.transform.position = emitterPosR.position;
                emitterObj.transform.position = emitterPosR.position;

                Pointer.transform.rotation = emitterPosR.rotation;
                emitterObj.transform.rotation = emitterPosR.rotation;
                //Inputactions asset = right handed one
            }
            else
            {
                Pointer.transform.position = emitterPosL.position;
                emitterObj.transform.position = emitterPosL.position;

                Pointer.transform.rotation = emitterPosL.rotation;
                emitterObj.transform.rotation = emitterPosL.rotation;
                //Input actions asset = left handed one
            }
        }
    }
}
