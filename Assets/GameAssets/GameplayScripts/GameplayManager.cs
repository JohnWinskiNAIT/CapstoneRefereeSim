using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.XR.Oculus;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameplayManager : MonoBehaviour
{
    static public GameplayManager Instance;

    [SerializeField]
    CutsceneData playEndCutscene;

    CutsceneData currentCutscene;
    PlayInformation currentPlayInfo;

    int cutsceneStatus;
    float playTimer;

    public bool cameraDone, moveDone;
    bool penaltyOccured;

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
        playTimer += Time.deltaTime;
        PlayCheck();
    }

    private void PlayCheck()
    {
        if (playTimer > currentPlayInfo.penaltyTimestamp && !penaltyOccured)
        {
            penaltyOccured = true;
        }
    }

    //Creates the PlayInformation struct, and fills it.
    private void InitiatePlayInformation()
    {
        currentPlayInfo = new PlayInformation();
        currentPlayInfo.penaltyTimestamp = Random.Range(0, 50f);
        currentPlayInfo.playStop = Random.Range(0, 15f);
        //Replace this random range with a reference to a list of all players in the scene.
        int player1 = Random.Range(0, 30);
        int player2 = Random.Range(0, 30);
        while (player2 ==  player1)
        {
            player2 = Random.Range(0, 30);
        }
        currentPlayInfo.offenderId = player1;
        currentPlayInfo.affectedId = player2;
        currentPlayInfo.penaltyType = (PenaltyType)Random.Range(0, Enum.GetNames(typeof(PenaltyType)).Length);
    }

    private struct PlayInformation
    {
        //Penalty Timestamp denotes when the penalty will occur. Playstop indicates an additional timer added onto the Timestamp before the play will end (unless ended with whistle).
        public float penaltyTimestamp, playStop;
        //This contains the ID on the playerlist for the two players who will be involved in this incident.
        public int offenderId, affectedId;
        public PenaltyType penaltyType;
    }
}

[Serializable]
public struct CutsceneData
{
    public int numberOfPoints;
    public GameObject waypointParent;
    public GameObject cameraParent;
    public bool wheelOpen;
}

public enum PenaltyType
{
    HeadContact,
    Tripping,
    CrossCheck,
    BlindCheck,
    Holding,
    Interference,
    HighSticking,
    Hooking,
    Slashing,
    HandPass
}
