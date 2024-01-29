using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HockeyPlayerProperties : MonoBehaviour
{
    struct player
    {
        bool puckPossesion;
        team playerTeam;
        dominantHand dominantHand;
        float height;
        Animation currentAnimation;
    }
    enum team
    {
        teamRed,
        teamBlue,
    }
    enum dominantHand
    {
        leftHand,
        rightHand,
    }
}
