using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    CutsceneData playEndCutscene;
    int cutsceneStatus;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void EndPlay()
    {
        GameplayEvents.LoadCutscene.Invoke(playEndCutscene);
    }
}

[Serializable]
public class CutsceneData
{
    public int numberOfPoints;
    public GameObject waypointParent;
    public GameObject cameraParent;
}
