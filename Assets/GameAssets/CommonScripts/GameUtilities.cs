using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Management;

public class GameUtilities
{
    //Class of Static Methods to help with regularly used features in other scripts.
    public static string PlayerSkinPath = "PlayerSkins/";
    public static InputActionAsset[] actionMapList = new InputActionAsset[4];

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

    static public void SetInputActions(InputActionAsset keyboard, InputActionAsset controller, InputActionAsset vrLeft, InputActionAsset vrRight)
    {
        actionMapList[0] = keyboard;
        actionMapList[1] = controller;
        actionMapList[2] = vrLeft;
        actionMapList[3] = vrRight;
    }

    static public Material RetrievePlayerSkin(int id)
    {
        Material selectedMaterial = null;

        selectedMaterial = Resources.Load<Material> (PlayerSkinPath + id);

        return selectedMaterial;
    }
}
