using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class PositionSerializer : MonoBehaviour
{
    //Object to convert to save data.
    [SerializeField]
    GameObject[] convertableObject;

    Vector3 objectPosition;
    List<HockeyPlayerPositionData> positionData;
    HockeyPlayerPositionData currentData;

    int saveSlot;
    int index = 0;
    const string FILEPATH = "SaveData\\PositionData";

    //makes object data into float data for saving.
    public void FixedUpdate()
    {
        index++;
        if (0 == 10 % index)
        {
            for (int i = 0; i < convertableObject.Length; i++)
            {
                objectPosition = convertableObject[i].transform.position;
                currentData.x[i] = objectPosition.x;
                currentData.y[i] = objectPosition.y;
                currentData.z[i] = objectPosition.z;
                positionData.Add(currentData);
            }
        }
    }
    public void loadPositionData()
    {
        PositionSaver.LoadPlayerData(FILEPATH + saveSlot, ref positionData);
    }
    public void savePositionData()
    {
        CreatePlayerFileStructure();
        PositionSaver.SavePositionData(FILEPATH + saveSlot, ref positionData);
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
