using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReplayMenuManager : MonoBehaviour
{
    const int FileHeight = 150;
    const string FILEPATH = "SaveData";
    const string PositionDataString = "PositionData";

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

            for (int i = 0; i < directoryZone.Length; i++)
            {
                if (directoryZone[i].Contains(PositionDataString))
                {
                    data.Add(Deserialize(directoryZone[i]));
                }
            }
        }
    }

    private void CreateButtons()
    {
        replayButtons = new GameObject[data.Count];
        replayParent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, FileHeight * data.Count);

        //UnityAction callback = () => LoadReplayPlayback(i + 1);

        for (int i = 0; i < data.Count; i++)
        {
            replayButtons[i] = Instantiate(menuOptionPrefab, replayParent.transform);
            replayButtons[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = data[i].replayName;
            replayButtons[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = data[i].replayTime.ToString();
            int test = i;
            replayButtons[i].GetComponent<Button>().onClick.AddListener(() => { LoadReplayPlayback(test); });
        }
    }

    public void LoadReplayPlayback(int id)
    {
        ReplaySettings.heldData = data[id];
        SceneManager.LoadScene("ReplayPlaybackScene");
    }

    private HockeyScenarioPositionData Deserialize(string name)
    {
        HockeyScenarioPositionData data;
        XmlSerializer serializer = new XmlSerializer(typeof(HockeyScenarioPositionData));
        Stream stream = File.Open($"{name}/" + PositionDataString, FileMode.Open);
        data = (HockeyScenarioPositionData)serializer.Deserialize(stream);

        return data;
    }
}
