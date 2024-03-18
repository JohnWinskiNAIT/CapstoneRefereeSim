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
    CutsceneData playEndCutscene, puckDropCutscene;

    CutsceneData offsetPuckDropCutscene;

    [SerializeField]
    FaceoffData[] rinkFaceoffs;
    public FaceoffData CurrentFaceoff { get; private set; }

    CutsceneData currentCutscene;
    PlayInformation currentPlayInfo;

    int cutsceneStatus;
    float playTimer, callTimestamp;

    public bool cameraDone, moveDone;
    bool playOngoing, penaltyOccured, penaltyCall;

    bool freezeManager;
    GameObject[] currentPlayers;

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
    }

    private void Start()
    {
        SelectFaceoff();
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
        if (cutsceneStatus < currentCutscene.NumberOfPoints)
        {
            GameplayEvents.CutsceneTrigger.Invoke(cutsceneStatus);
            if (currentCutscene.pointTypes[cutsceneStatus] == CutsceneData.PointType.WheelOpen)
            {
                GameplayEvents.OpenWheel.Invoke(true);
                Debug.Log($"{currentPlayInfo.penaltyType}");
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
        SelectFaceoff();
        UpdatePlayers();
        InitiatePlayInformation();
        offsetPuckDropCutscene = puckDropCutscene;
        offsetPuckDropCutscene.PuckDrop(CurrentFaceoff.unscaledOffset);
        GameplayEvents.LoadCutscene.Invoke(offsetPuckDropCutscene);
        currentCutscene = offsetPuckDropCutscene;
        cutsceneStatus = 0;
        
        playTimer = 0;
        playOngoing = true;
        GameplayEvents.CutsceneTrigger.Invoke(cutsceneStatus);
    }

    //Creates the PlayInformation struct, and fills it.
    private void InitiatePlayInformation()
    {
        currentPlayInfo = new()
        {
            penaltyTimer = Random.Range(15f, 50f),
            stopTimer = Random.Range(0, 15f)
        };
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
