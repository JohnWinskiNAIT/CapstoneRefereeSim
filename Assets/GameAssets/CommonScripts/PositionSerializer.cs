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

    [SerializeField]
    int saveSlot;
    int index = 0;
    bool saved = false;
    float timer;
    const string FILEPATH = "SaveData\\PositionData";

    //makes object data into float data for saving.
    private void Start()
    {
        positionData = new List<HockeyPlayerPositionData>();
        currentData = new HockeyPlayerPositionData();
        currentData.x = new float[10];
        currentData.y = new float[10];
        currentData.z = new float[10];
        timer = Time.time;

    }
    public void FixedUpdate()
    {
        //index++;
        //if (0 == 10 % index)
        {
            for (int i = 0; i < GameplayManager.Instance.currentPlayers.Length; i++)
            {
                objectPosition = GameplayManager.Instance.currentPlayers[i].transform.position;
                currentData.x[i] = objectPosition.x;
                currentData.y[i] = objectPosition.y;
                currentData.z[i] = objectPosition.z;
                positionData.Add(currentData);
            }
        }

        if (Time.time > 30f + timer && !saved)
        {
            savePositionData();
            saved= true;
        }
    }
    public void loadPositionData()
    {
        PositionSaver.LoadPlayerData(FILEPATH + saveSlot + "\\PositionData", ref positionData);
    }
    public void savePositionData()
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
}

[Serializable]
public struct HockeyPlayerPositionData
{
    public float[] x;
    public float[] y;
    public float[] z;
}
