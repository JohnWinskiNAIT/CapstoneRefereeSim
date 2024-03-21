using System;
using UnityEngine;

[Serializable]
public struct SettingsData
{
    public int scenarios;
    public int screenMode;
    public float masterVolume;
    public float SFXvolume;
    public float ambientVolume;
    public float lastMasterVolume;
    public float lastSFXvolume;
    public float lastAmbientVolume;
    public PenaltyData[] penalties;
    public StartingPosData[] startingPos;

    public int WhistleCount
    {
        get
        {
            int count = 0;
            for (int i = 0; i < penalties.Length; i++)
            {
                if (penalties[i].isWhistle)
                {
                    count++;
                }
            }
            return count;
        }
    }
}

[Serializable]
public struct PenaltyData
{
    public string PenaltyName;
    public string penaltyText;
    public bool isEnabled;
    public bool isWhistle;

    public Sprite RefereeSprite
    {
        get { return Resources.Load<Sprite>(Settings.RefereeSpritePath + PenaltyName); }
    }
}
[Serializable]
public struct StartingPosData
{
    public string posName;
    public bool isEnabled;
}