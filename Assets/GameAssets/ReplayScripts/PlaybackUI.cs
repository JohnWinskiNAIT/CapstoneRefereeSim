using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlaybackUI : MonoBehaviour
{
    Button stopStartButton;
    Image stopStartIcon;
    Slider playbackBar;

    int readingSlot;
    int totalCount;
    int currentPosition;
    float tickRate = ReplaySettings.Tick_Rate;

    List<HockeyPlayerPositionData> positionData;

    List<GameObject> players;

    // Start is called before the first frame update
    void Start()
    {
        PositionSaver.LoadPlayerData(ReplaySettings.FILEPATH + readingSlot + "\\PositionData", ref positionData);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < positionData.Count; i++)
        {

        }
    }

    void SetToPosition(int incomingPosition)
    {
        for (int j = 0; j < positionData[incomingPosition].x.Length; j++)
        {
            players[i].transform.position = new(positionData[incomingPosition].x[j], positionData[incomingPosition].y[j], positionData[incomingPosition].z[j]);
        }
    }
}
