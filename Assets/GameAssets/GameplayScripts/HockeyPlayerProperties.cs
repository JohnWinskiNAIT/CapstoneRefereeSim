using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HockeyPlayerProperties : MonoBehaviour
{
    public player CreatePlayer()
    {
        return new player { };
    }
}
[Serializable]
public struct player
{
    public bool puckPossesion;
    public int jerseyNumber;
    public team playerTeam;
    public dominantHand dominantHand;
    public float height;
    public Animation currentAnimation;
}
public enum team
{
    teamRed,
    teamBlue,
}
public enum dominantHand
{
    leftHand,
    rightHand,
}

