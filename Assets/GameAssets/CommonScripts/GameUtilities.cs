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

    static public bool VREnabled()
    {
        return XRGeneralSettings.Instance?.Manager?.activeLoader;

        // if (GameUtilities.VREnabled) {...Code}
    }

}
