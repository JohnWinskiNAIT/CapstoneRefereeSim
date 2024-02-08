using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameUtilities
{
    //Static class to help with regularly used features in other scripts.

    public static Vector2 CursorPercentage()
    {
        return new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
    }

}
