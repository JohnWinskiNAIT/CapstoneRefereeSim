using Oculus.VoiceSDK.UX;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class GameplayManager : MonoBehaviour
{
    static public GameplayManager Instance;

    public SettingsData mySettings;
    [SerializeField]
    CutsceneData playEndCutscene, puckDropCutscene;

    CutsceneData offsetPuckDropCutscene;

    [SerializeField]
    FaceoffData[] rinkFaceoffs;
    public FaceoffData CurrentFaceoff { get; private set; }

    CutsceneData currentCutscene;
    public PlayInformation CurrentPlayInfo { get; private set; }

    PositionSerializer recorder;

    int scenariosCompleted;
    int cutsceneStatus;
    float playTimer, callTimestamp, callDifference;

    public bool cameraDone, moveDone;
    bool playOngoing, penaltyMovement, penaltyOccured;

    bool freezeManager;
    public GameObject[] currentPlayers;

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

    public CallState Call {get; private set;}

    public enum GameState
    {
        Ice,
        Results
    }

    public enum CallState
    {
        None,
        Call,
        Whistle
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

        scenariosCompleted = 0;
        mySettings = Settings.mySettings;
        recorder = GetComponent<PositionSerializer>();
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
                currentPlayers[i].GetComponent<ZoneAIController>().SetupAIAttributes(setupInformation[i].type, setupInformation[i].team, zoneParent);
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
                currentPlayers[i].transform.rotation = Quaternion.LookRotation(CurrentFaceoff.unscaledOffset - currentPlayers[i].transform.position);
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
            callDifference = callTimestamp - CurrentPlayInfo.penaltyTimer;

            if (callDifference < 0)
            {
                CurrentPlayInfo.WipeId();
            }
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

    public bool TypeOfWheel()
    {
        bool returnBool = true;

        if (Call != CallState.Whistle)
        {
            returnBool = false;
        }

        return returnBool;
    }

    public void SetCallTimer()
    {
        if (callTimestamp == 0)
        {
            callTimestamp = playTimer;
        }
    }

    public void ConfirmChoice(int choice)
    {
        if (choice == CurrentPlayInfo.penaltyId)
        {
            Debug.Log("True");
        }
        else
        {
            Debug.Log("False");
        }

        Debug.Log(CurrentPlayInfo.penaltyId);

        resultsUI.GetComponent<ResultsDisplay>().InitiateResults(choice, CurrentPlayInfo.penaltyId, callDifference);
        recorder.EndRecording();
        resultsUI.SetActive(true);
        GameplayEvents.OpenWheel.Invoke(false, false);
    }

    public void ProgressCutscene()
    {
        cutsceneStatus++;
        if (cutsceneStatus < currentCutscene.NumberOfPoints)
        {
            GameplayEvents.CutsceneTrigger.Invoke(cutsceneStatus);
            if (currentCutscene.pointTypes[cutsceneStatus] == CutsceneData.PointType.WheelOpen)
            {
                GameplayEvents.OpenWheel.Invoke(true, TypeOfWheel());
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
    
    public void CallPrep(bool isWhistle)
    {
        if (isWhistle)
        {
            Call = CallState.Whistle;
        }
        else
        {
            Call = CallState.Call;
        }
    }

    public void SaveRecording()
    {
        recorder.SaveRecording();
    }

    private void PlayCheck()
    {
        Vector3 distance = (currentPlayers[CurrentPlayInfo.offenderId].transform.position - currentPlayers[CurrentPlayInfo.affectedId].transform.position) / 2;
        float speed = currentPlayers[CurrentPlayInfo.offenderId].GetComponent<ZoneAIController>().maxSpeed;
        float tValue = distance.magnitude / speed;

        if (playTimer + tValue >= CurrentPlayInfo.penaltyTimer && !penaltyMovement)
        {
            currentPlayers[CurrentPlayInfo.offenderId].GetComponent<ZoneAIController>().MoveTowardsTarget(-distance * 0.9f, tValue);
            currentPlayers[CurrentPlayInfo.affectedId].GetComponent<ZoneAIController>().MoveTowardsTarget(distance * 0.9f, tValue);
            penaltyMovement = true;
        }

        if (playTimer > CurrentPlayInfo.penaltyTimer && !penaltyOccured)
        {
            currentPlayers[CurrentPlayInfo.offenderId].GetComponent<ZoneAIController>().ResolvePenalty(false);
            currentPlayers[CurrentPlayInfo.affectedId].GetComponent<ZoneAIController>().ResolvePenalty(true);
            playTest.SetActive(true);
            Debug.Log($"{CurrentPlayInfo.penaltyId}");
            penaltyOccured = true;
        }

        if (playTimer > CurrentPlayInfo.penaltyTimer + CurrentPlayInfo.stopTimer)
        {
            playOngoing = false;
            GameplayEvents.EndPlay.Invoke();
        }
    }

    private void StartPlay()
    {
        scenariosCompleted++;
        if (scenariosCompleted > Settings.mySettings.scenarios)
        {
            SceneManager.LoadScene("MenuScene");
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }

        if (resultsUI.activeSelf)
        {
            resultsUI.SetActive(false);
        }

        SelectFaceoff();
        UpdatePlayers();
        InitiatePlayInformation();
        offsetPuckDropCutscene.OverwriteCutscene(puckDropCutscene);
        offsetPuckDropCutscene.PuckDrop(CurrentFaceoff.unscaledOffset);
        GameplayEvents.LoadCutscene.Invoke(offsetPuckDropCutscene);
        currentCutscene = offsetPuckDropCutscene;
        cutsceneStatus = 0;

        Call = CallState.None;
        playTimer = 0;
        penaltyMovement = false;
        penaltyOccured = false;

        playOngoing = true;
        recorder.InitiateRecording();
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
            penaltyId = mySettings.penalties[Random.Range(0, Settings.mySettings.penalties.Length)].penaltyId
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
        //What type of penalty it is is stored here
        public int penaltyId;
        
        //Used for calls that are too abrupt and early.
        public void WipeId()
        {
            penaltyId = -1;
        }
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
