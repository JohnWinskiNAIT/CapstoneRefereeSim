using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Management;

public static class GameUtilities
{
    //Static class to help with regularly used features in other scripts.

    static public Vector2 CursorPercentage()
    {
        return new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
    }

    static public Vector2 CursorPercentage(Canvas canvas, GameObject VRPointer)
    {
        //Vector3 VRPointerPosition = canvas.gameObject.transform.position - VRPointer.transform.position
        return new Vector2(
            VRPointer.transform.position.x / canvas.transform.lossyScale.x, 
            VRPointer.transform.position.y / canvas.transform.lossyScale.y);
    }

    static public bool VREnabled()
    {
        return XRGeneralSettings.Instance?.Manager?.activeLoader;

        // if (GameUtilities.VREnabled) {...Code}
    }

}
