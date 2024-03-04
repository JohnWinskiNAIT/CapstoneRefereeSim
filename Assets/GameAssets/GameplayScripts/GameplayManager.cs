using Oculus.VoiceSDK.UX;
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
    float playTimer, callTimestamp;

    public bool cameraDone, moveDone;
    bool playOngoing, penaltyOccured, penaltyCall;

    bool freezeManager;

    public GameObject playerUI;

    [SerializeField]
    GameObject playTest;

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
        GameplayEvents.InitializePlay.AddListener(StartPlay);
        GameplayEvents.SetPause.AddListener(PauseGame);

        GameplayEvents.InitializePlay.Invoke();
    }

    // Update is called once per frame
    public void EndPlay()
    {
        if (playOngoing)
        {
            playOngoing = false;
            Debug.Log($"Difference: {callTimestamp - currentPlayInfo.penaltyTimer}");
        }
        GameplayEvents.LoadCutscene.Invoke(playEndCutscene);
        currentCutscene = playEndCutscene;
        cutsceneStatus = 0;
        GameplayEvents.CutsceneTrigger.Invoke(cutsceneStatus);
    }

    public void PauseGame(bool pauseBool)
    {
        freezeManager = pauseBool;
    }

    public void SetCallTimer()
    {
        if (callTimestamp == 0)
        {
            callTimestamp = playTimer;
        }
    }

    public void ConfirmChoice(PenaltyType choice)
    {
        if (choice == currentPlayInfo.penaltyType)
        {
            Debug.Log("True");
        }
        else
        {
            Debug.Log("False");
        }
        GameplayEvents.InitializePlay.Invoke();
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
                Debug.Log($"{currentPlayInfo.penaltyType}");
            }
        }
        moveDone = false;
        cameraDone = false;
    }

    private void Update()
    {
        if (!freezeManager)
        {
            if (moveDone && cameraDone)
            {
                ProgressCutscene();
            }

            if (playOngoing)
            {
                playTimer += Time.deltaTime;
                PlayCheck();
            }
        }
    }
    
    public void CallPrep()
    {
        Debug.Log($"Difference: {callTimestamp - currentPlayInfo.penaltyTimer}");
        penaltyCall = true;
    }

    private void PlayCheck()
    {
        if (playTimer > currentPlayInfo.penaltyTimer && !penaltyOccured)
        {
            penaltyOccured = true;
            playTest.SetActive(true);

        }

        if (playTimer > currentPlayInfo.penaltyTimer + currentPlayInfo.stopTimer)
        {
            playOngoing = false;
            GameplayEvents.EndPlay.Invoke();
        }
    }

    private void StartPlay()
    {
        InitiatePlayInformation();
        playTimer = 0;
        playOngoing = true;
    }

    //Creates the PlayInformation struct, and fills it.
    private void InitiatePlayInformation()
    {
        currentPlayInfo = new PlayInformation();
        currentPlayInfo.penaltyTimer = Random.Range(15f, 50f);
        currentPlayInfo.stopTimer = Random.Range(0, 15f);
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
        penaltyCall = false;
        callTimestamp = 0;

        playTest.SetActive(false);
    }

    private struct PlayInformation
    {
        //Penalty Timestamp denotes when the penalty will occur. Playstop indicates an additional timer added onto the Timestamp before the play will end (unless ended with whistle).
        public float penaltyTimer, stopTimer;
        //This contains the ID on the playerlist for the two players who will be involved in this incident.
        public int offenderId, affectedId;
        //What type of penalty it is is stored here.
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
