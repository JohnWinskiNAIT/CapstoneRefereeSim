using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

static public class GameplayEvents
{
    public static UnityEvent EndPlay = new();
    public static UnityEvent<bool> SetPause = new();
    public static UnityEvent<int> CutsceneTrigger = new();
    public static UnityEvent<bool> OpenWheel = new();

    public static UnityEvent<CutsceneData> LoadCutscene = new();
    public static UnityEvent EndCutscene = new();
    public static UnityEvent InitializePlay = new();
}
