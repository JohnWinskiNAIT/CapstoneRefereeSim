using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    //Used to compare InputActions.
    [SerializeField]
    InputActionAsset inputActions;
    InputAction whistleAction, callAction, wheelTestAction;

    [SerializeField]
    PlayerControl tempPlayerControl;

    //Reference to the image with a radial fill that will appear in the center of the screen.
    [SerializeField]
    Image inputStoreRadial;
    [SerializeField]
    RectTransform leftSideStore, rightSideStore;
    [SerializeField]
    GameObject selectionWheel, optionPrefab;
    Image wheelGlow;
    Vector3 leftSideBL, leftSideTR, rightSideBL, rightSideTR;

    [SerializeField]
    WheelInformation wheelInfo;

    //Reference to the gameobjects/images that will denote the selection wheel.
    [SerializeField]
    GameObject wheelNotch;
    GameObject[] currentNotches;
    public bool wheelOpen;

    //Number that the final anchor point offset for making a call or whistle will be.
    [SerializeField]
    float chargingCornerOffset;
    Vector3 chargingVectorOffset;

    //Affirming the inputs + default anchor corners
    private void Awake()
    {
        callAction = inputActions.FindActionMap("Gameplay").FindAction("Call");
        whistleAction = inputActions.FindActionMap("Gameplay").FindAction("Whistle");
        wheelTestAction = inputActions.FindActionMap("Gameplay").FindAction("WheelTest");

        // getting the anchors for the sideObjects
        leftSideBL = leftSideStore.anchorMin;
        leftSideTR = leftSideStore.anchorMax;
        rightSideBL = rightSideStore.anchorMin;
        rightSideTR = rightSideStore.anchorMax;
        chargingVectorOffset = new Vector3(chargingCornerOffset, 0, 0);

        //getting the 
        wheelOpen = false;
        wheelGlow = selectionWheel.transform.Find("WheelGlow").GetComponent<Image>();
    }

    // Visual effects are performed in this update.
    private void Update()
    {
        if (tempPlayerControl.storedAction != null)
        {
            inputStoreRadial.fillAmount = tempPlayerControl.StoredActionStatus();
            if (tempPlayerControl.storedAction == whistleAction)
            {
                rightSideStore.anchorMin = Vector3.Lerp(rightSideBL, rightSideBL - chargingVectorOffset, tempPlayerControl.StoredActionStatus());
                rightSideStore.anchorMax = Vector3.Lerp(rightSideTR, rightSideTR - chargingVectorOffset, tempPlayerControl.StoredActionStatus());
            }
            if (tempPlayerControl.storedAction == callAction)
            {
                leftSideStore.anchorMin = Vector3.Lerp(leftSideBL, leftSideBL + chargingVectorOffset, tempPlayerControl.StoredActionStatus());
                leftSideStore.anchorMax = Vector3.Lerp(leftSideTR, leftSideTR + chargingVectorOffset, tempPlayerControl.StoredActionStatus());
            }
        }
        else
        {
            inputStoreRadial.fillAmount = 0;
            rightSideStore.anchorMin = rightSideBL;
            rightSideStore.anchorMax = rightSideTR;

            leftSideStore.anchorMin = leftSideBL;
            leftSideStore.anchorMax = leftSideTR;
        }


        // Lockstate
        if (wheelTestAction.IsPressed())
        {
            Cursor.lockState = CursorLockMode.None;
            if (!selectionWheel.activeSelf)
            {
                selectionWheel.SetActive(true);
                wheelOpen = true;
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            if (selectionWheel.activeSelf)
            {
                selectionWheel.SetActive(false);
                wheelOpen = false;
            }
        }

        // Most of this is for testing, but debugAngle
        if (wheelOpen)
        {
            Vector2 mouseCheck = GameUtilities.CursorPercentage() - new Vector2(0.5f, 0.5f);
            float debugAngle;
            if (GameUtilities.CursorPercentage().x < 0.5f)
            {
                debugAngle = 360 - Vector2.Angle(Vector2.up, mouseCheck);
            }
            else
            {
                debugAngle = Vector2.Angle(Vector2.up, mouseCheck);
            }

            //
            float segments = 360 / wheelInfo.numberOfOptions;
            Debug.Log(segments);
            for (int i = 0; i < wheelInfo.numberOfOptions; i++)
            {
                if (debugAngle > segments * i && debugAngle < segments * (i + 1))
                {
                    wheelGlow.fillAmount = segments / 360;
                    wheelGlow.transform.rotation = Quaternion.Euler(0, 0, -i * segments);
                }
            }
        }
    }

    private void OnEnable()
    {
        wheelTestAction.Enable();
    }

    private void OnDisable()
    {
        wheelTestAction.Disable();
    }

    [Serializable]
    public struct WheelInformation
    {
        public int numberOfOptions;
        public Image[] optionImages;
        public string[] optionText;
    }
}
