using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaybackManager : MonoBehaviour
{
    [SerializeField]
    int readingSlot;

    public int TotalCount {get; private set;}
    public int CurrentPosition {get; private set;}
    float tickRate = ReplaySettings.Tick_Rate;

    HockeyScenarioPositionData scenarioData;
    GameObject[] players;
    GameObject referee;
    GameObject puck;

    [SerializeField]
    GameObject playerPrefab;

    float timer;
    public bool Playback {get; private set;}

    // Update is called once per frame
    void Start()
    {
        //PositionSaver.LoadPlayerData(ReplaySettings.FILEPATH + readingSlot + "\\PositionData", ref scenarioData);
        scenarioData = ReplaySettings.heldData;
        TotalCount = scenarioData.playerData.Count;
        CreatePlayers();
        Playback = true;
    }

    void CreatePlayers()
    {
        players = new GameObject[10];

        for (int i = 0; i < players.Length; i++)
        {
            players[i] = Instantiate(playerPrefab, null);
            players[i].transform.position = new (scenarioData.playerData[0].playerX[i], scenarioData.playerData[0].playerY[i], scenarioData.playerData[0].playerZ[i]);
            GameObject model = players[i].transform.Find("Model").gameObject;
            model.GetComponentInChildren<MeshRenderer>().material = GameUtilities.RetrieveHelmetSkin();
            model.GetComponentInChildren<SkinnedMeshRenderer>().material = GameUtilities.RetrievePlayerSkin(1, true);
        }

        referee = Instantiate(playerPrefab, null);
        referee.transform.position = new Vector3(scenarioData.refereePosition[0].x, scenarioData.refereePosition[0].y, scenarioData.refereePosition[0].z);
    }

    void Update()
    {
        if (Playback)
        {
            if (timer > tickRate)
            {
                CurrentPosition++;
                timer = 0;
            }
            else
            {
                timer += Time.deltaTime;
                if (CurrentPosition < scenarioData.playerData.Count - 1)
                {
                    InterpolatePosition(CurrentPosition);
                }
            }

            if (CurrentPosition > scenarioData.playerData.Count)
            {
                Playback = false;
            }
        }
    }

    public void SetToPosition(int currentPosition)
    {
        if (currentPosition < scenarioData.playerData.Count)
        {
            for (int i = 0; i < scenarioData.playerData[currentPosition].playerX.Length; i++)
            {
                players[i].transform.position = new(scenarioData.playerData[currentPosition].playerX[i], scenarioData.playerData[currentPosition].playerY[i], scenarioData.playerData[currentPosition].playerZ[i]);
            }

            referee.transform.position = new(scenarioData.refereePosition[currentPosition].x, scenarioData.refereePosition[currentPosition].y, scenarioData.refereePosition[currentPosition].z);

            CurrentPosition = currentPosition;
        }
    }

    void InterpolatePosition(int currentPosition)
    {
        for (int i = 0; i < scenarioData.playerData[currentPosition].playerX.Length; i++)
        {
            Vector3 position1 = new(scenarioData.playerData[currentPosition].playerX[i], scenarioData.playerData[currentPosition].playerY[i], scenarioData.playerData[currentPosition].playerZ[i]);
            Vector3 position2 = new(scenarioData.playerData[currentPosition + 1].playerX[i], scenarioData.playerData[currentPosition + 1].playerY[i], scenarioData.playerData[currentPosition + 1].playerZ[i]);
            players[i].transform.position = Vector3.Lerp(position1, position2, timer / tickRate);
        }
        
        Vector3 refPosition1 = new(scenarioData.refereePosition[currentPosition].x, scenarioData.refereePosition[currentPosition].y, scenarioData.refereePosition[currentPosition].z);
        Vector3 refPosition2 = new(scenarioData.refereePosition[currentPosition + 1].x, scenarioData.refereePosition[currentPosition + 1].y, scenarioData.refereePosition[currentPosition + 1].z);
        referee.transform.position = Vector3.Lerp(refPosition1, refPosition2, timer / tickRate);
    }

    public void TogglePlayback(bool toggle)
    {
        if (CurrentPosition <= scenarioData.playerData.Count)
        {
            Playback = toggle;
        }
    }
}
