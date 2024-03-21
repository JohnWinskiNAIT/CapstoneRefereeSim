using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResultsDisplay : MonoBehaviour
{
    [SerializeField]
    InputActionAsset playerInputs;

    InputAction continueInput, replayInput;

    RectTransform resultsUI;

    [SerializeField]
    Vector3[] startCorners, finalCorners;

    [SerializeField]
    TextMeshProUGUI choiceText, actualText, timingText;

    [SerializeField]
    float transitionTime;

    float transitionTimer;
    bool resultsPulledUp;

    private void Awake()
    {
        continueInput = playerInputs.FindActionMap("Gameplay").FindAction("Call/Select");
        replayInput = playerInputs.FindActionMap("Gameplay").FindAction("Whistle/Cancel");
        resultsUI = GetComponent<RectTransform>();
    }

    public void InitiateResults(int choiceId, int actualId, float timing)
    {
        choiceText.text = GameplayManager.Instance.CurrentPlayInfo.penaltyType.ToString();
        actualText.text = GameplayManager.Instance.CurrentPlayInfo.penaltyType.ToString();

        choiceText.text = SettingsHolder.mySettings.penalties[choiceId].penaltyText;
        actualText.text = SettingsHolder.mySettings.penalties[actualId].penaltyText;

        timingText.text = GameplayManager.Instance.CurrentPlayInfo.penaltyTimer.ToString();
        transitionTimer = 0;
        resultsPulledUp = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (resultsPulledUp)
        {
            if (transitionTimer < transitionTime)
            {
                resultsUI.anchorMin = Vector3.Lerp(startCorners[0], finalCorners[0], transitionTimer / transitionTime);
                resultsUI.anchorMax = Vector3.Lerp(startCorners[1], finalCorners[1], transitionTimer / transitionTime);
                transitionTimer += Time.deltaTime;
            }
            else
            {
                if (continueInput.WasPressedThisFrame())
                {
                    resultsPulledUp = false;
                    GameplayEvents.InitializePlay.Invoke();
                }
            }
        }
    }

    private void OnEnable()
    {
        continueInput.Enable();
        replayInput.Enable();
    }

    private void OnDisable()
    {
        continueInput.Disable();
        replayInput.Disable();
    }

    private struct ResultsContainer
    {
        public string difference;
        public string chosenPenalty;
        public string actualPenalty;
    }
}
