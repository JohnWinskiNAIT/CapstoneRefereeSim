using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    //Very stupid but just made because typing in 360 each time will make it an int unless you include the f affix, and this makes it more readable.
    const float totalFill = 360f;
    const float screenScaleDenominator = 1280f;

    //Used to compare InputActions.
    [SerializeField]
    InputActionAsset inputActions;
    InputAction whistleCancelAction, callSelectAction, wheelTestAction;

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
    Transform wheelWorldspaceTrans;

    [SerializeField]
    Canvas playerUICanvas;

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

    public bool isVREnabled;  //gets set to true/false by playerControl
    Vector2 mouseCheck = new();
    Vector2 magnitudeCheck = new();
    [SerializeField] GameObject VRPointer;

    WheelInformation callInformation;
    WheelInformation whistleInformation;

    //Affirming the inputs + default anchor corners
    private void Awake()
    {
        callSelectAction = inputActions.FindActionMap("Gameplay").FindAction("Call/Select");
        whistleCancelAction = inputActions.FindActionMap("Gameplay").FindAction("Whistle/Cancel");
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

        GameplayEvents.OpenWheel.AddListener(ToggleWheel);
        GameplayEvents.InitializePlay.AddListener(ResetUI);
        ToggleWheel(false);
    }

    private void Start()
    {
        if (SettingsHolder.mySettings.WhistleCount > 0)
        {
            whistleInformation = GenerateWheelInformation(true);
        }
        if (SettingsHolder.mySettings.penalties.Length - SettingsHolder.mySettings.WhistleCount > 0)
        {
            callInformation = GenerateWheelInformation(false);
        }

        wheelInfo = whistleInformation;
    }

    private WheelInformation GenerateWheelInformation(bool isWhistle)
    {
        WheelInformation info = new WheelInformation();
        SettingsData settings = SettingsHolder.mySettings;
        int optionsCounter = 0;

        if (isWhistle)
        {
            info.options = new WheelInformation.Option[settings.WhistleCount];
        }
        else
        {
            info.options = new WheelInformation.Option[settings.EnabledCount - settings.WhistleCount];
        }

        for (int i = 0; i < settings.penalties.Length; i++)
        {
            if (settings.penalties[i].isWhistle == isWhistle && settings.penalties[i].isEnabled)
            {
                info.options[optionsCounter].optionImage = settings.penalties[i].RefereeSprite;
                info.options[optionsCounter].optionText = settings.penalties[i].penaltyText;
                info.options[optionsCounter].optionType = PenaltyType.HeadContact;
                info.options[optionsCounter].optionId = settings.penalties[i].penaltyId;
                optionsCounter++;
            }
        }
        return info;
    }

    // Visual effects are performed in this update.
    private void Update()
    {
        if (tempPlayerControl.HeldAction != null)
        {
            inputStoreRadial.fillAmount = tempPlayerControl.StoredActionStatus();
            if (tempPlayerControl.HeldAction == whistleCancelAction)
            {
                rightSideStore.anchorMin = Vector3.Lerp(rightSideBL, rightSideBL - chargingVectorOffset, tempPlayerControl.StoredActionStatus());
                rightSideStore.anchorMax = Vector3.Lerp(rightSideTR, rightSideTR - chargingVectorOffset, tempPlayerControl.StoredActionStatus());
            }
            if (tempPlayerControl.HeldAction == callSelectAction)
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

        if (Input.GetKeyDown(KeyCode.V))
        {
            if (isVREnabled)
            {
                isVREnabled = false;
            }
            else
            {
                isVREnabled = true;
            }
        }

        // Lockstate
        /*if (wheelTestAction.IsPressed())
        {
            ToggleWheel(true);
        }
        else
        {
            ToggleWheel(false);
        }*/

        // Figures out where mouse is relative to center of screen and tries to find an appropriate quadrant to fill based on
        if (wheelOpen)
        {
            if (isVREnabled)
            {
                playerUICanvas.renderMode = RenderMode.WorldSpace;
                playerUICanvas.transform.SetPositionAndRotation(wheelWorldspaceTrans.position, wheelWorldspaceTrans.rotation);
                playerUICanvas.transform.localScale = wheelWorldspaceTrans.localScale;

                mouseCheck = GameUtilities.CursorPercentage() - new Vector2(0.5f, 0.5f);

                //Self-made method to check for the magnitude (absolute distance) because percentage is width biased.
                magnitudeCheck = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2, 0);

            }
            else    //We will need to expand this out AGAIN when we want to do the controller joystick thing
                    //we could do it in an Enum. ControlType {Mouse, Controller, VRHands}
            {
                playerUICanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                playerUICanvas.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                playerUICanvas.transform.localScale = Vector3.one;
                //Uses a utility method to check the percentage of the screen the cursor is on for determining angels.
                mouseCheck = GameUtilities.CursorPercentage() - new Vector2(0.5f, 0.5f);

                //Self-made method to check for the magnitude (absolute distance) because percentage is width biased.
                magnitudeCheck = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2, 0);
            }

            if (magnitudeCheck.magnitude > Screen.height / 5)
            {
                float mouseAngle;
                if (GameUtilities.CursorPercentage().x < 0.5f)
                {
                    mouseAngle = totalFill - Vector2.Angle(Vector2.up, mouseCheck);
                }
                else
                {
                    mouseAngle = Vector2.Angle(Vector2.up, mouseCheck);
                }

                //
                float segments = totalFill / wheelInfo.NumberOfOptions;

                for (int i = 0; i < wheelInfo.NumberOfOptions; i++)
                {
                    if (mouseAngle > segments * i && mouseAngle < segments * (i + 1))
                    {
                        wheelGlow.fillAmount = segments / totalFill;
                        wheelText.text = wheelInfo.options[i].optionText;
                        wheelGlow.transform.rotation = Quaternion.Euler(0, playerUICanvas.transform.eulerAngles.y, -i * segments);

                        //Input to choose a penalty.
                        if (callSelectAction.WasPressedThisFrame())
                        {
                            GameplayManager.Instance.ConfirmChoice(wheelInfo.options[i].optionId);
                        }
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

    public void ToggleWheel(bool enable)
    {
        if (enable)
        {
            Cursor.lockState = CursorLockMode.None;
            if (!selectionWheel.activeSelf)
            {
                selectionWheel.SetActive(true);
                GenerateNotches();
                GenerateIcons();
                wheelOpen = true;
                tempPlayerControl.SetPlayerControl(PlayerControl.PlayerState.Lockout);
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
                tempPlayerControl.SetPlayerControl(PlayerControl.PlayerState.Control);
            }
        }
    }

    //Creates notches and rotates them dynamically according to the amount of segments in the selection wheel.
    private void GenerateNotches()
    {
        currentNotches = new GameObject[wheelInfo.NumberOfOptions];
        float notchGap = (Screen.height / 2f) * 0.65f;
        float segments = totalFill / wheelInfo.NumberOfOptions;
        float notchScale = (Screen.width / screenScaleDenominator);
        for (int i = 0; i < wheelInfo.NumberOfOptions; i++)
        {
            currentNotches[i] = Instantiate(wheelNotchObj, selectionWheel.transform);
            Vector2 position = new(Mathf.Sin((segments * i) * Mathf.Deg2Rad) * notchGap, Mathf.Cos((segments * i) * Mathf.Deg2Rad) * notchGap);
            currentNotches[i].transform.localScale = new Vector3(notchScale, 1f, 1f);
            currentNotches[i].GetComponent<RectTransform>().anchoredPosition = position;
            currentNotches[i].GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 90 - segments * i);
        }
    }

    //Creates icons dynamically according to the amount of segments in the selection wheel.
    private void GenerateIcons()
    {
        currentIcons = new GameObject[wheelInfo.NumberOfOptions];
        float notchGap = (Screen.height / 2f) * 0.6f;
        float segments = totalFill / wheelInfo.NumberOfOptions;
        float iconScale = 1f * (Screen.width / screenScaleDenominator);
        for (int i = 0; i < wheelInfo.NumberOfOptions; i++)
        {
            currentIcons[i] = Instantiate(iconObj, selectionWheel.transform);
            Vector2 position = new(Mathf.Sin((segments * i + segments / 2) * Mathf.Deg2Rad) * notchGap, Mathf.Cos((segments * i + segments / 2) * Mathf.Deg2Rad) * notchGap);
            currentIcons[i].transform.localScale = new Vector3(iconScale, iconScale, iconScale);
            currentIcons[i].GetComponent<RectTransform>().anchoredPosition = position;
            currentIcons[i].GetComponent<Image>().sprite = wheelInfo.options[i].optionImage;
        }
    }

    private void RemoveWheelElements()
    {
        if (currentNotches.Length > 0)
        {
            for (int i = 0; i < wheelInfo.NumberOfOptions; i++)
            {
                Destroy(currentNotches[i]);
            }
        }
        currentNotches = new GameObject[0];
        if (currentIcons.Length > 0)
        {
            for (int i = 0; i < wheelInfo.NumberOfOptions; i++)
            {
                Destroy(currentIcons[i]);
            }
        }
        currentIcons = new GameObject[0];
    }

    private void ResetUI()
    {
        ToggleWheel(false);
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
        public int NumberOfOptions
        {
            get { return options.Length; }
        }
        public Option[] options;

        [Serializable]
        public struct Option
        {
            public Sprite optionImage;
            public string optionText;
            public PenaltyType optionType;

            public int optionId;
        }
    }
}
