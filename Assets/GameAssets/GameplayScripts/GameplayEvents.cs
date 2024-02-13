using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

static public class GameplayEvents
{
    public static UnityEvent EndPlay = new UnityEvent();
    static UnityEvent<bool> SetPause = new UnityEvent<bool>();
    public static UnityEvent<int> CutsceneTrigger = new UnityEvent<int>();
}
