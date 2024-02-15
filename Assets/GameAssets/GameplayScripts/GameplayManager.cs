using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    static public GameplayManager Instance;

    [SerializeField]
    CutsceneData playEndCutscene;

    CutsceneData currentCutscene;

    int cutsceneStatus;

    public bool cameraDone, moveDone;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = gameObject.GetComponent<GameplayManager>();
        }
        else
        {
            Destroy(gameObject);
        }

        GameplayEvents.EndPlay.AddListener(EndPlay);
    }

    // Update is called once per frame
    public void EndPlay()
    {
        GameplayEvents.LoadCutscene.Invoke(playEndCutscene);
        currentCutscene = playEndCutscene;
        cutsceneStatus = 0;
        GameplayEvents.CutsceneTrigger.Invoke(cutsceneStatus);
    }

    public void ProgressCutscene()
    {
        cutsceneStatus++;
        if (cutsceneStatus < currentCutscene.numberOfPoints)
        {
            GameplayEvents.CutsceneTrigger.Invoke(cutsceneStatus);
        }
        else
        {
            if (currentCutscene.wheelOpen)
            {
                GameplayEvents.OpenWheel.Invoke(true);
            }
        }
        moveDone = false;
        cameraDone = false;
    }

    private void Update()
    {
        if (moveDone && cameraDone)
        {
            ProgressCutscene();
        }
    }
}

[Serializable]
public class CutsceneData
{
    public int numberOfPoints;
    public GameObject waypointParent;
    public GameObject cameraParent;
    public bool wheelOpen;
}
