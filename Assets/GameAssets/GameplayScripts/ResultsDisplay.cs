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
    RectTransform nameInput;

    [SerializeField]
    Vector3[] startCorners, finalCorners;

    [SerializeField]
    TextMeshProUGUI choiceText, actualText, timingText, promptText;

    [SerializeField]
    float transitionTime;

    float transitionTimer;
    bool resultsPulledUp, recorded;

    string selectedName;

    private void Awake()
    {
        continueInput = playerInputs.FindActionMap("Gameplay").FindAction("Call/Select");
        replayInput = playerInputs.FindActionMap("Gameplay").FindAction("Whistle/Cancel");
        resultsUI = GetComponent<RectTransform>();
        nameInput = transform.GetChild(transform.childCount - 1).GetComponent<RectTransform>();
        selectedName = string.Empty;
    }

    public void InitiateResults(int choiceId, int actualId, float timing)
    {
        ToggleNameEntry(false);
        Debug.Log(choiceId);
        string choice;
        string actual;
        if (actualId == -1)
        {
            actual = "None";
        }
        else
        {
            actual = Settings.mySettings.FindPenaltyId(actualId).penaltyText;
        }
        if (choiceId == -1)
        {
            choice = "None";
        }
        else
        {
            choice = Settings.mySettings.FindPenaltyId(choiceId).penaltyText;
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

                if (!nameInput.gameObject.activeSelf && replayInput.WasPressedThisFrame() && !recorded)
                {
                    ToggleNameEntry(true);
                }
                if (!nameInput.gameObject.activeSelf && continueInput.WasPressedThisFrame())
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

    public void ToggleNameEntry(bool toggle)
    {
        nameInput.gameObject.SetActive(toggle);

        if (toggle)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void SaveReplay()
    {
        recorded = true;
        GameplayManager.Instance.SaveRecording(selectedName);
        ToggleNameEntry(false);
    }

    public void SetName(string newName)
    {
        selectedName = newName;
    }

    private struct ResultsContainer
    {
        public string difference;
        public string chosenPenalty;
        public string actualPenalty;
    }
}
