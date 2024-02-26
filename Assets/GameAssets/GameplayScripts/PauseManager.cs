using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager instance;

    //Reference UI parent this needs to enable and disable along with individual parts it needs to control.


    // Declares itself instance. Do *NOT* give this DontDestroyOnLoad.
    // This should be destroyed and recreated when swapping from main menu to gameplay.
    // Game data and settings should be stored in common scripts.
    private void Awake()
    {
        if (instance == null)
        {
            instance = gameObject.GetComponent<PauseManager>();
        }
        else
        {
            Destroy(gameObject);
        }

        GameplayEvents.SetPause.AddListener(PauseGame);
    }

    public void PauseGame(bool pausing)
    {
        if (pausing)
        {

        }
        else
        {

        }
    }
}
