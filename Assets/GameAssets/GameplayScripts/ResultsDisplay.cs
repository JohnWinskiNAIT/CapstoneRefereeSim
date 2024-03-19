using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultsDisplay : MonoBehaviour
{
    [SerializeField]
    RectTransform resultsUI;

    [SerializeField]
    Tuple<Vector3, Vector3> startCorners, finalCorners;

    [SerializeField]
    TextMeshProUGUI successText, timingText;

    [SerializeField]
    float transitionTime;

    float transitionTimer;
    bool resultsPulledUp;

    public void InitiateResults()
    {
        GameplayManager.Instance.
    }

    // Update is called once per frame
    void Update()
    {
        if (resultsPulledUp)
        {
            if (transitionTimer < transitionTime)
            {
                resultsUI.anchorMin = Vector3.Lerp(startCorners.Item1, finalCorners.Item1, transitionTimer / transitionTime);
                resultsUI.anchorMax = Vector3.Lerp(startCorners.Item2, finalCorners.Item2, transitionTimer / transitionTime);
            }
        }
    }
}
