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
    List<HockeyPlayerPositionData> positionData;
    [SerializeField] HockeyPlayerPositionData currentData;

    GameObject[] currentGhostPlayers;

    int saveSlot;
    int index = 0;

    bool savedOrLoaded;
    bool playGhostData;
    float timer;
    const string FILEPATH = "SaveData\\PositionData";
    private bool active;

    //makes object data into float data for saving.
    private void Start()
    {
        savedOrLoaded = false;
        active = false;
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
        positionData = new List<HockeyPlayerPositionData>();
        currentData = new HockeyPlayerPositionData();
        currentData.x = new float[10];
        currentData.y = new float[10];
        currentData.z = new float[10];
        timer = Time.time;

        active = true;
        playGhostData = false;
    }

    public void InitiatePlayback(GameObject[] ghostPlayers)
    {
        //Same as above but set up for replay with a list of players.
        currentGhostPlayers = ghostPlayers;
    }

    public void Update()
    {
        if (!GameplayManager.Instance.ManagerPaused)
        {
            if (active)
            {
                if (playGhostData)
                {
                    GenerateGhostPlayers();
                }
                else
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
            else
            {
                if (positionData != null && !active)
                {
                    active = true;
                }
            }
        }
    }

    public void TrackPositionData()
    {
        for (int i = 0; i < GameplayManager.Instance.currentPlayers.Length; i++)
        {
            if (GameplayManager.Instance.currentPlayers[i] != null)
            {
                objectPosition = GameplayManager.Instance.currentPlayers[i].transform.position;
                currentData.x[i] = objectPosition.x;
                currentData.y[i] = objectPosition.y;
                currentData.z[i] = objectPosition.z;
            }
        }
        positionData.Add(currentData);
        Debug.Log(positionData[positionData.Count - 1].x[0]);
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
        PositionSaver.LoadPlayerData(FILEPATH + saveSlot + "\\PositionData", ref positionData);
    }
    public void SavePositionData()
    {
        CreatePlayerFileStructure();
        PositionSaver.SavePositionData(FILEPATH + saveSlot + "\\PositionData", ref positionData);
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
    public void GenerateGhostPlayers()
    {
        if (!savedOrLoaded)
        {
            LoadPositionData();
            savedOrLoaded = true;
        }
        //foreach (HockeyPlayerPositionData positionData in positionData)

        for (int i = 0; i < currentGhostPlayers.Length; i++)
        {
            currentData = positionData[index];
            objectPosition.x = currentData.x[i];
            objectPosition.y = currentData.y[i];
            objectPosition.z = currentData.z[i];
            currentGhostPlayers[i].transform.position = objectPosition;
        }
        index++;
    }
}

[Serializable]
public struct HockeyPlayerPositionData
{
    public float[] x;
    public float[] y;
    public float[] z;
}
