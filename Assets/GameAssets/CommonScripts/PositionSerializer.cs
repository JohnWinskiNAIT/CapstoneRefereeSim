using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PositionSerializer : MonoBehaviour
{
    //Object to convert to save data.
    [SerializeField]
    GameObject convertableObject;

    Vector3 objectPosition;
    PositionData positionData;
    int saveSlot;

    const string FILEPATH = "SaveData\\PositionData";

    //makes object data into float data for saving.
    public void ConvertObject(GameObject[] passedObjects)
    {
        for (int i = 0; i < passedObjects.Length; i++)
        {
            convertableObject = passedObjects[i];
            objectPosition = convertableObject.transform.position;
            positionData.x[i] = objectPosition.x;
            positionData.y[i] = objectPosition.y;
            positionData.z[i] = objectPosition.z;
        }
    }
    public void loadPositionData()
    {
        PositionSaver.LoadPlayerData(FILEPATH + saveSlot, ref positionData);
    }
    public void savePositionData()
    {
        PositionSaver.SavePositionData(FILEPATH + saveSlot, ref positionData);
    }
}

[Serializable]
public struct PositionData
{
    public float[] x;
    public float[] y;
    public float[] z;
}
