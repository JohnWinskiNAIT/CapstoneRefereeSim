using Oculus.VoiceSDK.UX;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameplayManager : MonoBehaviour
{
    static public GameplayManager Instance;

    public GameObject SettingsContainer;
    public SettingsData mySettings;
    [SerializeField]
    CutsceneData playEndCutscene, puckDropCutscene;

    CutsceneData offsetPuckDropCutscene;

    [SerializeField]
    FaceoffData[] rinkFaceoffs;
    public FaceoffData CurrentFaceoff { get; private set; }
    GameState gameplayState;

    CutsceneData currentCutscene;
    public PlayInformation CurrentPlayInfo { get; private set; }

    int cutsceneStatus;
    float playTimer, callTimestamp;
    float? activationTime;

    public bool cameraDone, moveDone;
    bool playOngoing, penaltyOccured;

    bool freezeManager;
    GameObject[] currentPlayers;

    public GameObject playerUI;
    public GameObject resultsUI;

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

    public enum GameState
    {
        Ice,
        Results
    }

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
    }

    private void Start()
    {
        offsetPuckDropCutscene = new();

        gameplayState = GameState.Ice;
        mySettings = Settings.mySettings;
        Debug.Log(mySettings.penalties[1].isWhistle);
        SelectFaceoff();
        EnablePlayers();
        GeneratePlayers();
        GameplayEvents.InitializePlay.Invoke();
    }

    void GeneratePlayers()
    {
        currentPlayers = new GameObject[enabledPlayers.Length];
        for (int i = 0; i < enabledPlayers.Length; i++)
        {
            if (enabledPlayers[i])
            {
                currentPlayers[i] = Instantiate(playerPrefab, null);
                if (CurrentFaceoff.isCentre)
                {
                    currentPlayers[i].transform.position = setupInformation[i].centrePosition + CurrentFaceoff.unscaledOffset;
                    currentPlayers[i].GetComponent<ZoneAIController>().SetupAIAttributes(setupInformation[i].type, setupInformation[i].team, zoneParent, setupInformation[i].centrePosition + CurrentFaceoff.unscaledOffset);
                }
                else
                {
                    currentPlayers[i].transform.position = setupInformation[i].otherPosition + CurrentFaceoff.unscaledOffset;
                    currentPlayers[i].GetComponent<ZoneAIController>().SetupAIAttributes(setupInformation[i].type, setupInformation[i].team, zoneParent, setupInformation[i].otherPosition + CurrentFaceoff.unscaledOffset);
                }
            }
        }
    }
    void EnablePlayers()
    {
        for(int i = 0; i < enabledPlayers.Length; i++)
        {
            enabledPlayers[i] = mySettings.startingPos[i].isEnabled;
        }
    }
    void UpdatePlayers()
    {
        for (int i = 0; i < currentPlayers.Length; i++)
        {
            if (currentPlayers[i] != null)
            {
                if (CurrentFaceoff.isCentre)
                {
                    currentPlayers[i].transform.position = setupInformation[i].centrePosition + CurrentFaceoff.unscaledOffset;
                }
                else
                {
                    currentPlayers[i].transform.position = setupInformation[i].otherPosition + CurrentFaceoff.unscaledOffset;
                }
            }
        }
    }

    void SelectFaceoff()
    {
        int selectedId = Random.Range(0, rinkFaceoffs.Length);
        // for or while loop here to check if the selectedId is enabled

        CurrentFaceoff = rinkFaceoffs[selectedId];
    }

    // Update is called once per frame
    public void EndPlay()
    {
        if (playOngoing)
        {
            playOngoing = false;
            Debug.Log($"Difference: {callTimestamp - CurrentPlayInfo.penaltyTimer}");
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
        if (choice == CurrentPlayInfo.penaltyType)
        {
            Debug.Log("True");
        }
        else
        {
            Debug.Log("False");
        }

        SelectFaceoff();
        gameplayState = GameState.Results;
        resultsUI.gameObject.SetActive(true);
        //GameplayEvents.InitializePlay.Invoke();
    }

    public void ProgressCutscene()
    {
        cutsceneStatus++;
        if (cutsceneStatus < currentCutscene.NumberOfPoints)
        {
            GameplayEvents.CutsceneTrigger.Invoke(cutsceneStatus);
            if (currentCutscene.pointTypes[cutsceneStatus] == CutsceneData.PointType.WheelOpen)
            {
                GameplayEvents.OpenWheel.Invoke(true);
                Debug.Log($"{CurrentPlayInfo.penaltyType}");
            }

            moveDone = false;
            cameraDone = false;
        }
        else
        {
            GameplayEvents.EndCutscene.Invoke();
            currentCutscene = null;
        }
    }

    private void Update()
    {
        if (!freezeManager)
        {
            if (moveDone && cameraDone && currentCutscene != null)
            {
                ProgressCutscene();
            }

            if (playOngoing && currentCutscene == null)
            {
                playTimer += Time.deltaTime;
                PlayCheck();
            }
        }
    }
    
    public void CallPrep()
    {
        Debug.Log($"Difference: {callTimestamp - CurrentPlayInfo.penaltyTimer}");
    }

    private void PlayCheck()
    {
        if (playTimer > CurrentPlayInfo.penaltyTimer - 10f && activationTime == null)
        {
            Vector3 distance = currentPlayers[CurrentPlayInfo.offenderId].transform.position - currentPlayers[CurrentPlayInfo.affectedId].transform.position;
            Debug.Log(distance.magnitude);
            activationTime = distance.magnitude;
        }

        if (playTimer > CurrentPlayInfo.penaltyTimer && !penaltyOccured)
        {
            penaltyOccured = true;
            playTest.SetActive(true);
        }

        if (playTimer > CurrentPlayInfo.penaltyTimer + CurrentPlayInfo.stopTimer)
        {
            playOngoing = false;
            GameplayEvents.EndPlay.Invoke();
        }
    }

    private void StartPlay()
    {
        UpdatePlayers();
        InitiatePlayInformation();
        offsetPuckDropCutscene.OverwriteCutscene(puckDropCutscene);
        offsetPuckDropCutscene.PuckDrop(CurrentFaceoff.unscaledOffset);
        GameplayEvents.LoadCutscene.Invoke(offsetPuckDropCutscene);
        currentCutscene = offsetPuckDropCutscene;
        cutsceneStatus = 0;
        activationTime = null;

        //Debug.Log(CurrentFaceoff.faceoffName);
        
        playTimer = 0;
        playOngoing = true;
        GameplayEvents.CutsceneTrigger.Invoke(cutsceneStatus);
    }

    //Creates the PlayInformation struct, and fills it.
    private void InitiatePlayInformation()
    {
        //Replace this random range with a reference to a list of all players in the scene.
        bool playersExist = false;

        for (int i = 0; i < currentPlayers.Length && !playersExist; i++)
        {
            if (enabledPlayers[i])
            {
                playersExist = true;
            }
        }

        int player1 = Random.Range(0, currentPlayers.Length / 2);
        while (!enabledPlayers[player1] && playersExist)
        {
            player1 = Random.Range(0, currentPlayers.Length / 2);
        }
        int player2 = Random.Range(currentPlayers.Length / 2, currentPlayers.Length);
        while (!enabledPlayers[player2] && playersExist)
        {
            player2 = Random.Range(currentPlayers.Length / 2, currentPlayers.Length);
        }

        CurrentPlayInfo = new()
        {
            penaltyTimer = Random.Range(15f, 50f),
            stopTimer = Random.Range(5f, 15f),
            offenderId = player1,
            affectedId = player2,
            penaltyType = (PenaltyType)Random.Range(0, Enum.GetNames(typeof(PenaltyType)).Length)
        };

        callTimestamp = 0;

        playTest.SetActive(false);
    }

    public struct PlayInformation
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
        public Vector3 centrePosition;
        public Vector3 otherPosition;
    }
}

