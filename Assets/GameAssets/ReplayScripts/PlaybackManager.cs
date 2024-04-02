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

    List<HockeyPlayerPositionData> positionData;
    GameObject[] players;

    [SerializeField]
    GameObject playerPrefab;

    float timer;
    public bool Playback {get; private set;}

    // Update is called once per frame
    void Start()
    {
        PositionSaver.LoadPlayerData(ReplaySettings.FILEPATH + readingSlot + "\\PositionData", ref positionData);
        TotalCount = positionData.Count;
        CreatePlayers();
        Playback = true;
    }

    void CreatePlayers()
    {
        players = new GameObject[10];

        for (int i = 0; i < players.Length; i++)
        {
            players[i] = Instantiate(playerPrefab, null);
            players[i].transform.position = new (positionData[0].x[i], positionData[0].y[i], positionData[0].z[i]);
            GameObject model = players[i].transform.Find("Model").gameObject;
            model.GetComponentInChildren<MeshRenderer>().material = GameUtilities.RetrieveHelmetSkin();
            model.GetComponentInChildren<SkinnedMeshRenderer>().material = GameUtilities.RetrievePlayerSkin(1, true);
        }
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
                InterpolatePosition(CurrentPosition);
            }

            if (CurrentPosition > positionData.Count)
            {
                Playback = false;
            }
        }
    }

    public void SetToPosition(int currentPosition)
    {
        for (int i = 0; i < positionData[currentPosition].x.Length; i++)
        {
            players[i].transform.position = new(positionData[currentPosition].x[i], positionData[currentPosition].y[i], positionData[currentPosition].z[i]);
        }
    }

    void InterpolatePosition(int currentPosition)
    {
        for (int i = 0; i < positionData[currentPosition].x.Length; i++)
        {
            Vector3 position1 = new(positionData[currentPosition].x[i], positionData[currentPosition].y[i], positionData[currentPosition].z[i]);
            Vector3 position2 = new(positionData[currentPosition + 1].x[i], positionData[currentPosition + 1].y[i], positionData[currentPosition + 1].z[i]);
            players[i].transform.position = Vector3.Lerp(position1, position2, timer / tickRate);
        }
    }

    public void TogglePlayback(bool toggle)
    {
        if (CurrentPosition <= positionData.Count)
        {
            Playback = toggle;
        }
    }
}
