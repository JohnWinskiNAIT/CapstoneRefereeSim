using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    static public GameplayManager Instance;

    [SerializeField]
    CutsceneData playEndCutscene;

    int cutsceneStatus;

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
        cutsceneStatus = 0;
        GameplayEvents.CutsceneTrigger.Invoke(cutsceneStatus);
    }

    public void ProgressCutscene()
    {
        cutsceneStatus++;
        GameplayEvents.CutsceneTrigger.Invoke(cutsceneStatus);
    }
}

[Serializable]
public class CutsceneData
{
    public int numberOfPoints;
    public GameObject waypointParent;
    public GameObject cameraParent;
}
