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
        string choice = SettingsHolder.mySettings.penalties[choiceId].penaltyText;
        string actual = SettingsHolder.mySettings.penalties[actualId].penaltyText;
        float timer = Mathf.Round(timing * 10f) * 0.1f;

        choiceText.text = $"Player Call: {choice}";
        actualText.text = $"Actual Penalty: {actual}";
        timingText.text = $"Time Before Call: {timer}s";

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

    private struct ResultsContainer
    {
        public string difference;
        public string chosenPenalty;
        public string actualPenalty;
    }
}