[Serializable]
public class CutsceneData
{
    public int NumberOfPoints
    {
        get { return waypoints.Length; }
    }

    public Vector3[] waypoints;
    public Vector3[] cameraPoints;
    public PointType[] pointTypes;

    public void PuckDrop(Vector3 puckPosition)
    {
        for (int i = 0; i < cameraPoints.Length; i++)
        {
            cameraPoints[i] = new(puckPosition.x, cameraPoints[i].y, puckPosition.z);
        }

        for (int i = 0; i < waypoints.Length; i++)
        {
            if (puckPosition.x < 0)
            {
                waypoints[i] = new(waypoints[i].x * -1, waypoints[i].y, waypoints[i].z);
            }
            waypoints[i] += puckPosition;
        }
    }

    public void OverwriteCutscene(CutsceneData overwritingData)
    {
        waypoints = new Vector3[overwritingData.NumberOfPoints];
        Array.Copy(overwritingData.waypoints, waypoints, overwritingData.waypoints.Length);
        cameraPoints = new Vector3[overwritingData.NumberOfPoints];
        Array.Copy(overwritingData.cameraPoints, cameraPoints, overwritingData.cameraPoints.Length);
        pointTypes = new PointType[overwritingData.NumberOfPoints];
        Array.Copy(overwritingData.pointTypes, pointTypes, overwritingData.pointTypes.Length);
    }

    public enum PointType
    {
        Movement,
        WheelOpen,
        PuckdropInput,
        Teleport
    }
}

[Serializable]
public class FaceoffData
{
    public int faceoffId;
    public string faceoffName;
    public Vector3 unscaledOffset;
    public Vector3 playerOffset1;
    public Vector3 playerOffset2;
    public bool isCentre;
}

public class ResultData
{
    public float timerDifference;
    public float chosenPenalty;
    public float occuredPenalty;
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
