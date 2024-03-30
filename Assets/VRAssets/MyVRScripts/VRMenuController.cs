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
        if (callselectAction.ReadValue<float>() > 0.1)   
        {
            emitter.Activate();
            //Debug.Log("MouseDown");
        }
        else
        {
            emitter.Deactivate();
        }

        LazerPositionUpdate();

        CamRotationUpdate();
        //rotate off of values from thum stick
    }

    void CamRotationUpdate()
    {
        camParent.transform.Rotate(0, lookAction.ReadValue<Vector2>().x * lookSensitivity , 0);
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
