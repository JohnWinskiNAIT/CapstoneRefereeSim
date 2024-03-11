using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRMenuController : MonoBehaviour
{
    [SerializeField] LazerEmitter emitter;

    [SerializeField] GameObject canvasParent;
    Canvas canvas;

    [SerializeField] Camera cam;

    private void Start()
    {
        canvas = canvasParent.GetComponentInChildren<Canvas>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))   
        {
            emitter.Activate();
            //Debug.Log("MouseDown");
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            emitter.Deactivate();
            //Debug.Log("MouseUp");
        }
    }
}
