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
}

[Serializable]
public struct PenaltyData
{
    public string PenaltyName;
    public bool isEnabled;
}
[Serializable]
public struct StartingPosData
{
    public string posName;
    public bool isEnabled;
}