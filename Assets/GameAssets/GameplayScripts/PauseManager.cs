using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField]
    float pauseTransitionTime;

    [SerializeField]
    RectTransform pauseMenu;

    public static PauseManager instance;

    Vector3 speed = Vector3.zero;
    bool paused;
    float pauseTimer;
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

    private void Update()
    {
        if (paused && pauseTimer < pauseTransitionTime)
        {
            pauseTimer += Time.deltaTime;
        }

        if (!paused && pauseTimer > 0)
        {
            pauseTimer -= Time.deltaTime;
        }
        pauseMenu.anchorMax = Vector3.SmoothDamp(new Vector3(0, 1), new Vector3(1, 1), ref speed, pauseTransitionTime);
        pauseMenu.anchorMin = Vector3.SmoothDamp(new Vector3(-1, 0), new Vector3(0, 0), ref speed, pauseTransitionTime);
    }

    public void PauseGame(bool pausing)
    {
        paused = pausing;
    }
}
