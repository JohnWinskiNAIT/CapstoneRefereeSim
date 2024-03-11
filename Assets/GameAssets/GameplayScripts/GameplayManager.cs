using Oculus.VoiceSDK.UX;
using System;
using System.Collections;
using System.Collections.Generic;
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
    bool[] enabledPlayers;
    [SerializeField]
    PlayerSetup[] setupInformation;
    [SerializeField]
    GameObject zoneParent;

    [SerializeField]
    GameObject playerPrefab;
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

    private void Start()
    {
        GeneratePlayers();
    }

    void GeneratePlayers()
    {
        GameObject createdPlayer;
        for (int i = 0; i < enabledPlayers.Length; i++)
        {
            if (enabledPlayers[i])
            {
                createdPlayer = Instantiate(playerPrefab, null);
                createdPlayer.transform.position = setupInformation[i].startingPosition;
                createdPlayer.GetComponent<ZoneAIController>().SetupAIAttributes(setupInformation[i].type, setupInformation[i].team, zoneParent, setupInformation[i].startingPosition);
            }
        }
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

    [Serializable]
    private struct PlayerSetup
    {
        public ZoneAIController.AITeam team;
        public ZoneAIController.AIType type;
        public Vector3 startingPosition;
    }
}

[Serializable]
public class CutsceneData
{
    public int numberOfPoints;
    public GameObject waypointParent;
    public GameObject cameraParent;
    public bool wheelOpen;

    Vector3[] waypoints;
    Vector3[] cameraPoints;
    public PointType[] pointTypes;

    public enum PointType
    {
        Movement,
        WheelOpen,
        PuckdropInput
    }
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
