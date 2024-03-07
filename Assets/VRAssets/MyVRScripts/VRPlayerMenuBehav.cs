using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Management;

public class VRPlayerMenuBehav : MonoBehaviour
{
    [SerializeField] LazerEmitter emitter;


    /// <summary>
    /// ////////////////////bool isInMenu; maybe this should e covered in state?
    /// we want to make the Call button Call when playying hockey, but be a Menu selecting pointer every other time
    /// </summary>

    // Makes sure to get all actions on Awake as opposed to start, otherwise OnEnable goes first.
    void Awake()
    {

    }

    private void Start()
    {

    }


    void Update()
    {

    }

}