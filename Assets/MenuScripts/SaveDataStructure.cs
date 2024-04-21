using System;
using Unity.VisualScripting;
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
                if (penalties[i].isWhistle && penalties[i].isEnabled)
                {
                    count++;
                }
            }
            return count;
        }
    }

    public int EnabledCount
    {
        get
        {
            int count = 0;
            for (int i = 0; i < penalties.Length; i++)
            {
                if (penalties[i].isEnabled)
                {
                    count++;
                }
            }
            return count;
        }
    }

    public PenaltyData FindPenaltyId(int id)
    {
        PenaltyData foundPenalty = penalties[0];
        bool done = false;
        for (int i = 0; i < penalties.Length && !done; i++)
        {
            if (penalties[i].penaltyId == id)
            {
                foundPenalty = penalties[i];
                done = true;
            }
        }
        return foundPenalty;
    }
}

[Serializable]
public struct PenaltyData
{
    public string PenaltyName;
    public string penaltyText;
    public int penaltyId;
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