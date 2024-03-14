using System;
using UnityEngine;

[Serializable]
public struct SettingsData
{
    public float masterVolume;
    public float SFXvolume;
    public float ambientVolume;
    public PenaltyData[] penalties;
}

[Serializable]
public struct PenaltyData
{
    public string penaltyName;
    public Sprite penaltyIcon;
    public bool isEnabled;
}