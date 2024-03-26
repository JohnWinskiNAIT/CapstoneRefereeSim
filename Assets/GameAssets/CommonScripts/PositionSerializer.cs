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

    [SerializeField] bool savedOrLoaded = false;
    [SerializeField] bool playGhostData;
    float timer;
    const string FILEPATH = "SaveData\\PositionData";
    bool active;

    //makes object data into float data for saving.
    private void Start()
    {
        active = false;

        int slotCheck = 0;
        int breakCheck = 0;
        while (File.Exists(FILEPATH + saveSlot + "\\PositionData") && breakCheck < 100)
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
        active = true;
    }

    public void FixedUpdate()
    {
        if (active)
        {
            if (playGhostData)
            {
                GenerateGhostPlayers();
            }
            else
            {
                TrackPositionData();
            }
        }
    }

    public void TrackPositionData()
    {
        for (int i = 0; i < GameplayManager.Instance.currentPlayers.Length; i++)
        {
            objectPosition = GameplayManager.Instance.currentPlayers[i].transform.position;
            currentData.x[i] = objectPosition.x;
            currentData.y[i] = objectPosition.y;
            currentData.z[i] = objectPosition.z;
            positionData.Add(currentData);
        }


        if (Time.time > 30f + timer && !savedOrLoaded)
        {
            SavePositionData();
            savedOrLoaded = true;
        }
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
        {
            
            for (int i = 0; i < currentGhostPlayers.Length; i++)
            {
                currentData = positionData[index];
                objectPosition.x = currentData.x[i];
                objectPosition.y = currentData.y[i];
                objectPosition.z = currentData.z[i];
                currentGhostPlayers[i].transform.position = objectPosition;
            }
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
