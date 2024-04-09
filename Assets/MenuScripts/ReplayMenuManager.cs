using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ReplayMenuManager : MonoBehaviour
{
    GameObject menuOptionPrefab;

    [SerializeField]
    GameObject replayParent;

    HockeyScenarioPositionData[] data;

    private void Start()
    {
        menuOptionPrefab = Resources.Load<GameObject>("MenuPrefabs/ReplayOption");
    }

    private void LoadData()
    {
        if (Directory.Exists("saveData"))
        {

        }
    }


}
