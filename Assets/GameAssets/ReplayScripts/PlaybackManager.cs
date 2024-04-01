using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaybackManager : MonoBehaviour
{
    int readingSlot;
    int totalCount;
    int currentPosition;
    float tickRate = ReplaySettings.Tick_Rate;

    List<HockeyPlayerPositionData> positionData;
    GameObject[] players;

    [SerializeField]
    GameObject playerPrefab;

    float timer;
    bool playback;

    // Update is called once per frame
    void Start()
    {
        readingSlot = 1;
        PositionSaver.LoadPlayerData(ReplaySettings.FILEPATH + readingSlot + "\\PositionData", ref positionData);
        CreatePlayers();
        playback = true;
    }

    void CreatePlayers()
    {
        players = new GameObject[10];

        for (int i = 0; i < players.Length; i++)
        {
            players[0] = Instantiate(playerPrefab, null);
            players[0].transform.position = new (positionData[0].x[i], positionData[0].y[i], positionData[0].z[i]);
            players[0].GetComponentInChildren<MeshRenderer>().material = GameUtilities.RetrievePlayerSkin(0);
        }
    }

    void Update()
    {
        if (playback)
        {
            if (timer > tickRate)
            {
                currentPosition++;
                timer = 0;
            }
            else
            {
                timer += Time.deltaTime;
                InterpolatePosition(currentPosition);
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
}
