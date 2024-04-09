using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;

public class ReplayMenuManager : MonoBehaviour
{
    const string FILEPATH = "SaveData";
    const string PositionDataString = "PositionData";
    const string DATAPATH = "SaveData/PositionData";

    GameObject menuOptionPrefab;

    [SerializeField]
    GameObject replayParent;

    List<HockeyScenarioPositionData> data;
    GameObject[] replayButtons;

    private void Start()
    {
        menuOptionPrefab = Resources.Load<GameObject>("MenuPrefabs/ReplayOption");
        LoadData();
        CreateButtons();
    }

    private void LoadData()
    {
        if (Directory.Exists(FILEPATH))
        {
            string[] directoryZone = Directory.GetDirectories(FILEPATH);
            data = new List<HockeyScenarioPositionData>();
            Debug.Log(directoryZone.Length);

            for (int i = 0; i < directoryZone.Length; i++)
            {
                if (directoryZone[i].Contains(PositionDataString))
                {
                    data.Add(Deserialize(i));
                }
            }
        }
    }

    private void CreateButtons()
    {
        replayButtons = new GameObject[data.Count];

        for (int i = 0; i < data.Count; i++)
        {
            replayButtons[i] = Instantiate(menuOptionPrefab, replayParent.transform);
            replayButtons[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Replay Test";
            replayButtons[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = DateTime.Now.ToString();
        }
    }

    private HockeyScenarioPositionData Deserialize(int id)
    {
        HockeyScenarioPositionData data;
        XmlSerializer serializer = new XmlSerializer(typeof(HockeyScenarioPositionData));
        Stream stream = File.Open(DATAPATH + $"{id}/" + PositionDataString, FileMode.Open);
        data = (HockeyScenarioPositionData)serializer.Deserialize(stream);

        return data;
    }
}
