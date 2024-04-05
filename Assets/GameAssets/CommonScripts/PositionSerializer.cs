using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class PositionSerializer : MonoBehaviour
{
    //Object to convert to save data.
    Vector3 objectPosition;

    HockeyPlayerPositionData currentPositions;
    HockeyScenarioPositionData scenarioData;

    int saveSlot;
    int index = 0;

    bool savedOrLoaded;
    bool playGhostData;
    float timer;
    const string FILEPATH = "SaveData\\PositionData";
    [SerializeField]
    private bool active;

    //makes object data into float data for saving.
    private void Start()
    {
        savedOrLoaded = false;
        playGhostData = false;

        int slotCheck = 0;
        int breakCheck = 0;
        timer = 0;
        while (File.Exists(FILEPATH + slotCheck + "\\PositionData") && breakCheck < 100)
        {
            slotCheck++;
            breakCheck++;
        }
        saveSlot = slotCheck;
    }

    public void InitiateRecording()
    {
        currentPositions = new()
        {
            playerX = new float[10],
            playerY = new float[10],
            playerZ = new float[10]
        };

        timer = Time.time;

        active = true;
        playGhostData = false;

        scenarioData = new()
        {
            playerData = new List<HockeyPlayerPositionData>(),
            puckPosition = new List<Vector3>(),
            refereePosition = new List<Vector3>()
        };
    }

    public void Update()
    {
        if (!GameplayManager.Instance.ManagerPaused)
        {
            if (active)
            {
                if (timer > ReplaySettings.Tick_Rate)
                {
                    TrackPositionData();
                    timer = 0;
                }
                else
                {
                    timer += Time.deltaTime;
                }
            }
        }
    }

    public void TrackPositionData()
    {
        HockeyPlayerPositionData newData = new();
        newData.playerX = new float[10];
        newData.playerY = new float[10];
        newData.playerZ = new float[10];

        for (int i = 0; i < GameplayManager.Instance.currentPlayers.Length; i++)
        {
            if (GameplayManager.Instance.currentPlayers[i] != null)
            {
                objectPosition = GameplayManager.Instance.currentPlayers[i].transform.position;
                newData.playerX[i] = objectPosition.x;
                newData.playerY[i] = objectPosition.y;
                newData.playerZ[i] = objectPosition.z;
            }
        }

        scenarioData.refereePosition.Add(GameplayManager.Instance.player.transform.position);
        scenarioData.puckPosition.Add(GameplayManager.Instance.puck.transform.position);
        scenarioData.playerData.Add(newData);
    }

    public void EndRecording()
    {
        active = false;
    }

    public void SaveRecording()
    {
        SavePositionData();
        savedOrLoaded = true;
    }

    public void LoadPositionData()
    {
        PositionSaver.LoadPlayerData(FILEPATH + saveSlot + "\\PositionData", ref scenarioData);
    }
    public void SavePositionData()
    {
        CreatePlayerFileStructure();
        PositionSaver.SavePositionData(FILEPATH + saveSlot + "\\PositionData", ref scenarioData);
    }
    void CreatePlayerFileStructure()
    {
        // Determine whether the directory exists.
        if (Directory.Exists(FILEPATH + saveSlot))
        {
            Debug.Log("Folder structure already exists");
        }
        else
        {
            // Try to create the directory.
            Directory.CreateDirectory(FILEPATH + saveSlot);
        }
    }
}

[Serializable]
public struct HockeyPlayerPositionData
{
    public float[] playerX;
    public float[] playerY;
    public float[] playerZ;
}

public class HockeyScenarioPositionData
{
    public List<HockeyPlayerPositionData> playerData;

    public bool[] playerEnabled;
    public int[] playerSkins;

    public List<Vector3> puckPosition;
    public List<Vector3> refereePosition;
}
