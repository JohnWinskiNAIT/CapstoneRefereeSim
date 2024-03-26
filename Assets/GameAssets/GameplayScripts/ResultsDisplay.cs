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
    TextMeshProUGUI choiceText, actualText, timingText, promptText;

    [SerializeField]
    float transitionTime;

    float transitionTimer;
    bool resultsPulledUp, recorded;

    private void Awake()
    {
        continueInput = playerInputs.FindActionMap("Gameplay").FindAction("Call/Select");
        replayInput = playerInputs.FindActionMap("Gameplay").FindAction("Whistle/Cancel");
        resultsUI = GetComponent<RectTransform>();
    }

    public void InitiateResults(int choiceId, int actualId, float timing)
    {
        string choice = Settings.mySettings.penalties[choiceId].penaltyText;
        string actual;
        if (actualId == -1)
        {
            actual = "None";
        }
        else
        {
            actual = Settings.mySettings.penalties[actualId].penaltyText;
        }

        float timer = Mathf.Round(timing * 10f) * 0.1f;

        choiceText.text = $"Player Call: {choice}";
        actualText.text = $"Actual Penalty: {actual}";
        timingText.text = $"Time Before Call: {timer}s";

        transitionTimer = 0;
        resultsPulledUp = true;
        recorded = false;
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
                resultsUI.anchorMin = finalCorners[0];
                resultsUI.anchorMax = finalCorners[1];

                if (replayInput.WasPressedThisFrame() && !recorded)
                {
                    GameplayManager.Instance.SaveRecording();
                    recorded = true;

                }
                if (continueInput.WasPressedThisFrame())
                {
                    resultsPulledUp = false;
                    GameplayEvents.InitializePlay.Invoke();
                }
            }

            if (recorded)
            {
                promptText.text = "Replay recorded. Left Click to continue.";
            }
            else
            {
                promptText.text = "Left Click to continue. Right Click to save the replay.";
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
