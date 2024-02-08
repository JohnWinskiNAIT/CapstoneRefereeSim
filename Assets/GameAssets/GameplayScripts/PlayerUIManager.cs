using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    //Very stupid but just made because typing in 360 each time will make it an int unless you include the f affix, and this makes it more readable.
    const float totalFill = 360f;

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

    //Wheel related references.
    [SerializeField]
    GameObject selectionWheel;
    [SerializeField]
    TextMeshProUGUI wheelText;
    Image wheelGlow;
    Vector3 leftSideBL, leftSideTR, rightSideBL, rightSideTR;

    [SerializeField]
    WheelInformation wheelInfo;

    //Reference to the gameobjects/images that will denote the selection wheel.
    [SerializeField]
    GameObject wheelNotchObj, iconObj;
    GameObject[] currentNotches, currentIcons;
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

        currentNotches = new GameObject[0];
        currentIcons = new GameObject[0];
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
                GenerateNotches();
                GenerateIcons();
                wheelOpen = true;
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            if (selectionWheel.activeSelf)
            {
                selectionWheel.SetActive(false);
                RemoveWheelElements();
                wheelOpen = false;
            }
        }

        // Figures out where mouse is relative to center of screen and tries to find an appropriate quadrant to fill based on
        if (wheelOpen)
        {
            //Uses a utility method to check the percentage of the screen the cursor is on for determining angels.
            Vector2 mouseCheck = GameUtilities.CursorPercentage() - new Vector2(0.5f, 0.5f);
            //Self-made method to check for the magnitude (absolute distance) because percentage is width biased.
            Vector2 magnitudeCheck = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2, 0);
            if (magnitudeCheck.magnitude > Screen.height / 5)
            {
                float debugAngle;
                if (GameUtilities.CursorPercentage().x < 0.5f)
                {
                    debugAngle = totalFill - Vector2.Angle(Vector2.up, mouseCheck);
                }
                else
                {
                    debugAngle = Vector2.Angle(Vector2.up, mouseCheck);
                }

                //
                float segments = totalFill / wheelInfo.numberOfOptions;

                for (int i = 0; i < wheelInfo.numberOfOptions; i++)
                {
                    if (debugAngle > segments * i && debugAngle < segments * (i + 1))
                    {
                        wheelGlow.fillAmount = segments / totalFill;
                        wheelText.text = wheelInfo.optionText[i];
                        wheelGlow.transform.rotation = Quaternion.Euler(0, 0, -i * segments);
                    }
                }
            }
            else
            {
                wheelGlow.fillAmount = 0;
                wheelText.text = "None";
            }
        }
    }

    //Creates notches and rotates them dynamically according to the amount of segments in the selection wheel.
    private void GenerateNotches()
    {
        currentNotches = new GameObject[wheelInfo.numberOfOptions];
        float notchGap = (Screen.height / 2f) * 0.6f;
        float segments = totalFill / wheelInfo.numberOfOptions;
        for (int i = 0; i < wheelInfo.numberOfOptions; i++)
        {
            currentNotches[i] = Instantiate(wheelNotchObj, selectionWheel.transform);
            Vector2 position = new Vector2(Mathf.Sin((segments * i) * Mathf.Deg2Rad) * notchGap, Mathf.Cos((segments * i) * Mathf.Deg2Rad) * notchGap);
            currentNotches[i].GetComponent<RectTransform>().anchoredPosition = position;
            currentNotches[i].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 90 - segments * i);
        }
    }

    //Creates icons dynamically according to the amount of segments in the selection wheel.
    private void GenerateIcons()
    {
        currentIcons = new GameObject[wheelInfo.numberOfOptions];
        float notchGap = (Screen.height / 2f) * 0.6f;
        float segments = totalFill / wheelInfo.numberOfOptions;
        for (int i = 0; i < wheelInfo.numberOfOptions; i++)
        {
            currentIcons[i] = Instantiate(iconObj, selectionWheel.transform);
            Vector2 position = new Vector2(Mathf.Sin((segments * i + segments / 2) * Mathf.Deg2Rad) * notchGap, Mathf.Cos((segments * i + segments / 2) * Mathf.Deg2Rad) * notchGap);
            currentIcons[i].GetComponent<RectTransform>().anchoredPosition = position;
            currentIcons[i].GetComponent<Image>().sprite = wheelInfo.optionImages[i];
            //set the image for each icon
        }
    }

    private void RemoveWheelElements()
    {
        if (currentNotches.Length > 0)
        {
            for (int i = 0; i < wheelInfo.numberOfOptions; i++)
            {
                Destroy(currentNotches[i]);
            }
        }
        currentNotches = new GameObject[0];
        if (currentIcons.Length > 0)
        {
            for (int i = 0; i < wheelInfo.numberOfOptions; i++)
            {
                Destroy(currentIcons[i]);
            }
        }
        currentIcons = new GameObject[0];
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
        public Sprite[] optionImages;
        public string[] optionText;
    }
}
