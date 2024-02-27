using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stinkycontroller : MonoBehaviour
{
    [SerializeField] LazerEmitter emitter;

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
